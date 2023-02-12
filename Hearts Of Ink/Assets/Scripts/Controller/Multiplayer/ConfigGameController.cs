using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using LobbyHOIServer.Models.Models;
using LobbyHOIServer.Models.Models.In;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigGameController : MonoBehaviour
{
    private readonly string RootPath = Application.streamingAssetsPath;
    /// <summary>
    /// Tiempo en segundos tras el cual hay que notificar al servidor que aun estamos configurando la partida.
    /// </summary>
    private const float TimeToNotifyActive = 300;

    private const byte NoAlliance = 0;

    /// <summary>
    /// Momento en que se comunica con el server por última vez, expresado en segundos desde el inicio de la partida.
    /// </summary>
    private float lastAdviceToServer;

    private bool isGameHost;
    private int ownLine;
    private MapModel _mapModel;
    private GlobalInfo globalInfo;
    private DropdownIndexer factionDropdownsIds;
    private List<ConfigLineModel> _configLinesState;
    public int startLines;
    public int spacing;
    public Text txtGamekey;
    public Text factionDescription;
    public Text bonusDescription;
    public InputField playerName;
    public StartGameController startGameController;

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

            if (Time.time > lastAdviceToServer + TimeToNotifyActive)
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
            Transform objectLine;
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
                string objectName = "factionLine" + "_" + configLine.MapSocketId;
                objectLine = GameObject.Find(objectName).transform;
                cbPlayerType = objectLine.Find("cbPlayerType").GetComponent<Dropdown>();
                colorFactionImage = objectLine.Find("btnColorFaction").GetComponent<Image>();
                btnColorFaction = objectLine.Find("btnColorFaction").GetComponent<Button>();
                btnAlliance = objectLine.Find("btnAlliance").GetComponent<Button>();
                txtAlliance = objectLine.Find("btnAlliance").GetComponentInChildren<Text>();
                cbPlayerType.value = (int)configLine.PlayerType;              
                colorFactionImage.color = ColorUtils.GetColorByString(configLine.Color);
                if (!string.IsNullOrEmpty(configLine.PlayerName))
                {
                    ChangeFactionDescriptions(faction);
                    cbPlayerType.gameObject.SetActive(false);
                    txtPlayerName = objectLine.Find("txtPlayerName").GetComponent<Text>();
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

    public void GameCreatedByHost(string gameKey, string mapId)
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

    public void LoadConfigGame(List<ConfigLineModel> configLines, string gameKey, string mapId, bool isGameHost)
    {
        try
        {
            this.txtGamekey.text = gameKey;
            this.gameObject.SetActive(true);
            startGameController.MapId = mapId;
            _mapModel = MapDAC.LoadMapInfoById(mapId, RootPath);
            globalInfo = GlobalInfoDAC.LoadGlobalMapInfo();
            LobbyHOIHub.Instance.SuscribeToRoom(txtGamekey.text);
            MapController.Instance.UpdateMap(_mapModel.SpritePath);

            if (configLines == null)
            {
                configLines = LoadConfigLinesFromMap(true);
                configLines[0].PlayerName = playerName.text;
                Debug.Log("ConfigLines received empty, this is ok if are creating game.");
            }

            factionDropdownsIds = new DropdownIndexer();
            foreach (ConfigLineModel configLine in configLines)
            {
                GlobalInfoFaction globalInfoFaction = globalInfo.Factions.Find(faction => faction.Id == configLine.FactionId);
                factionDropdownsIds.AddRegister(configLine.FactionId, globalInfoFaction.NameLiteral);
            }

            ownLine = configLines.Find(item => item.PlayerName == playerName.text).MapSocketId;
            for (int index = 0; index < configLines.Count; index++)
            {
                bool lineEnabled = isGameHost || ownLine == configLines[index].MapSocketId;

                LoadFactionLine(configLines, index, lineEnabled);
            }

            this.isGameHost = isGameHost;
            _configLinesState = configLines;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error message: " + ex.Message);
            Debug.LogError("Error StackTrace: " + ex.Message);
            Debug.LogError($"MethodImput... gameKey: {gameKey}, mapId: {mapId}, isGameHost: {isGameHost}");

            foreach (var configLine in configLines)
            {
                Debug.LogError("Objects (configLine): " + configLine.ToString());
            }
        }
    }

    public List<ConfigLineModel> GetConfigLinesForMultiplayer()
    {
        List<ConfigLineModel> allLines;

        allLines = LoadConfigLinesFromMap(false);
        allLines.AddRange(_configLinesState);

        return allLines;
    }

    private List<ConfigLineModel> LoadConfigLinesFromMap(bool getPlayables)
    {
        List<ConfigLineModel> configLines = new List<ConfigLineModel>();
        List<MapPlayerModel> loadedLines;

        if (getPlayables)
        {
            loadedLines = _mapModel.Players.FindAll(p => p.IsPlayable);
        }
        else
        {
            loadedLines = _mapModel.Players.FindAll(p => !p.IsPlayable);
        }

        foreach (MapPlayerModel player in loadedLines)
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

    private void LoadFactionsCombo(ConfigLineModel currentLine, Dropdown cbFactions, bool isEnabled)
    {
        cbFactions.options.AddRange(factionDropdownsIds.GetOptions());
        cbFactions.RefreshShownValue();
        cbFactions.value = factionDropdownsIds.GetValue(currentLine.FactionId);

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
                    item.NameLiteral == cbFaction.options[cbFaction.value].text).Id;
            configLine.Alliance = string.IsNullOrWhiteSpace(txtAlliance.text) ? NoAlliance : Convert.ToByte(txtAlliance.text);
            configLine.Color = ColorUtils.GetStringByColor(colorFactionImage.color);
            configLine.PlayerType = (Player.IA)cbPlayerType.value;

            configLineIn.configLineModel = configLine;
            configLineIn.gameKey = txtGamekey.text;

            ConfigLinesUpdater.Instance.SendConfigLine(configLineIn);
        }
    }

    public void LoadFactionLine(List<ConfigLineModel> configLines, int index, bool isEnabled)
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
        ConfigLineModel configLine;

        Debug.Log("LoadFactionLine - Start");

        try
        {
            configLine = configLines[index];
            faction = globalInfo.Factions.Find(item => item.Id == configLine.FactionId);
            position = new Vector3(0, startLines);
            position.y -= spacing * index;
            newObject = ((GameObject)Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
            newObject.name = "factionLine" + "_" + configLine.MapSocketId;
            newObject.SetParent(this.transform, false);

            LoadFactionsCombo(configLine, newObject.Find("cbFaction").GetComponent<Dropdown>(), isEnabled);
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
            GlobalInfoFaction globalInfoFaction = globalInfo.Factions.Find(faction => faction.NameLiteral == comboFactionName);
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
        Debug.Log("New faction:" + faction.NameLiteral);
        factionDescription.text = faction.DescriptionLiteral;
        bonusDescription.text = globalInfo.Bonus.Find(bonus => bonus.Id == faction.BonusId).Descriptions[0].Value;
    }
}
