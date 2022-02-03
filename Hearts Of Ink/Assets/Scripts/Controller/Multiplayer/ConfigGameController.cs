using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using LobbyHOIServer.Models.Models;
using LobbyHOIServer.Models.Models.In;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigGameController : MonoBehaviour
{
    /// <summary>
    /// Tiempo en segundos tras el cual hay que notificar al servidor que aun estamos configurando la partida.
    /// </summary>
    private const float timeToNotifyActive = 300;

    /// <summary>
    /// Indica si actualmente hay una partida creada por el jugador local en fase de configuración.
    /// </summary>
    private bool activeGameCreated;

    /// <summary>
    /// Momento en que se comunica con el server por última vez, expresado en segundos desde el inicio de la partida.
    /// </summary>
    private float lastAdviceToServer;

    private const int PlayerOwnerValue = 0;
    private MapModel mapModel;
    private GlobalInfo globalInfo;
    public int startLines;
    public int spacing;
    public Text txtGamekey;
    public Text factionDescription;
    public Text bonusDescription;
    public InputField playerName;
    public MapController mapController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activeGameCreated)
        {
            if (Time.time > lastAdviceToServer + timeToNotifyActive)
            {
                NotifyActive();
                lastAdviceToServer = Time.time;
            }
        }
    }

    public void GameCreatedByHost(string gameKey, string mapName)
    {
        this.gameObject.SetActive(true);
        this.txtGamekey.text = gameKey;
        LoadConfigGame(null, mapName);
    }

    public async void NotifyActive()
    {
        WebServiceCaller<string, bool> wsCaller = new WebServiceCaller<string, bool>();
        HOIResponseModel<bool> response;

        try
        {
            response = await wsCaller.GenericWebServiceCaller(Method.POST, LobbyHOIControllers.NotifyActive, txtGamekey.text);

            if (!response.serviceResponse)
            {
                Debug.LogWarning("Server response false on " + LobbyHOIControllers.NotifyActive + " service call.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void LoadConfigGame(List<ConfigLineModel> configLines, string mapName)
    {
        mapModel = MapDAC.LoadMapInfo(mapName);
        globalInfo = MapDAC.LoadGlobalMapInfo();

        mapController.UpdateMap(mapModel.SpritePath);

        if (configLines == null)
        {
            configLines = LoadConfigLinesFromMap();
            configLines[0].PlayerName = playerName.text;
        }

        for (int index = 0; index < configLines.Count; index++)
        {
            LoadFactionLine(configLines[index], index);
        }
    }

    private List<ConfigLineModel> LoadConfigLinesFromMap()
    {
        List<ConfigLineModel> configLines = new List<ConfigLineModel>();

        foreach (MapPlayerModel player in mapModel.Players)
        {
            configLines.Add(new ConfigLineModel()
            {
                Color = player.Color,
                FactionId = player.FactionId,
                MapSocketId = player.MapSocketId,
                PlayerName = string.Empty,
                PlayerType = Player.IA.PLAYER
            });
        }

        return configLines;
    }

    private void LoadFactionsCombo(Dropdown cbFactions)
    {
        globalInfo.Factions.ForEach(faction => cbFactions.options.Add(new Dropdown.OptionData(faction.Names[0].Value)));
        cbFactions.RefreshShownValue();
        cbFactions.onValueChanged.AddListener(delegate { OnChangeConfigLine(cbFactions.transform.parent); });
    }

    private async void OnChangeConfigLine(Transform factionLine)
    {
        WebServiceCaller<ConfigLineIn, bool> webServiceCaller = new WebServiceCaller<ConfigLineIn, bool>();
        ConfigLineIn configLineIn = new ConfigLineIn();
        Dropdown cbFaction = factionLine.Find("cbFaction").GetComponent<Dropdown>();
        HOIResponseModel<bool> response;

        configLineIn.gameKey = txtGamekey.text;
        configLineIn.configLineModel = new ConfigLineModel()
        {
            FactionId = globalInfo.Factions.Find(item =>
                item.Names[0].Value == cbFaction.options[cbFaction.value].text).Id
        };

        response = await webServiceCaller.GenericWebServiceCaller(Method.PUT, LobbyHOIControllers.ConfigLine, configLineIn);
    }

    public void LoadFactionLine(ConfigLineModel configLine, int index)
    {
        string prefabPath = "Prefabs/fileFactionMultiplayer";
        Transform newObject;
        Vector3 position;
        Dropdown cbPlayerType;
        Image btnColorFaction;
        Text txtPlayerName;
        GlobalInfoFaction faction;

        faction = globalInfo.Factions.Find(item => item.Id == configLine.FactionId);
        position = new Vector3(0, startLines);
        position.y -= spacing * index;
        newObject = ((GameObject)Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine" + faction.Names[0].Value + "_" + faction.Id + "_" + configLine.MapSocketId;
        newObject.SetParent(this.transform, false);

        LoadFactionsCombo(newObject.Find("cbFaction").GetComponent<Dropdown>());
        cbPlayerType = newObject.Find("cbPlayerType").GetComponent<Dropdown>();
        cbPlayerType.value = (int) configLine.PlayerType;
        cbPlayerType.onValueChanged.AddListener(delegate { OnValueChange(cbPlayerType); });
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Image>();
        btnColorFaction.color = ColorUtils.GetColorByString(configLine.Color);

        if (!string.IsNullOrEmpty(configLine.PlayerName))
        {
            ChangeFactionDescriptions(faction);
            cbPlayerType.gameObject.SetActive(false);
            txtPlayerName = newObject.Find("txtPlayerName").GetComponent<Text>();
            txtPlayerName.text = configLine.PlayerName;
            txtPlayerName.gameObject.SetActive(true);
        }
    }

    public void OnValueChange(Dropdown comboOrder)
    {
        if (comboOrder.value == 0)
        {
            int comboFactionId = Convert.ToInt32(comboOrder.transform.parent.gameObject.name.Split('_')[1]);
            GlobalInfoFaction globalInfoFaction = globalInfo.Factions.Find(faction => faction.Id == comboFactionId);

            ChangeFactionDescriptions(globalInfoFaction);
        }
    }

    private void ChangeFactionDescriptions(GlobalInfoFaction faction)
    {
        Debug.Log("New faction:" + faction.Names[0].Value);
        factionDescription.text = faction.Descriptions[0].Value;
        bonusDescription.text = globalInfo.Bonus.Find(bonus => bonus.Id == faction.BonusId).Descriptions[0].Value;
    }
}
