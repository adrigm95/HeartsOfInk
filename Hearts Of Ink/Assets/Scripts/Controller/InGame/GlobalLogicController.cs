using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.Data.Literals;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneChangeController;

public class GlobalLogicController : MonoBehaviour
{
    /// <summary>
    /// Contador que se utiliza para que las unidades clonadas no tengan el mismo nombre.
    /// </summary>
    [NonSerialized]
    public int troopsCounter;

    [NonSerialized]
    public GameModel gameModel;

    public List<CityController> cities;
    public List<AILogic> aiLogics;
    public float GameSpeed;
    public SelectionModel selection;
    public UnitAnimation unitAnimation;
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
    public GameObject pauseItem;
    public bool Pause { get; set; }

    private void Start()
    {
        aiLogics = new List<AILogic>();
        selection = new SelectionModel();
        cameraController = FindObjectOfType<CameraController>();
        statisticsController = FindObjectOfType<StatisticsController>();
        sceneChangeController = FindObjectOfType<SceneChangeController>();
        gameOptionsHolder = FindObjectOfType<GameOptionsController>();

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

        troopsCounter = 0;

        if (gameModel.Gametype == GameModel.GameType.Single)
        {
            tutorialController.DisplayTutorial();
        }
        else
        {
            SetPauseState(true, null);
            waitingPanel.Show(this);
            StartGameSignalR.Instance.SendClientReady(gameModel.gameKey);
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
        TimeManagement();
        UpdateUnitAnimation();
        CheckVictoryConditions();
    }

    private GameModel GetMockedGameModel()
    {
        GameModel gameModel = new GameModel(0);
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
                    break;
            }
        }
    }

    private void AwakeMap()
    {
        MapModel mapModel = MapDAC.LoadMapInfo(gameModel.MapId);
        MapController.Instance.UpdateMap(mapModel.SpritePath);
        CleanTransform(citiesCanvas.transform);
        CleanTransform(troopsCanvas.transform);

        foreach (MapCityModel city in mapModel.Cities)
        {
            Player cityOwner = gameModel.Players.First(item => item.MapSocketId == city.MapSocketId);
            InstantiateCity(city, cityOwner);
        }

        foreach (MapTroopModel troop in mapModel.Troops)
        {
            Player troopOwner = gameModel.Players.First(item => item.MapSocketId == troop.MapSocketId);
            InstantiateTroop(troop.Units, VectorUtils.FloatVectorToVector3(troop.Position), troopOwner);
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
        newObject.name = cityModel.Name;
        newObject.Owner = cityOwner;
    }

    public void InstantiateTroop(int units, Vector3 position, Player troopOwner)
    {
        TroopController newObject;

        if (troopOwner.Faction.Bonus.BonusId == Bonus.Id.Recruitment)
        {
            units += 10;
        }

        newObject = ((GameObject) Instantiate(
            Resources.Load("Prefabs/Troop"), 
            position, 
            troopsCanvas.transform.rotation, 
            troopsCanvas.transform)
            ).GetComponent<TroopController>();
        newObject.name += troopsCounter;
        newObject.troopModel = new TroopModel(troopOwner);
        newObject.troopModel.Units = units;

        troopsCounter++;
    }

    private void TimeManagement()
    {
        if (Pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void CheckVictoryConditions()
    {
        bool isGameFinished = true;
        Player firstOwner = cities[0].Owner;
        byte firstOwnerAlliance = cities[0].Owner.Alliance;

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

    public void InputManagement()
    {
        float horizontalAxis;
        float verticalAxis;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPauseState(!Pause, KeyCode.Escape);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SetPauseState(!Pause, KeyCode.E);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndSelection();
        }

        horizontalAxis = Input.GetAxis(AxisData.HorizontalAxis);
        verticalAxis = Input.GetAxis(AxisData.VerticalAxis);

        if (horizontalAxis == 0 && verticalAxis == 0)
        {
            // TODO: Check on screen axis.
        }
        else
        {
            cameraController.InputCameraMovement(horizontalAxis, verticalAxis);
        }
    }

    public void SetPauseState(bool pauseState, KeyCode? initializer)
    {
        Pause = pauseState;
        pauseItem.SetActive(Pause);

        switch (initializer)
        {
            case KeyCode.Escape:
                pausePanel.SetActive(Pause);
                break;
            case KeyCode.E:
            case null:
                // No hay que hacer nada en estos caso.
                break;
            default:
                Debug.LogWarning("Unexpected pause estate initializer: " + initializer);
                break;
        }
    }

    /// <summary>
    /// Actualiza el estado de la animación de parpadeo de la unidad seleccionada.
    /// </summary>
    public void UpdateUnitAnimation()
    {
        if (selection.HaveObjectSelected && unitAnimation != null)
        {
            TroopController selectedTroop = selection.SelectionObject.GetComponent<TroopController>();
            selectedTroop.AnimateText(unitAnimation.IterateAnimation(Time.time));
        }
    }

    public void ClickReceivedFromTroop(TroopController newSelection)
    {
        if (!selection.HaveObjectSelected)
        {
            if (newSelection.troopModel.Player == thisPcPlayer)
            {
                selection.ChangeSelection(newSelection.gameObject, typeof(TroopController));
                Debug.Log("TroopSelected: " + newSelection);
                unitAnimation = new UnitAnimation(Time.time);
            }
        }
        else
        {
            if (selection.SelectionObject != newSelection.gameObject)
            {
                TroopController selectedTroop = selection.SelectionObject.GetComponent<TroopController>();

                Debug.Log("New target for troop: " + newSelection);
                selectedTroop.troopModel.SetTarget(newSelection.gameObject, this);

                EndSelection();
            }
        }
    }

    public void ClickReceivedFromCity(CityController newSelection)
    {
        if (selection.HaveObjectSelected)
        {
            if (selection.SelectionType == typeof(TroopController))
            {
                TroopController selectedTroop = selection.SelectionObject.GetComponent<TroopController>();

                Debug.Log("New target for troop: " + newSelection);
                selectedTroop.troopModel.SetTarget(newSelection.gameObject, this);
            }

            EndSelection();
        }
    }

    public void ClickReceivedFromMap()
    {
        if (selection.HaveObjectSelected)
        {
            if (selection.SelectionType == typeof(TroopController))
            {
                Vector3 mouseClickPosition = ScreenToWorldPoint();
                TroopController selectedTroop = selection.SelectionObject.GetComponent<TroopController>();

                Debug.Log("New target position for troop: " + mouseClickPosition);
                selectedTroop.troopModel.SetTarget(new GameObject(GlobalConstants.EmptyTargetName), this);
                selectedTroop.troopModel.Target.transform.position = mouseClickPosition;
                selectedTroop.troopModel.Target.transform.parent = emptyTargetsHolder.transform;
                EndSelection();
            }
        }
        else
        {
            Debug.Log("Map click received.");
        }
    }

    /// <summary>
    /// Método utilizado para obtener la posición en pantalla de un click.
    /// 
    /// TODO: Mover a CameraUtils y comprobar si sigue funcionando.
    /// </summary>
    /// <returns></returns>
    private Vector3 ScreenToWorldPoint()
    {
        Vector3 cameraPosition = cameraController.transform.position;
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraPosition.z);

        position = Camera.main.ScreenToViewportPoint(position);
        position = Camera.main.ViewportToWorldPoint(position);
        position *= -1f;
        position.x += cameraPosition.x * 2;
        position.y += cameraPosition.y * 2;

        return position;
    }

    public void CleanTroopSelection(TroopModel selectedTroop)
    {
        if (selectedTroop.Target != null 
            && selectedTroop.Target.name.Contains(GlobalConstants.EmptyTargetName))
        {
            DestroyUnit(selectedTroop.Target, null);
        }
    }

    public void DestroyUnit(GameObject unitToDestroy, TroopController destroyer)
    {
        TroopController troopController = unitToDestroy.GetComponent<TroopController>();

        if (troopController != null)
        {
            CleanTroopSelection(troopController.troopModel);
        }

        if (destroyer != null)
        {
            statisticsController.ReportArmyDefeated(troopController, destroyer);
        }
        
        Destroy(unitToDestroy);
    }

    private void EndSelection()
    {
        if (selection.SelectionType == typeof(TroopController))
        {
            TroopController selectedTroop = selection.SelectionObject.GetComponent<TroopController>();
            selectedTroop.AnimateText(1f);
        }

        selection.SetAsNull();
        unitAnimation = null;
    }

    private void CleanTransform(Transform cleanTransform)
    {
        foreach (Transform child in cleanTransform)
        {
            child.parent = null;
            // Esto no funciona.
            //Destroy(child);

            // Esto si.
            Destroy(child.gameObject);
        }

        Debug.Log($"Cleaned transform: {cleanTransform.name}");
    }
}