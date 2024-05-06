using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorPanelController : MonoBehaviour
{
    private MapModelHeader mapModelHeader;
    private MapModel mapModel;
    private GlobalInfo globalInfo;
    private List<Dropdown> factions;
    private List<MapModelHeader> availableMaps;
    public MapEditorLogicController _mapEditorLogic;
    public Transform citiesHolder;
    public Transform troopsHolder;
    public GameObject uploadChoise;
    public Dropdown cbMaps;
    public Dropdown cbMapImages;
    public InputField mapName;
    public Toggle isForMultiplayer;
    public Toggle isForSingleplayer;
    public int startFactionLines;
    public int spacing;

    public MapModel MapModel { get { return mapModel; } }

    void Start()
    {
        factions = new List<Dropdown>();
        LoadAvailableMaps(string.Empty);
        cbMaps.onValueChanged.AddListener(delegate { LoadMap(); });
        LoadMap();
    }

    void Update()
    {
        
    }

    public void LoadAvailableMaps(string firstMap)
    {
        cbMaps.options.Clear();
        availableMaps = MapDAC.GetAvailableMaps(GlobalConstants.RootPath);
        availableMaps.ForEach(map => cbMaps.options.Add(new Dropdown.OptionData(map.DisplayName)));
        cbMaps.RefreshShownValue();

        if (!string.IsNullOrWhiteSpace(firstMap))
        {
            cbMaps.value = cbMaps.options.Select(option => option.text).ToList().IndexOf(firstMap);
        }
    }

    public void LoadMap()
    {
        Debug.Log("Loading map: " + cbMaps.itemText.text);
        _mapEditorLogic.ResetSelection();
        mapModelHeader = availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text);
        mapModel = MapDAC.LoadMapInfoByName(mapModelHeader.DefinitionName, GlobalConstants.RootPath);
        ValidateAndCorrectMap(mapModel);
        globalInfo = GlobalInfoDAC.LoadGlobalMapInfo();

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
        Color startColor = ColorUtils.NextColor(Color.black, globalInfo.AvailableColors);

        mapModel.Players.ForEach(player => maxSocketId = player.MapSocketId > maxSocketId ? player.MapSocketId : maxSocketId);

        // El primer registro necesitamos que su mapSocketId sea 0 para no romper la lógica del panel de puntuaciones.
        if (maxSocketId != 0)
        {
            maxSocketId++;
        }
        
        playerModel = new MapPlayerModel(maxSocketId);
        playerModel.Color = ColorUtils.GetStringByColor(startColor);
        mapModel.Players.Add(playerModel);
        LoadFactionLine(playerModel);
        LoadFactionLines();
    }

    public void OnClick_PlayerColor(Image colorImage)
    {
        Debug.Log($"Color changed for image {colorImage.name}; color: {colorImage.color}");
        colorImage.color = ColorUtils.NextColor(colorImage.color, globalInfo.AvailableColors);
        SaveFactionsInModel();
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
        Text txtAlliance;

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
        txtAlliance = newObject.Find("btnAlliance").GetComponentInChildren<Text>();
        tgIsPlayable = newObject.Find("tgIsPlayable").GetComponent<Toggle>();

        btnColorFaction.onClick.AddListener(delegate { OnClick_PlayerColor(colorFactionImage); });
        cbPlayerType.value = player.IaId;
        colorFactionImage.color = ColorUtils.GetColorByString(player.Color);
        LoadFactionsCombo(cbFaction, player.FactionId);
        txtAlliance.text = AllianceUtils.ConvertToString(player.Alliance);
        tgIsPlayable.isOn = player.IsPlayable;

        factions.Add(cbFaction);
    }

    public void SaveMap()
    {
        UpdateModel();
        MapDAC.SaveMapDefinition(mapModel, GlobalConstants.RootPath);
        MapDAC.SaveMapHeader(mapModelHeader, GlobalConstants.RootPath);
        LoadAvailableMaps(mapModel.DisplayName);
    }

    public void UploadMap()
    {
        uploadChoise.SetActive(true);
    }

    public void AcceptUploadMap()
    {
        uploadChoise.SetActive(false);
    }

    public void CancelUploadMap()
    {
        uploadChoise.SetActive(false);
    }

    private void UpdateCanvas()
    {
        LoadFactionLines();
        SetCitiesInCanvas();
        SetTroopsInCanvas();
    }

    private void SetCitiesInCanvas()
    {
        foreach (MapCityModel mapCityModel in mapModel.Cities)
        {
            _mapEditorLogic.SetCityInCanvas(mapCityModel);
        }
    }

    private void SetTroopsInCanvas()
    {
        foreach (MapTroopModel mapTroopModel in mapModel.Troops)
        {
            _mapEditorLogic.SetTroopInCanvas(mapTroopModel);
        }
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
        string image = string.Empty;
        //Falta inicializar el combo para que esto funcione
        //string image = cbMapImages.options[cbMaps.value].text;

        if (!string.IsNullOrWhiteSpace(image))
        {
            mapModel.SpritePath = image;
        }

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
        GlobalInfoFaction globalInfoFaction;

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

            globalInfoFaction = globalInfo.Factions.Find(item =>
            item.NameLiteral == cbFaction.options[cbFaction.value].text);

            playerModel.Name = string.IsNullOrEmpty(playerModel.Name) 
                ? RandomUtils.RandomStringValue(globalInfoFaction.IANames)
                : playerModel.Name;
            playerModel.IaId = cbPlayerType.value;
            playerModel.FactionId = globalInfoFaction.Id;
            playerModel.Alliance = StringUtils.ToInt32(txtAlliance.text);
            
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

    /// <summary>
    /// Este método analiza el mapa cargado para comprobar que todos sus datos son correctos, y si hay algo que no está bien trata de corregirlo.
    /// </summary>
    private void ValidateAndCorrectMap(MapModel mapModel)
    {
        // Todos los mapas deben tener al menos una facción.
        if (mapModel.Players.Count == 0)
        {
            AddNewFactionLine();
        }

        // El jugador de las ciudades es válido
        //ValidateCitiesPlayer(mapModel);

        // El jugador de las tropas es válido
        //ValidateTroopsPlayer(mapModel);

        // El MapSocketId debe comenzar en 0 y ser correlativo.
        if (mapModel.Players.Min(p => p.MapSocketId) != 0)
        {
            MakeMapSocketIdCorrelative(mapModel);
        }
    }

    /// <summary>
    /// Comprobar que el MapSocketId de las ciudades coincide con alguno de los de mapModel.Players.
    /// </summary>
    /// <param name="mapModel"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ValidateCitiesPlayer(MapModel mapModel)
    {
        throw new NotImplementedException();
    }

    // <summary>
    /// Comprobar que el MapSocketId de las tropas coincide con alguno de los de mapModel.Players.
    /// </summary>
    /// <param name="mapModel"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ValidateTroopsPlayer(MapModel mapModel)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Este método tiene que asegurarse de que el valor de MapSocketId de mapModel.Players comience en 0 y sea correlativo, (por ejemplo: para un mapa de 4 jugadores seria 0, 1, 2, 3).
    /// 
    /// El método no solo tiene que actualizar el MapSoccketId en mapModel.Players, sino también en mapModel.Cities y mapModel.troops.
    /// </summary>
    private void MakeMapSocketIdCorrelative(MapModel mapModel)
    {
        throw new NotImplementedException();
    }
}