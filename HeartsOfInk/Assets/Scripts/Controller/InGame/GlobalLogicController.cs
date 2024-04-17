using Assets.Scripts.Controller.InGame;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AnalyticsServer.Models;
using static SceneChangeController;
using UnityEngine.UI;

public class GlobalLogicController : MonoBehaviour
{
    private float LastGameSpeed = 1f;
    private SelectionModel selection;
    private WebServiceCaller<LogDto, bool> logSender = new WebServiceCaller<LogDto, bool>();
    private WebServiceCaller<LogExceptionDto, bool> exceptionSender = new WebServiceCaller<LogExceptionDto, bool>();
    private WebServiceCaller<LogAnalyticsDto, bool> analyticSender = new WebServiceCaller<LogAnalyticsDto, bool>();

    /// <summary>
    /// Contador que se utiliza para que las unidades clonadas no tengan el mismo nombre.
    /// </summary>
    [NonSerialized]
    public int troopsCounter;

    /// <summary>
    /// Contador que se utiliza para que las ciudades clonadas no tengan el mismo nombre.
    /// </summary>
    [NonSerialized]
    public int citiesCounter;

    [NonSerialized]
    public GameModel gameModel;

    public bool IsMultiplayerHost { get { return gameModel.Gametype == GameModel.GameType.MultiplayerHost; } }
    public bool IsMultiplayerClient { get { return gameModel.Gametype == GameModel.GameType.MultiplayerClient; } }
    public bool IsSingleplayer { get { return gameModel.Gametype == GameModel.GameType.Single; } }

    [SerializeField]
    public TargetPositionMarkerController targetMarkerController;
    public List<CityController> cities;
    public List<AILogic> aiLogics;
    public float GameSpeed;
    public Player thisPcPlayer;
    public CameraController cameraController;
    public StatisticsController statisticsController;
    public SceneChangeController sceneChangeController;
    public GameOptionsController gameOptionsHolder;
    public TutorialController tutorialController;
    public WaitingPanelController waitingPanel;
    public GameObject troopsCanvas;
    public GameObject citiesCanvas;
    public GameObject pausePanel;
    public GameObject emptyTargetsHolder;
    public GameObject debugInfoPanel;
    public Image pauseItem;
    public Image playItem;
    public Image mediumSpeedItem;
    public Image fatestSpeedItem;

    private void Awake()
    {
        LogManager.SendLog(logSender, "START - GlobalLogicController");

        try
        {
            aiLogics = new List<AILogic>();
            selection = new SelectionModel();
            cameraController = FindObjectOfType<CameraController>();
            statisticsController = FindObjectOfType<StatisticsController>();
            sceneChangeController = FindObjectOfType<SceneChangeController>();
            gameOptionsHolder = FindObjectOfType<GameOptionsController>();

            if (debugInfoPanel != null && Debug.isDebugBuild)
            {
                debugInfoPanel.SetActive(true);
            }

            if (gameOptionsHolder == null)
            {
                Debug.LogWarning("Load mocked game");
                gameModel = GetMockedGameModel();
            }
            else
            {
                gameModel = gameOptionsHolder.gameModel;
            }

            AwakeIA();
            AwakeMap();
            cities = FindObjectsOfType<CityController>().ToList();

            if (gameModel.Gametype == GameModel.GameType.Single)
            {
                tutorialController.DisplayTutorial();
            }
            else
            {
                ChangeSpeed(GameSpeedConstants.PlaySpeed);
                //waitingPanel.Show(this);
                IngameHOIHub.Instance.SuscribeToRoom(gameModel.GameKey, thisPcPlayer.Name);
                StartGameIngameSignalR.Instance.SendClientReady(gameModel.GameKey);
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSingleplayer || IsMultiplayerHost)
        {
            HostInputManagement();
        }
        else if (IsMultiplayerClient)
        {
            ClientInputManagement();
        }
        else
        {
            string errorMsg = "Unexpected type of game: " + gameModel.Gametype;
            LogManager.SendException(exceptionSender, new Exception(errorMsg));
            Debug.LogWarning(errorMsg);
        }

        if (IsMultiplayerHost || IsMultiplayerClient)
        {
            TroopDeadSignalR.GlobalLogicController = this;
        }

        UpdateUnitAnimation();
        CheckVictoryConditions();
        UpdateMultiselect();
    }

    /// <summary>
    /// Lógica de multiselección.
    /// 
    /// Multiplayer/singleplayer: Sirve para ambos.
    /// </summary>
    private void UpdateMultiselect()
    {
        try
        {
            if (selection.UpdateMultiselect(cameraController.ScreenToWorldPoint(), troopsCanvas.transform, thisPcPlayer.MapSocketId))
            {
                UpdateTargetMarker();
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    private void UpdateTargetMarker()
    {
        GameObject target = null;
        bool allTargetEquals = true;
        TroopController troopController = null;

        try
        {
            if (selection.SelectionType == typeof(TroopController))
            {
                foreach (GameObject selectable in selection.SelectionObjects)
                {
                    troopController = selectable.GetComponent<TroopController>();
                    target = target == null ? troopController.troopModel.Target : target;

                    if (target != null)
                    {
                        if (troopController.troopModel.Target == null)
                        {
                            allTargetEquals = false;
                            break;
                        }
                        else if (target.transform.position != troopController.troopModel.Target.transform.position)
                        {
                            allTargetEquals = false;
                            break;
                        }
                    }
                }

                if (allTargetEquals)
                {
                    targetMarkerController.SetTargetPosition(target, false);
                }
                else
                {
                    targetMarkerController.RemoveTargetPosition();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error on UpdateTargetMarker, message: {ex.Message};");
            Debug.LogError($"Error on UpdateTargetMarker, stacktrace: {ex.StackTrace};");
            Debug.LogError($"Error on UpdateTargetMarker, variable Target: {target};");
            Debug.LogError($"Error on UpdateTargetMarker, variable allTargetEquals: {allTargetEquals};");
            Debug.LogError($"Error on UpdateTargetMarker, variable troopController: {troopController};");
            LogManager.SendException(exceptionSender, ex, $"allTargetEquals: {allTargetEquals}, target: {target}, troopController: {troopController}");
            Debug.LogException(ex);
        }
    }

    public Player GetPlayer(byte mapSocketId)
    {
        return gameModel.Players.First(pl => pl.MapSocketId == mapSocketId);
    }
    public Player GetPlayer(string name)
    {
        return gameModel.Players.First(pl => pl.Name == name);
    }

    private GameModel GetMockedGameModel()
    {
        GameModel gameModel = new GameModel("0");
        gameModel.Gametype = GameModel.GameType.Single;

        for (int index = 0; index < 4; index++)
        {
            Player player = new Player();
            string factionId = Convert.ToString(index);
            player.Faction.Id = Convert.ToInt32(factionId);
            player.MapSocketId = Convert.ToByte(index);
            player.Faction.Bonus = new Bonus(Bonus.Id.None);

            switch (index)
            {
                case 0:
                    player.IaId = Player.IA.PLAYER;
                    player.Name = "Player";
                    player.Color = "255,0,46";
                    player.Alliance = 1;
                    break;
                case 1:
                    player.IaId = Player.IA.IA;
                    player.Name = "Cisneros";
                    player.Color = "88,212,255";
                    break;
                case 2:
                    player.IaId = Player.IA.IA;
                    player.Name = "Djambo";
                    player.Color = "0,234,61";
                    player.Alliance = 1;
                    break;
                case 3:
                    player.IaId = Player.IA.NEUTRAL;
                    player.Name = "Hamraoui";
                    player.Color = "255,223,0";
                    break;
            }

            gameModel.Players.Add(player);
        }

        return gameModel;
    }

    private void AwakeIA()
    {
        foreach (Player player in gameModel.Players)
        {
            switch (player.IaId)
            {
                case Player.IA.IA:
                    aiLogics.Add(new AILogic(player, this));
                    break;
                case Player.IA.PLAYER:
                    thisPcPlayer = player;
                    Debug.Log($"Player assigned {player.Name}");
                    break;
            }
        }
    }

    private void AwakeMap()
    {
        MapModel mapModel = null;
        int index = 0;
        MapCityModel city = null;
        MapTroopModel troop = null;

        try
        {
            Debug.Log($"Loading map id: {gameModel.MapId}");
            mapModel = MapDAC.LoadMapInfoById(gameModel.MapId, GlobalConstants.RootPath);
            Debug.Log($"Map path: {mapModel.SpritePath}");
            MapController.Instance.UpdateMap(mapModel.SpritePath);

            for (index = 0; index < mapModel.Cities.Count; index++)
            {
                city = mapModel.Cities[index];
                Player cityOwner = GetPlayer(city.MapSocketId);
                InstantiateCity(city, cityOwner);
            }

            for (index = 0; index < mapModel.Troops.Count; index++)
            {
                troop = mapModel.Troops[index];
                Player troopOwner = GetPlayer(troop.MapSocketId);
                InstantiateTroopSingleplayer(troop.Units, VectorUtils.FloatVectorToVector3(troop.Position), troopOwner);
            }
        }
        catch (InvalidOperationException ex)
        {
            Debug.LogError($"Any city or troop has an invalid MapSocketId: {ex.Message}; array index {index}");

            if (troop == null)
            {
                Debug.Log($"Last city evaluated: {city}");
            }
            else
            {
                Debug.Log($"Last troop evaluated: {troop}");
            }
        }
    }

    public void InstantiateCity(MapCityModel cityModel, Player cityOwner)
    {
        CityController newObject;
        string prefabName = cityModel.Type == 0 ? "Prefabs/Capital" : "Prefabs/CityPrefab";

        newObject = ((GameObject)Instantiate(
            Resources.Load(prefabName),
            VectorUtils.FloatVectorToVector3(cityModel.Position),
            citiesCanvas.transform.rotation,
            citiesCanvas.transform)
            ).GetComponent<CityController>();
        newObject.name = cityModel.Name + "_" + citiesCounter;
        newObject.Owner = cityOwner;

        citiesCounter++;
    }

    public void InstantiateTroopSingleplayer(int units, Vector3 position, Player troopOwner)
    {
        try
        {
            if (troopOwner.Faction.Bonus.BonusId == Bonus.Id.Recruitment)
            {
                units += 10;
            }

            InstantiateTroop(units, position, troopOwner, troopsCounter.ToString());
            troopsCounter++;
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void InstantiateTroopMultiplayer(string name, int units, Vector3 position, byte mapSocketId)
    {
        InstantiateTroop(units, position, GetPlayer(mapSocketId), name);
    }

    private void InstantiateTroop(int units, Vector3 position, Player troopOwner, string troopId)
    {
        TroopController newObject;

        try
        {

            newObject = ((GameObject)Instantiate(
                Resources.Load("Prefabs/Troop"),
                position,
                troopsCanvas.transform.rotation,
                troopsCanvas.transform)
                ).GetComponent<TroopController>();
            newObject.name = troopId;
            newObject.troopModel = new TroopModel(troopOwner);
            newObject.troopModel.Units = units;
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Lógica multiplayer/singleplayer: Sirve para los dos de forma temporal, revisar funcionamiento a futuro.
    /// </summary>
    private void CheckVictoryConditions()
    {
        bool isGameFinished = true;
        Player firstOwner;
        byte firstOwnerAlliance;

        try
        {
            firstOwner = cities[0].Owner;
            firstOwnerAlliance = cities[0].Owner.Alliance;

            //TODO: Terminar de adaptar condición de victoria a alianzas
            foreach (CityController city in cities)
            {
                if (city.Owner != firstOwner)
                {
                    isGameFinished = false;
                    break;
                }
            }

            if (isGameFinished)
            {
                sceneChangeController.ChangeScene(Scenes.Endgame);
                statisticsController.ReportGameEnd(cities);
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Lógica de control de imputs.
    /// 
    /// Multiplayer/singleplayer: Valida para singleplayer y para el host del multiplayer.
    /// </summary>
    public void HostInputManagement()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pausePanel.SetActive(!pausePanel.activeSelf);
                ChangeSpeed(KeyCode.Escape);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeSpeed(KeyCode.Space);
            }
            else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                ChangeSpeed(KeyCode.Minus);
            }
            else if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                ChangeSpeed(KeyCode.Plus);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                EndSelection();
            }

            if (selection.MultiselectOrigin != null)
            {
                if (Input.GetMouseButtonUp(KeyConstants.LeftClick))
                {
                    selection.MultiselectOrigin = null;
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void ClientInputManagement()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pausePanel.SetActive(!pausePanel.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                EndSelection();
            }

            if (selection.MultiselectOrigin != null)
            {
                if (Input.GetMouseButtonUp(KeyConstants.LeftClick))
                {
                    selection.MultiselectOrigin = null;
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void ChangeSpeed(KeyCode key)
    {
        try
        {
            if (key == KeyCode.Escape)
            {
                if (pausePanel.activeSelf)
                {
                    ChangeSpeed(GameSpeedConstants.PauseSpeed);
                }
                else
                {
                    ChangeSpeed(GameSpeedConstants.PlaySpeed);
                }
            }
            if (key == KeyCode.Space || key == KeyCode.Plus || key == KeyCode.KeypadPlus)
            {
                switch (Time.timeScale)
                {
                    case GameSpeedConstants.PauseSpeed:
                        ChangeSpeed(GameSpeedConstants.PlaySpeed);
                        break;
                    case GameSpeedConstants.PlaySpeed:
                        ChangeSpeed(GameSpeedConstants.MediumSpeed);
                        break;
                    case GameSpeedConstants.MediumSpeed:
                        ChangeSpeed(GameSpeedConstants.FatestSpeed);
                        break;
                    case GameSpeedConstants.FatestSpeed:
                        if (key == KeyCode.Space)
                        {
                            ChangeSpeed(GameSpeedConstants.PauseSpeed);
                        }
                        break;
                }
            }
            else if (key == KeyCode.Minus || key == KeyCode.KeypadMinus)
            {
                switch (Time.timeScale)
                {
                    case GameSpeedConstants.FatestSpeed:
                        ChangeSpeed(GameSpeedConstants.MediumSpeed);
                        break;
                    case GameSpeedConstants.MediumSpeed:
                        ChangeSpeed(GameSpeedConstants.PlaySpeed);
                        break;
                    case GameSpeedConstants.PlaySpeed:
                        ChangeSpeed(GameSpeedConstants.PauseSpeed);
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Unexpected key: " + key);
            }

            CheckTimeChangeAnalytics();
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void ChangeSpeed(Image origin)
    {
        try
        {
            Debug.Log("ChangeSpeed called by: " + origin.name);

            if (!pausePanel.activeSelf)
            {
                if (origin.Equals(pauseItem))
                {
                    ChangeSpeed(GameSpeedConstants.PauseSpeed);
                }
                else if (origin.Equals(playItem))
                {
                    ChangeSpeed(GameSpeedConstants.PlaySpeed);
                }
                else if (origin.Equals(mediumSpeedItem))
                {
                    ChangeSpeed(GameSpeedConstants.MediumSpeed);
                }
                else if (origin.Equals(fatestSpeedItem))
                {
                    ChangeSpeed(GameSpeedConstants.FatestSpeed);
                }

                CheckTimeChangeAnalytics();
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            LogManager.SendException(exceptionSender, ex);
        }
    }

    public void ChangeSpeed(float newSpeed)
    {
        Time.timeScale = newSpeed;

        switch (newSpeed)
        {
            case GameSpeedConstants.PauseSpeed:
                ChangeButtonColors(pauseItem, Color.white);
                ChangeButtonColors(playItem, Color.black);
                ChangeButtonColors(mediumSpeedItem, Color.black);
                ChangeButtonColors(fatestSpeedItem, Color.black);
                break;
            case GameSpeedConstants.PlaySpeed:
                ChangeButtonColors(pauseItem, Color.black);
                ChangeButtonColors(playItem, Color.white);
                ChangeButtonColors(mediumSpeedItem, Color.black);
                ChangeButtonColors(fatestSpeedItem, Color.black);
                break;
            case GameSpeedConstants.MediumSpeed:
                ChangeButtonColors(pauseItem, Color.black);
                ChangeButtonColors(playItem, Color.black);
                ChangeButtonColors(mediumSpeedItem, Color.white);
                ChangeButtonColors(fatestSpeedItem, Color.black);
                break;
            case GameSpeedConstants.FatestSpeed:
                ChangeButtonColors(pauseItem, Color.black);
                ChangeButtonColors(playItem, Color.black);
                ChangeButtonColors(mediumSpeedItem, Color.black);
                ChangeButtonColors(fatestSpeedItem, Color.white);
                break;
        }
    }

    private void ChangeButtonColors(Image button, Color color)
    {
        button.color = color;
    }

    private void CheckTimeChangeAnalytics()
    {
        try
        {
            if (Time.timeScale != LastGameSpeed)
            {
                LastGameSpeed = Time.timeScale;
                LogManager.LogAnalytic(analyticSender, "TimeManagement", $"New game speed: {Time.timeScale}");
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Actualiza el estado de la animación de parpadeo de la unidad seleccionada.
    /// 
    /// Lógica para multiplayer y singleplayer.
    /// </summary>
    public void UpdateUnitAnimation()
    {
        try
        {
            if (selection.HaveObjectSelected)
            {
                List<GameObject> destroyedObjects = new List<GameObject>();

                foreach (GameObject selectedTroopObject in selection.SelectionObjects)
                {
                    try
                    {
                        IObjectAnimator selectedTroop = selectedTroopObject.GetComponent<IObjectAnimator>();

                        if (selectedTroop != null)
                        {
                            selectedTroop.Animate();
                        }
                    }
                    catch (MissingReferenceException)
                    {
                        destroyedObjects.Add(selectedTroopObject);
                    }
                }

                selection.SelectionObjects.RemoveAll(item => destroyedObjects.Contains(item));
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void LeftClickReceivedFromTroop(TroopController newSelection)
    {
        try
        {
            if (selection.HaveObjectSelected)
            {
                if (!selection.SelectionObjects.Contains(newSelection.gameObject))
                {
                    EndSelection();
                    SetTroopSelected(newSelection, false);
                }
            }
            else
            {
                SetTroopSelected(newSelection, false);
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void RightClickReceivedFromTroop(TroopController newSelection)
    {
        try
        {
            if (selection.HaveObjectSelected)
            {
                if (!selection.SelectionObjects.Contains(newSelection.gameObject))
                {
                    foreach (GameObject selectedTroopObject in selection.SelectionObjects)
                    {
                        TroopController selectedTroop = selectedTroopObject.GetComponent<TroopController>();

                        Debug.Log("New target for troop: " + newSelection);
                        selectedTroop.troopModel.SetTarget(newSelection.gameObject, this);
                    }

                    EndSelection();
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void ClickReceivedFromMap(KeyCode mouseKeyPressed)
    {
        try
        {
            switch (mouseKeyPressed)
            {
                case KeyCode.Mouse0: // Left mouse button
                    EndSelection();
                    selection.StartMultiselect(cameraController.ScreenToWorldPoint(), typeof(TroopController));
                    targetMarkerController.RemoveTargetPosition();
                    Debug.Log($"MultiselectOrigin assignated {selection.MultiselectOrigin}");
                    break;
                case KeyCode.Mouse1: // Right mouse button
                    MoveSelectedTroops();
                    break;
                default:
                    string log = $"Unexpected map click {mouseKeyPressed}";
                    LogManager.SendLog(logSender, log);
                    Debug.LogWarning(log);
                    break;
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    private void SetTroopSelected(TroopController newSelection, bool isMultiselect)
    {
        try
        {
            if (newSelection.troopModel.Player == thisPcPlayer)
            {
                selection.SetObjectSelected(newSelection, isMultiselect, typeof(TroopController), thisPcPlayer.MapSocketId);
                targetMarkerController.SetTargetPosition(newSelection.troopModel.Target, false);
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    private void MoveSelectedTroops()
    {
        try
        {
            if (selection.HaveObjectSelected && selection.SelectionType == typeof(TroopController))
            {
                Vector3 mouseClickPosition = cameraController.ScreenToWorldPoint();
                Debug.Log("New target position for troop: " + mouseClickPosition);

                foreach (GameObject selectedTroopObject in selection.SelectionObjects)
                {
                    TroopController selectedTroop = selectedTroopObject.GetComponent<TroopController>();

                    selectedTroop.troopModel.SetTarget(new GameObject(GlobalConstants.EmptyTargetName), this);
                    selectedTroop.troopModel.Target.transform.position = mouseClickPosition;
                    selectedTroop.troopModel.Target.transform.parent = emptyTargetsHolder.transform;
                    targetMarkerController.SetTargetPosition(mouseClickPosition, true);
                }

                EndSelection();
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void CleanTroopSelection(TroopModel selectedTroop)
    {
        try
        {
            if (selectedTroop.Target != null
                && selectedTroop.Target.name.Contains(GlobalConstants.EmptyTargetName))
            {
                DestroyUnit(selectedTroop.Target, null);
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    public void DestroyUnit(string troopName)
    {
        Transform troop = troopsCanvas.transform.Find(troopName);

        if (troop == null)
        {
            Debug.LogWarning("Troop not finded: " + troopName);
        }
        else
        {
            DestroyUnit(troop.gameObject, null);
        }
    }

    public void DestroyUnit(GameObject unitToDestroy, TroopController destroyer)
    {
        TroopController troopController;

        try
        {
            troopController = unitToDestroy.GetComponent<TroopController>();

            if (troopController != null)
            {
                CleanTroopSelection(troopController.troopModel);
                troopController.DestroyTroopActions(IsMultiplayerHost);
            }

            if (destroyer != null)
            {
                statisticsController.ReportArmyDefeated(troopController, destroyer);
            }

            if (IsMultiplayerHost)
            {
                TroopDeadSignalR.Instance.SendTroopDead(gameModel.GameKey, unitToDestroy.transform.name);
            }

            Destroy(unitToDestroy);
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }

    private void EndSelection()
    {
        try
        {
            selection.EndSelection();
            targetMarkerController.RemoveTargetPosition();
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
            Debug.LogException(ex);
        }
    }
}