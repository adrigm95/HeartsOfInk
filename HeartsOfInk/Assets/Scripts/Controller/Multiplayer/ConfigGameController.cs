using AnalyticsServer.Models;
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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigGameController : MonoBehaviour
{
    /// <summary>
    /// Tiempo en segundos tras el cual hay que notificar al servidor que aun estamos configurando la partida.
    /// </summary>
    private const float TimeToNotifyActive = 300;

    private const byte NoAlliance = 0;

    /// <summary>
    /// Momento en que se comunica con el server por última vez, expresado en segundos desde el inicio de la partida.
    /// </summary>
    private float lastAdviceToServer;

    [NonSerialized]
    public bool isGameHost;

    private WebServiceCaller<LogExceptionDto, bool> exceptionSender =
    new WebServiceCaller<LogExceptionDto, bool>();
    private WebServiceCaller<LogDto, bool> logSender =
        new WebServiceCaller<LogDto, bool>();
    private int ownLine;
    private MapModel _mapModel;
    private GlobalInfo globalInfo;
    private DropdownIndexer factionDropdownsIds;
    public List<ConfigLineModel> _configLinesState;
    public int startLines;
    public int spacing;
    public Text txtGamekey;
    public Text factionDescription;
    public Text bonusDescription;
    public Text txtBtnReady;
    public InputField inpNick;
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
            Transform objectLine = null;
            Dropdown cbPlayerType;
            Image colorFactionImage;
            Button btnColorFaction;
            Text txtPlayerName;
            GlobalInfoFaction faction;
            Button btnAlliance;
            Text txtAlliance;
            Toggle tglIsReady;

            try
            {
                Debug.Log("UpdateConfigLines - Start - Player modification received: " + configLine.PlayerName);
                ConfigLineModel localConfigLine = _configLinesState.Find(item => item.MapPlayerSlotId == configLine.MapPlayerSlotId);

                faction = globalInfo.Factions.Find(item => item.Id == configLine.FactionId);
                string objectName = "factionLine" + "_" + configLine.MapPlayerSlotId;
                GetObjectLineReferences(ref objectLine, out cbPlayerType, out colorFactionImage, out btnColorFaction, 
                    out txtPlayerName, out btnAlliance, out txtAlliance, out tglIsReady, objectName);
                cbPlayerType.value = (int)configLine.PlayerType;
                colorFactionImage.color = ColorUtils.GetColorByString(configLine.Color);
                localConfigLine.PlayerType = configLine.PlayerType;
                localConfigLine.Color = configLine.Color;

                if (configLine.MapPlayerSlotId == ownLine)
                {
                    if (!tglIsReady.isOn)
                    {
                        // El HOST puede cambiar la facción/alianza a otros jugadores si no han marcado como ready su linea. 
                        ChangeFactionDescriptions(faction);
                        txtAlliance.text = Convert.ToString(configLine.Alliance);
                        localConfigLine.Alliance = configLine.Alliance;
                    }
                }
                else
                {
                    // Algunos datos no se pueden actualizar desde fuera para la linea propia pero si para el resto de lineas.
                    if (string.IsNullOrWhiteSpace(configLine.PlayerName))
                    {
                        cbPlayerType.gameObject.SetActive(true);
                        txtPlayerName.gameObject.SetActive(false);
                    }
                    else
                    {
                        cbPlayerType.gameObject.SetActive(false);
                        txtPlayerName.text = configLine.PlayerName;
                        localConfigLine.PlayerName = configLine.PlayerName;
                        txtPlayerName.gameObject.SetActive(true);
                    }

                    txtAlliance.text = Convert.ToString(configLine.Alliance);
                    localConfigLine.Alliance = configLine.Alliance;
                    tglIsReady.isOn = configLine.IsReady;
                    localConfigLine.IsReady = configLine.IsReady;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                LogManager.SendException(exceptionSender, ex, ex.Message, SceneManager.GetActiveScene().name);
                throw;
            }
        }
    }

    private void GetObjectLineReferences(ref Transform objectLine, out Dropdown cbPlayerType, out Image colorFactionImage, out Button btnColorFaction, out Text txtPlayerName, out Button btnAlliance, out Text txtAlliance, out Toggle tglIsReady, string objectLineName)
    {
        try
        {
            objectLine = objectLine != null ? objectLine : GameObject.Find(objectLineName).transform;
            cbPlayerType = objectLine.Find("cbPlayerType").GetComponent<Dropdown>();
            colorFactionImage = objectLine.Find("btnColorFaction").GetComponent<Image>();
            btnColorFaction = objectLine.Find("btnColorFaction").GetComponent<Button>();
            btnAlliance = objectLine.Find("btnAlliance").GetComponent<Button>();
            txtAlliance = objectLine.Find("btnAlliance").GetComponentInChildren<Text>();
            txtPlayerName = objectLine.Find("txtPlayerName").GetComponent<Text>();
            tglIsReady = objectLine.Find("tglIsReady").GetComponent<Toggle>();
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex, ex.Message, SceneManager.GetActiveScene().name);
            throw;
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
            startGameController.UpdateGameModel(new GameModel(mapId)
            {
                GameKey = gameKey
            });
            _mapModel = MapDAC.LoadMapInfoById(mapId, Application.persistentDataPath);
            globalInfo = GlobalInfoDAC.LoadGlobalMapInfo();
            LobbyHOIHub.Instance.SuscribeToRoom(txtGamekey.text);
            MapController.Instance.UpdateMap(_mapModel.SpriteName);

            txtBtnReady.text = isGameHost ? "Comenzar" : "Listo";

            if (configLines == null)
            {
                configLines = LoadConfigLinesFromMap(true);
                configLines[0].PlayerName = inpNick.text;
                Debug.Log("ConfigLines received empty, this is ok if are creating game.");
            }

            factionDropdownsIds = new DropdownIndexer();
            foreach (ConfigLineModel configLine in configLines)
            {
                GlobalInfoFaction globalInfoFaction = globalInfo.Factions.Find(faction => faction.Id == configLine.FactionId);
                if (factionDropdownsIds.GetValue(configLine.FactionId) == -1)
                {
                    factionDropdownsIds.AddRegister(configLine.FactionId, globalInfoFaction.NameLiteral);
                }
            }

            ownLine = configLines.Find(item => item.PlayerName == inpNick.text).MapPlayerSlotId;
            for (int index = 0; index < configLines.Count; index++)
            {
                bool lineEnabled = isGameHost || ownLine == configLines[index].MapPlayerSlotId;

                LoadFactionLine(configLines, index, lineEnabled);
            }

            this.isGameHost = isGameHost;
            _configLinesState = configLines;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error message: " + ex.Message);
            Debug.LogError("Error StackTrace: " + ex.StackTrace);
            Debug.LogError($"MethodImput... gameKey: {gameKey}, mapId: {mapId}, isGameHost: {isGameHost}");

            foreach (var configLine in configLines)
            {
                Debug.LogError("Objects (configLine): " + configLine.ToString());
            }

            LogManager.SendException(exceptionSender, ex, ex.Message, SceneManager.GetActiveScene().name);
        }
    }

    public List<ConfigLineModel> GetConfigLinesForMultiplayer()
    {
        List<ConfigLineModel> allLines;

        try
        {
            foreach (var configLine in _configLinesState)
            {
                if (configLine.PlayerType == Player.IA.PLAYER)
                {
                    if (string.IsNullOrEmpty(configLine.PlayerName))
                    {
                        configLine.PlayerType = Player.IA.IA;
                    }
                    else if (configLine.PlayerName != inpNick.text)
                    {
                        Debug.Log("Player name different: configLine.PlayerName: " + configLine.PlayerName + " inpNick.name: " + inpNick.text);
                        configLine.PlayerType = Player.IA.OTHER_PLAYER;
                    }
                }
            }

            // Cargamos las facciones no jugables (ciudades libres).
            allLines = LoadConfigLinesFromMap(false);
            allLines.AddRange(_configLinesState);
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex, ex.Message, SceneManager.GetActiveScene().name);
            throw;
        }

        return allLines;
    }

    private List<ConfigLineModel> LoadConfigLinesFromMap(bool getPlayables)
    {
        List<ConfigLineModel> configLines = new List<ConfigLineModel>();
        List<MapPlayerSlotModel> loadedLines;

        try
        {
            if (getPlayables)
            {
                loadedLines = _mapModel.PlayerSlots.FindAll(p => p.IsPlayable);
            }
            else
            {
                loadedLines = _mapModel.PlayerSlots.FindAll(p => !p.IsPlayable);
            }

            foreach (MapPlayerSlotModel playerSlot in loadedLines)
            {
                configLines.Add(new ConfigLineModel()
                {
                    Color = playerSlot.Color,
                    FactionId = playerSlot.FactionId,
                    MapPlayerSlotId = playerSlot.Id,
                    PlayerName = string.Empty,
                    PlayerType = (int)Player.IA.PLAYER
                });
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex, ex.Message, SceneManager.GetActiveScene().name);
            throw;
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
        Dropdown cbPlayerType;
        Text txtAlliance;
        Text txtPlayerName;
        Image colorFactionImage;
        Dropdown cbFaction;
        Toggle tglIsReady;

        Debug.Log("OnChangeConfigLine - Start - Object modified: " + factionLine.name);

        GetObjectLineReferences(ref factionLine, out cbPlayerType, out colorFactionImage, out _, out txtPlayerName, out _, out txtAlliance, out tglIsReady, null);
        cbFaction = factionLine.Find("cbFaction").GetComponent<Dropdown>();

        string[] splittedName = factionLine.name.Split('_');
        int mapPlayerSlotId = Convert.ToInt32(splittedName[1]);
        ConfigLineModel configLine = _configLinesState.Find(item => item.MapPlayerSlotId == mapPlayerSlotId);
        Debug.Log("Pre-modified configLine || Playername: " + configLine.PlayerName);

        if (isGameHost || configLine.MapPlayerSlotId == ownLine)
        {
            configLine.FactionId = globalInfo.Factions.Find(item =>
                    item.NameLiteral == cbFaction.options[cbFaction.value].text).Id;
            configLine.Alliance = string.IsNullOrWhiteSpace(txtAlliance.text) ? NoAlliance : Convert.ToByte(txtAlliance.text);
            configLine.Color = ColorUtils.GetStringByColor(colorFactionImage.color);
            configLine.PlayerType = (Player.IA)cbPlayerType.value;
            configLine.PlayerName = txtPlayerName.text;
            configLine.IsReady = tglIsReady.isOn;

            configLineIn.configLineModel = configLine;
            configLineIn.gameKey = txtGamekey.text;

            Debug.Log("Post-modified configLine || Playername: " + configLine.PlayerName);
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
            newObject.name = "factionLine" + "_" + configLine.MapPlayerSlotId;
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
            LogManager.SendException(exceptionSender, ex, ex.Message, SceneManager.GetActiveScene().name);
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
