using Assets.Scripts.Data;
using Assets.Scripts.Data.Literals;
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
    public int unitsCounter;

    public List<CityController> cities;
    public List<AILogic> aiLogics;
    public float GameSpeed;
    public SelectionModel selection;
    public UnitAnimation unitAnimation;
    public Faction playerFaction;
    public CameraController cameraController;
    public StatisticsController statisticsController;
    public SceneChangeController sceneChangeController;
    public GameObject pausePanel;
    public GameObject emptyTargetsHolder;
    public bool Pause { get; set; }

    private void Awake()
    {
        string gametype;

        aiLogics = new List<AILogic>();
        selection = new SelectionModel();
        cities = FindObjectsOfType<CityController>().ToList();
        cameraController = FindObjectOfType<CameraController>();
        statisticsController = FindObjectOfType<StatisticsController>();
        sceneChangeController = FindObjectOfType<SceneChangeController>();

        gametype = PlayerPrefs.GetString(PlayerPrefsData.GametypeKey, PlayerPrefsData.GametypeDefault);
        AwakeFactionsManagement(gametype);

        unitsCounter = 0;
        SetPauseState(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
        TimeManagement();
        UpdateUnitAnimation();
        CheckVictoryConditions();
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
        Faction.Id firstOwner = cities[0].Owner;

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

    private void AwakeFactionsManagement(string gametype)
    {
        playerFaction = new Faction();

        switch (gametype)
        {
            case PlayerPrefsData.GametypeDefault:
                aiLogics.Add(new AILogic(Faction.Id.NOMADS, this));
                aiLogics.Add(new AILogic(Faction.Id.VUKIS, this));
                aiLogics.Add(new AILogic(Faction.Id.GOVERNMENT, this));

                playerFaction.FactionId = Faction.Id.REBELS;
                break;
            case PlayerPrefsData.GametypeSingle:
                AwakeFactionManagement(Faction.Id.GOVERNMENT);
                AwakeFactionManagement(Faction.Id.REBELS);
                AwakeFactionManagement(Faction.Id.VUKIS);
                AwakeFactionManagement(Faction.Id.NOMADS);
                break;
        }
    }

    private void AwakeFactionManagement(Faction.Id factionId)
    {
        string factionIdStr = Convert.ToString((int) factionId);
        Faction.IA managementKey = (Faction.IA) PlayerPrefs.GetInt(factionIdStr, 1);

        switch (managementKey)
        {
            case Faction.IA.PLAYER:
                playerFaction.FactionId = factionId;
                PlayerPrefs.SetInt(PlayerPrefsData.PlayerFactionIdKey, Convert.ToInt32(factionId));
                break;
            case Faction.IA.IA:
                aiLogics.Add(new AILogic(factionId, this));
                break;
        }
    }

    public void InputManagement()
    {
        float horizontalAxis;
        float verticalAxis;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPauseState(!Pause);
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

    public void SetPauseState(bool pauseState)
    {
        Pause = pauseState;
        pausePanel.SetActive(Pause);
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
            if (newSelection.troopModel.FactionId == playerFaction.FactionId)
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

    public void ClickReceivedFromMap(MapController newSelection)
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
}
