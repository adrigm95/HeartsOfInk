using Assets.Scripts.Controller.InGame;
using Assets.Scripts.Data;
using Assets.Scripts.Data.EditorModels;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Logic;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorPanelController : MonoBehaviour
{
    private const string PrefabCity = "Prefabs/EditorCityPrefab";
    private const string PrefabCapital = "Prefabs/EditorCapital";
    private const string PrefabTroop = "Prefabs/EditorTroop";
    private MapModelHeader mapModelHeader;
    private MapModel mapModel;
    private GlobalInfo globalInfo;
    private List<Dropdown> factions;
    private List<MapModelHeader> availableMaps;
    private CameraController cameraController;
    private SelectionModel selection;
    public Transform citiesHolder;
    public Transform troopsHolder;
    public Dropdown cbMaps;
    public InputField mapName;
    public Toggle isForMultiplayer;
    public Toggle isForSingleplayer;
    public Toggle citiesEnabled;
    public Toggle troopsEnabled;
    public int startFactionLines;
    public int spacing;

    public bool AddingCities
    {
        get { return citiesEnabled.isOn; }
    }

    public bool AddingTroops
    {
        get { return troopsEnabled.isOn; }
    }

    void Start()
    {
        factions = new List<Dropdown>();
        selection = new SelectionModel();
        cameraController = FindObjectOfType<CameraController>();
        LoadAvailableMaps();
        cbMaps.onValueChanged.AddListener(delegate { LoadMap(); });
        LoadMap();
    }

    void Update()
    {
        UpdateUnitAnimation();
    }

    public void LoadAvailableMaps()
    {
        cbMaps.options.Clear();
        availableMaps = MapDAC.GetAvailableMaps();
        availableMaps.ForEach(map => cbMaps.options.Add(new Dropdown.OptionData(map.DisplayName)));
        cbMaps.RefreshShownValue();
    }

    public void LoadMap()
    {
        Debug.Log("Loading map: " + cbMaps.itemText.text);
        mapModelHeader = availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text);
        mapModel = MapDAC.LoadMapInfo(mapModelHeader.DefinitionName);
        globalInfo = MapDAC.LoadGlobalMapInfo();

        mapName.text = mapModel.DisplayName;
        isForMultiplayer.isOn = mapModel.AvailableForMultiplayer;
        isForSingleplayer.isOn = mapModel.AvailableForSingleplayer;
        MapController.Instance.UpdateMap(mapModel.SpritePath);
        UpdateCanvas();
    }

    public void AddNewFactionLine()
    {
        byte maxSocketId = 0;
        MapPlayerModel playerModel;

        mapModel.Players.ForEach(player => maxSocketId = player.MapSocketId > maxSocketId ? player.MapSocketId : maxSocketId);
        maxSocketId++;
        playerModel = new MapPlayerModel(maxSocketId);
        mapModel.Players.Add(playerModel);
        LoadFactionLine(playerModel);
    }

    public void OnClick_PlayerColor(Image colorImage)
    {
        Debug.Log($"Color changed for image {colorImage.name}; color: {colorImage.color}");
        colorImage.color = ColorUtils.NextColor(colorImage.color, globalInfo.AvailableColors);
    }

    public void ClickReceivedFromMap(KeyCode mouseKeyPressed)
    {
        Debug.Log("ClickReceivedFromMap");

        if (citiesEnabled.isOn)
        {
            CreateNewCity();
        }
        else if (troopsEnabled.isOn)
        {
            CreateNewTroop();
        }
    }

    public void ClickReceivedFromCity(EditorCityController city)
    {
        if (!citiesEnabled.isOn && !troopsEnabled.isOn)
        {
            EndSelection();
            selection.ChangeSelection(city.gameObject, typeof(EditorCityController));
        }
    }

    public void ClickReceivedFromTroop(EditorTroopController troop)
    {
        if (!troopsEnabled.isOn && !citiesEnabled.isOn)
        {
            EndSelection();
            selection.ChangeSelection(troop.gameObject, typeof(EditorCityController));
        }
    }

    /// <summary>
    /// Actualiza el estado de la animación de parpadeo de la unidad seleccionada.
    /// </summary>
    public void UpdateUnitAnimation()
    {
        if (selection.HaveObjectSelected)
        {
            foreach (GameObject selectedObject in selection.SelectionObjects)
            {
                IObjectAnimator selectedAnimator = selectedObject.GetComponent<IObjectAnimator>();
                
                selectedAnimator?.Animate();
            }
        }
    }

    public void ShowMapInfoPanel_OnClick()
    {
        gameObject.SetActive(!isActiveAndEnabled);
    }

    public void ChangeOption_OnClick(GameObject sender)
    {
        
    }

    private void EndSelection()
    {
        if (selection.HaveObjectSelected)
        {
            foreach (GameObject selectedTroopObject in selection.SelectionObjects)
            {
                IObjectAnimator selectedTroop = selectedTroopObject.GetComponent<IObjectAnimator>();
                if (selectedTroop != null)
                {
                    selectedTroop.EndAnimation();
                }
            }

            selection.SetAsNull();
        }
    }

    private void CreateNewCity()
    {
        EditorCityController newObject;
        SpriteRenderer spriteRenderer;
        MapPlayerModel player;
        Vector3 mouseClickPosition;

        mouseClickPosition = cameraController.ScreenToWorldPoint();
        Debug.Log("Adding new city for position: " + mouseClickPosition);
        newObject = ((GameObject)Instantiate(
                            Resources.Load(PrefabCity),
                            mouseClickPosition,
                            citiesHolder.rotation,
                            citiesHolder)
                            ).GetComponent<EditorCityController>();

        spriteRenderer = newObject.GetComponent<SpriteRenderer>();
        player = mapModel.Players[0];
        newObject.name = "New City";
        newObject.isCapital = false;
        newObject.ownerSocketId = player.MapSocketId;
        newObject.panelController = this;
        spriteRenderer.color = ColorUtils.GetColorByString(player.Color);
    }

    private void CreateNewTroop()
    {
        EditorTroopController newObject;
        TextMeshProUGUI unitsText;
        MapPlayerModel player;
        Vector3 mouseClickPosition;

        mouseClickPosition = cameraController.ScreenToWorldPoint();
        Debug.Log("Adding new troop for position: " + mouseClickPosition);
        newObject = ((GameObject)Instantiate(
                            Resources.Load(PrefabTroop),
                            mouseClickPosition,
                            troopsHolder.rotation,
                            troopsHolder)
                            ).GetComponent<EditorTroopController>();

        unitsText = newObject.GetComponent<TextMeshProUGUI>();
        player = mapModel.Players[0];
        newObject.name = "Troop";
        newObject.ownerSocketId = player.MapSocketId;
        newObject.panelController = this;
        unitsText.text = Convert.ToString(GlobalConstants.DefaultCompanySize);
        unitsText.color = ColorUtils.GetColorByString(player.Color);
    }

    private void LoadFactionLines()
    {
        CleanFactionLines();
        foreach (MapPlayerModel player in mapModel.Players)
        {
            LoadFactionLine(player);
        }
    }

    private void LoadFactionLine(MapPlayerModel player)
    {
        string prefabPath = "Prefabs/editorFactionLine";
        Transform newObject;
        Vector3 position;
        GlobalInfoFaction faction;
        Dropdown cbFaction;
        Dropdown cbPlayerType;
        Image colorFactionImage;
        Button btnColorFaction;
        Toggle tgIsPlayable;

        faction = globalInfo.Factions.Find(item => item.Id == player.FactionId);
        position = new Vector3(0, startFactionLines);
        position.y -= spacing * factions.Count;
        newObject = ((GameObject)Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine_" + player.MapSocketId;
        newObject.SetParent(this.transform, false);

        cbFaction = newObject.Find("cbFaction").GetComponent<Dropdown>();
        cbPlayerType = newObject.Find("cbPlayerType").GetComponent<Dropdown>();
        colorFactionImage = newObject.Find("btnColorFaction").GetComponent<Image>();
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Button>();
        tgIsPlayable = newObject.Find("tgIsPlayable").GetComponent<Toggle>();

        btnColorFaction.onClick.AddListener(delegate { OnClick_PlayerColor(colorFactionImage); });
        cbPlayerType.value = player.IaId;
        colorFactionImage.color = ColorUtils.GetColorByString(player.Color);
        LoadFactionsCombo(cbFaction, player.FactionId);
        tgIsPlayable.isOn = player.IsPlayable;

        factions.Add(cbFaction);
    }

    public void SaveMap()
    {
        UpdateModel();
        MapDAC.SaveMapDefinition(mapModel);
        MapDAC.SaveMapHeader(mapModelHeader);
    }

    private void UpdateCanvas()
    {
        SetCitiesInCanvas();
        SetTroopsInCanvas();
        LoadFactionLines();
    }

    private void SetCitiesInCanvas()
    {
        foreach (MapCityModel mapCityModel in mapModel.Cities)
        {
            EditorCityController newObject;
            MapPlayerModel player;
            SpriteRenderer spriteRenderer;
            Vector3 position = VectorUtils.FloatVectorToVector3(mapCityModel.Position);
            string resourceName = mapCityModel.Type == 1 ? PrefabCity : PrefabCapital;

            newObject = ((GameObject)Instantiate(
                            Resources.Load(resourceName),
                            position,
                            citiesHolder.rotation,
                            citiesHolder)
                            ).GetComponent<EditorCityController>();

            spriteRenderer = newObject.GetComponent<SpriteRenderer>();
            newObject.name = mapCityModel.Name;
            newObject.isCapital = !Convert.ToBoolean(mapCityModel.Type);
            newObject.ownerSocketId = mapCityModel.MapSocketId;
            newObject.panelController = this;
            player = mapModel.Players.Find(p => p.MapSocketId == mapCityModel.MapSocketId);
            spriteRenderer.color = ColorUtils.GetColorByString(player.Color);
        }
    }

    private void SetTroopsInCanvas()
    {
        //TODO
    }

    private void UpdateModel()
    {
        SetMapInfoOnModel();
        SaveFactionsInModel();
        SaveCitiesInModel();
        SaveTroopsInModel();
    }

    private void LoadFactionsCombo(Dropdown factionsCombo, int factionId)
    {
        List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();

        foreach (GlobalInfoFaction factionInfo in globalInfo.Factions)
        {
            dropdownOptions.Add(new Dropdown.OptionData()
            {
                text = factionInfo.NameLiteral
            });
        }

        factionsCombo.AddOptions(dropdownOptions);
        factionsCombo.RefreshShownValue();
        factionsCombo.value = factionId;
    }

    private void SetMapInfoOnModel()
    {
        string image = cbMaps.options[cbMaps.value].text;

        mapModel.DisplayName = mapName.text;
        mapModel.AvailableForMultiplayer = isForMultiplayer.isOn;
        mapModel.AvailableForSingleplayer = isForSingleplayer.isOn;

        mapModelHeader.DisplayName = mapName.text;
        mapModelHeader.AvailableForMultiplayer = isForMultiplayer.isOn;
        mapModelHeader.AvailableForSingleplayer = isForSingleplayer.isOn;
    }

    private void SaveFactionsInModel()
    {
        Transform lineObject;
        MapPlayerModel playerModel;
        Dropdown cbPlayerType;
        Image btnColorFaction;
        Toggle tgIsPlayable;
        Text txtAlliance;

        foreach (Dropdown cbFaction in factions)
        {
            byte mapSocketId;

            lineObject = cbFaction.transform.parent;
            mapSocketId = Convert.ToByte(lineObject.name.Split('_')[1]);
            playerModel = mapModel.Players.Find(item => item.MapSocketId == mapSocketId);

            if (playerModel == null)
            {
                playerModel = new MapPlayerModel(mapSocketId);
                mapModel.Players.Add(playerModel);
            }
            
            cbPlayerType = lineObject.Find("cbPlayerType").GetComponent<Dropdown>();
            btnColorFaction = lineObject.Find("btnColorFaction").GetComponent<Image>();
            tgIsPlayable = lineObject.Find("tgIsPlayable").GetComponent<Toggle>();
            txtAlliance = lineObject.Find("btnAlliance").GetComponentInChildren<Text>();

            playerModel.IaId = cbPlayerType.value;
            playerModel.FactionId = globalInfo.Factions.Find(item =>
                    item.NameLiteral == cbFaction.options[cbFaction.value].text).Id;
            playerModel.Alliance = Convert.ToInt32(txtAlliance);
            playerModel.IsPlayable = tgIsPlayable.isOn;
            playerModel.Color = ColorUtils.GetStringByColor(btnColorFaction.color);
        }
    }

    private void SaveCitiesInModel()
    {
        mapModel.Cities = new List<MapCityModel>();

        foreach (Transform city in citiesHolder)
        {
            EditorCityController editorCity = city.GetComponent<EditorCityController>();

            mapModel.Cities.Add(new MapCityModel()
            {
                MapSocketId = Convert.ToByte(editorCity.ownerSocketId),
                Name = editorCity.name,
                Position = new float[] { editorCity.transform.position.x, editorCity.transform.position.y },
                Type = editorCity.isCapital ? 0 : 1
            });
        }
    }

    private void SaveTroopsInModel()
    {
        TextMeshProUGUI unitsText;
        mapModel.Troops = new List<MapTroopModel>();

        foreach (Transform troop in troopsHolder)
        {
            EditorTroopController editorCity = troop.GetComponent<EditorTroopController>();
            unitsText = troop.GetComponent<TextMeshProUGUI>();

            mapModel.Troops.Add(new MapTroopModel()
            {
                MapSocketId = Convert.ToByte(editorCity.ownerSocketId),
                Position = new float[] { editorCity.transform.position.x, editorCity.transform.position.y },
                Units = Convert.ToInt32(unitsText.text)
            });
        }
    }

    private void CleanFactionLines()
    {
        foreach (Dropdown cbFaction in factions)
        {
            Destroy(cbFaction.transform.parent.gameObject);
        }

        factions.Clear();
    }
}