using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
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
    /// Momento en que se comunica con el server por última vez, expresado en segundos desde el inicio de la partida.
    /// </summary>
    private float lastAdviceToServer;

    private bool isGameHost;
    private int ownLine;
    private List<ConfigLineModel> _configLinesState;
    private MapModel mapModel;
    private GlobalInfo globalInfo;
    public int startLines;
    public int spacing;
    public Text txtGamekey;
    public Text factionDescription;
    public Text bonusDescription;
    public InputField playerName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            UpdateConfigLines();

            if (Time.time > lastAdviceToServer + timeToNotifyActive)
            {
                NotifyActive();
                lastAdviceToServer = Time.time;
            }
        }
    }

    private void UpdateConfigLines()
    {
        foreach (ConfigLineModel configLine in ConfigLinesUpdater.Instance.GetReceivedConfigLines())
        {
            Transform ObjectLine;
            Vector3 position;
            Dropdown cbPlayerType;
            Image colorFactionImage;
            Button btnColorFaction;
            Text txtPlayerName;
            GlobalInfoFaction faction;
            Button btnAlliance;
            Text txtAlliance;

            try
            {
                faction = globalInfo.Factions.Find(item => item.Id == configLine.FactionId);
                string ObjetName = "factionLine" + "_" + configLine.MapSocketId;
                ObjectLine = GameObject.Find(ObjetName).transform;
                cbPlayerType = ObjectLine.Find("cbPlayerType").GetComponent<Dropdown>();
                colorFactionImage = ObjectLine.Find("btnColorFaction").GetComponent<Image>();
                btnColorFaction = ObjectLine.Find("btnColorFaction").GetComponent<Button>();
                btnAlliance = ObjectLine.Find("btnAlliance").GetComponent<Button>();
                txtAlliance = ObjectLine.Find("btnAlliance").GetComponentInChildren<Text>();
                cbPlayerType.value = (int)configLine.PlayerType;              
                colorFactionImage.color = ColorUtils.GetColorByString(configLine.Color);
                if (!string.IsNullOrEmpty(configLine.PlayerName))
                {
                    ChangeFactionDescriptions(faction);
                    cbPlayerType.gameObject.SetActive(false);
                    txtPlayerName = ObjectLine.Find("txtPlayerName").GetComponent<Text>();
                    txtPlayerName.text = configLine.PlayerName;
                    txtPlayerName.gameObject.SetActive(true);
                }
                txtAlliance.text =  Convert.ToString(configLine.Alliance);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }
    }

    public void GameCreatedByHost(string gameKey, short mapId)
    {
        LoadConfigGame(null, gameKey, mapId, true);
    }

    public async void NotifyActive()
    {
        WebServiceCaller<string, bool> wsCaller = new WebServiceCaller<string, bool>();
        HOIResponseModel<bool> response;

        try
        {
            response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.POST, LobbyHOIControllers.NotifyActive, txtGamekey.text);

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

    public void LoadConfigGame(List<ConfigLineModel> configLines, string gameKey, short mapId, bool isGameHost)
    {
        this.txtGamekey.text = gameKey;
        this.gameObject.SetActive(true);
        mapModel = MapDAC.LoadMapInfo(mapId);
        globalInfo = MapDAC.LoadGlobalMapInfo();
        LobbyHOIHub.Instance.SuscribeToRoom(txtGamekey.text);
        MapController.Instance.UpdateMap(mapModel.SpritePath);

        if (configLines == null)
        {
            configLines = LoadConfigLinesFromMap();
            configLines[0].PlayerName = playerName.text;
            Debug.Log("ConfigLines received empty, this is ok if are creating game.");
        }

        ownLine = configLines.Find(item => item.PlayerName == playerName.text).MapSocketId;

        for (int index = 0; index < configLines.Count; index++)
        {
            bool lineEnabled = isGameHost || ownLine == configLines[index].MapSocketId;

            LoadFactionLine(configLines[index], index, lineEnabled);
        }

        this.isGameHost = isGameHost;        
        _configLinesState = configLines;
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
                PlayerType = (int) Player.IA.PLAYER
            });
        }

        return configLines;
    }

    private void LoadFactionsCombo(Dropdown cbFactions, bool isEnabled)
    {
        globalInfo.Factions.ForEach(faction => cbFactions.options.Add(new Dropdown.OptionData(faction.Names[0].Value)));
        cbFactions.RefreshShownValue();

        if (isEnabled)
        {
            cbFactions.onValueChanged.AddListener(delegate { OnChangeConfigLine(cbFactions.transform.parent); });
        }
        else
        {
            cbFactions.enabled = false;
        }
    }

    public void OnChangeConfigLine(Transform factionLine)
    {
        ConfigLineIn configLineIn = new ConfigLineIn();
        Dropdown cbFaction = factionLine.Find("cbFaction").GetComponent<Dropdown>();
        Dropdown cbPlayerType = factionLine.Find("cbPlayerType").GetComponent<Dropdown>();
        Image colorFactionImage = factionLine.Find("btnColorFaction").GetComponent<Image>();
        Button btnColorFaction = factionLine.Find("btnColorFaction").GetComponent<Button>();
        Text txtAlliance = factionLine.Find("btnAlliance").GetComponentInChildren<Text>();

        string[] splittedName = factionLine.name.Split('_');
        int mapSocketId = Convert.ToInt32(splittedName[1]);
        ConfigLineModel configLine = _configLinesState.Find(item => item.MapSocketId == mapSocketId);

        if (isGameHost || configLine.MapSocketId == ownLine)
        {
            configLine.FactionId = globalInfo.Factions.Find(item =>
                    item.Names[0].Value == cbFaction.options[cbFaction.value].text).Id;
            configLine.Alliance = Convert.ToByte(txtAlliance.text);
            configLine.Color = ColorUtils.GetStringByColor(colorFactionImage.color);
            configLine.PlayerType = (Player.IA)cbPlayerType.value;

            configLineIn.configLineModel = configLine;
            configLineIn.gameKey = txtGamekey.text;

            ConfigLinesUpdater.Instance.SendConfigLine(configLineIn);
        }
    }

    public void LoadFactionLine(ConfigLineModel configLine, int index, bool isEnabled)
    {
        string prefabPath = "Prefabs/fileFactionMultiplayer";
        Transform newObject;
        Vector3 position;
        Dropdown cbPlayerType;
        Image colorFactionImage;
        Button btnColorFaction;
        Text txtPlayerName;
        GlobalInfoFaction faction;
        Button btnAlliance;
        FactionItemController factionItemController;

        Debug.Log("LoadFactionLine - Start");

        try
        {
            faction = globalInfo.Factions.Find(item => item.Id == configLine.FactionId);
            position = new Vector3(0, startLines);
            position.y -= spacing * index;
            newObject = ((GameObject)Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
            newObject.name = "factionLine" + "_" + configLine.MapSocketId;
            newObject.SetParent(this.transform, false);

            LoadFactionsCombo(newObject.Find("cbFaction").GetComponent<Dropdown>(), isEnabled);
            cbPlayerType = newObject.Find("cbPlayerType").GetComponent<Dropdown>();
            colorFactionImage = newObject.Find("btnColorFaction").GetComponent<Image>();
            btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Button>();
            btnAlliance = newObject.Find("btnAlliance").GetComponent<Button>();
            factionItemController = newObject.GetComponent<FactionItemController>();
            factionItemController.configGameController = this;

            cbPlayerType.value = (int)configLine.PlayerType;

            if (isEnabled)
            {
                cbPlayerType.onValueChanged.AddListener(delegate { OnValueChange_Faction(cbPlayerType); });
                btnColorFaction.onClick.AddListener(delegate { OnClick_PlayerColor(colorFactionImage); });
            }
            else
            {
                cbPlayerType.enabled = false;
                btnColorFaction.enabled = false;
                btnAlliance.enabled = false;
            }

            colorFactionImage.color = ColorUtils.GetColorByString(configLine.Color);

            if (!string.IsNullOrEmpty(configLine.PlayerName))
            {
                ChangeFactionDescriptions(faction);
                cbPlayerType.gameObject.SetActive(false);
                txtPlayerName = newObject.Find("txtPlayerName").GetComponent<Text>();
                txtPlayerName.text = configLine.PlayerName;
                txtPlayerName.gameObject.SetActive(true);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            throw;
        }
    }

    public void OnValueChange_Faction(Dropdown comboOrder)
    {
        if (comboOrder.value == 0)
        {
            string comboFactionName = comboOrder.options[comboOrder.value].text;
            GlobalInfoFaction globalInfoFaction = globalInfo.Factions.Find(faction => faction.Names[0].Value == comboFactionName);
            ChangeFactionDescriptions(globalInfoFaction);
        }
    }

    public void OnClick_PlayerColor(Image colorImage)
    {
        Debug.Log($"Color changed for image {colorImage.name}; color: {colorImage.color}");
        Debug.Log($"Current global info {globalInfo}");
        colorImage.color = ColorUtils.NextColor(colorImage.color, globalInfo.AvailableColors);
        OnChangeConfigLine(colorImage.transform.parent);
    }

    private void ChangeFactionDescriptions(GlobalInfoFaction faction)
    {
        Debug.Log("New faction:" + faction.Names[0].Value);
        factionDescription.text = faction.Descriptions[0].Value;
        bonusDescription.text = globalInfo.Bonus.Find(bonus => bonus.Id == faction.BonusId).Descriptions[0].Value;
    }
}
