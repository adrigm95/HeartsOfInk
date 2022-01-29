using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
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
    private string gameKey;

    private MapModel mapModel;
    private GlobalInfo globalInfo;
    private List<Dropdown> factions;
    public int startLines;
    public int spacing;
    public Text factionDescription;
    public Text bonusDescription;

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
        this.gameKey = gameKey;
        LoadMap(mapName);
    }

    public async void NotifyActive()
    {
        WebServiceCaller<string, bool> wsCaller = new WebServiceCaller<string, bool>();
        HOIResponseModel<bool> response;

        try
        {
            response = await wsCaller.GenericWebServiceCaller(Method.POST, LobbyHOIControllers.NotifyActive, gameKey);

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

    public void LoadMap(string mapName)
    {
        mapModel = MapDAC.LoadMapInfo(mapName);
        globalInfo = MapDAC.LoadGlobalMapInfo();
        factions = new List<Dropdown>();

        foreach (MapPlayerModel player in mapModel.Players)
        {
            LoadFactionLine(player);
        }
    }

    public void LoadFactionLine(MapPlayerModel player)
    {
        string prefabPath = "Prefabs/fileFactionMultiplayer";
        Transform newObject;
        Vector3 position;
        Dropdown cbFaction;
        Image btnColorFaction;
        GlobalInfoFaction faction;

        faction = globalInfo.Factions.Find(item => item.Id == player.FactionId);
        position = new Vector3(0, startLines);
        position.y -= spacing * factions.Count;
        newObject = ((GameObject)Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine" + faction.Names[0].Value + "_" + faction.Id + "_" + player.MapSocketId;
        newObject.SetParent(this.transform, false);

        cbFaction = newObject.Find("cbFaction").GetComponent<Dropdown>();
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Image>();

        cbFaction.value = player.IaId;
        cbFaction.onValueChanged.AddListener(delegate { OnValueChange(cbFaction); });
        btnColorFaction.color = ColorUtils.GetColorByString(player.Color);

        //txtPlayerName.text = faction.Names[0].Value;

        factions.Add(cbFaction);

        if (player.IaId == PlayerOwnerValue)
        {
            ChangeFactionDescriptions(faction);
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
