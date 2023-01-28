﻿using Assets.Scripts.Controller.InGame;
using Assets.Scripts.Data;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Logic;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SceneChangeController;

public class GlobalLogicController : MonoBehaviour
{
    private SelectionModel selection;

    /// <summary>
    /// Contador que se utiliza para que las unidades clonadas no tengan el mismo nombre.
    /// </summary>
    [NonSerialized]
    public int troopsCounter;

    [NonSerialized]
    public GameModel gameModel;

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
            //waitingPanel.Show(this);
            IngameHOIHub.Instance.SuscribeToRoom(gameModel.GameKey, thisPcPlayer.Name);
            StartGameIngameSignalR.Instance.SendClientReady(gameModel.GameKey);
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
        TimeManagement();
        UpdateUnitAnimation();
        CheckVictoryConditions();
        UpdateMultiselect();
    }

    private void UpdateMultiselect()
    {
        if (selection.UpdateMultiselect(cameraController.ScreenToWorldPoint(), troopsCanvas.transform, thisPcPlayer.MapSocketId))
        {
            UpdateTargetMarker();
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
        }
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
            mapModel = MapDAC.LoadMapInfo(gameModel.MapId);
            Debug.Log($"Map path: {mapModel.SpritePath}");
            MapController.Instance.UpdateMap(mapModel.SpritePath);

            for (index = 0; index < mapModel.Cities.Count; index++)
            {
                city = mapModel.Cities[index];
                Player cityOwner = gameModel.Players.First(item => item.MapSocketId == city.MapSocketId);
                InstantiateCity(city, cityOwner);
            }

            for (index = 0; index < mapModel.Troops.Count; index++)
            {
                troop = mapModel.Troops[index];
                Player troopOwner = gameModel.Players.First(item => item.MapSocketId == troop.MapSocketId);
                InstantiateTroop(troop.Units, VectorUtils.FloatVectorToVector3(troop.Position), troopOwner);
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

        if (selection.MultiselectOrigin != null)
        {
            if (Input.GetMouseButtonUp(KeyConstants.LeftClick))
            {
                selection.MultiselectOrigin = null;
            }
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

    public void LeftClickReceivedFromTroop(TroopController newSelection)
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

    public void RightClickReceivedFromTroop(TroopController newSelection)
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

    public void ClickReceivedFromMap(KeyCode mouseKeyPressed)
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
                Debug.LogWarning($"Unexpected map click {mouseKeyPressed}");
                break;
        }
    }

    private void SetTroopSelected(TroopController newSelection, bool isMultiselect)
    {
        if (newSelection.troopModel.Player == thisPcPlayer)
        {
            selection.SetObjectSelected(newSelection, isMultiselect, typeof(TroopController), thisPcPlayer.MapSocketId);
            targetMarkerController.SetTargetPosition(newSelection.troopModel.Target, false);
        }
    }

    private void MoveSelectedTroops()
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
        selection.EndSelection();
        targetMarkerController.RemoveTargetPosition();
    }
}