using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateMapPanelController : MonoBehaviour
{
    [SerializeField]
    private EditorPanelController editorPanelController;
    [SerializeField]
    private Dropdown background;
    [SerializeField]
    private InputField mapName;
    [SerializeField]
    private Toggle isForSingleplayer;
    [SerializeField]
    private Toggle isForMultiplayer;
    [SerializeField]
    private Text backgroundPath;
    [SerializeField]
    private MapController mapController;

    // Start is called before the first frame update
    void Start()
    {
        LoadBackgroundCombo();
        SetBackgroundPath();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePanel()
    {
        this.gameObject.SetActive(true);
        editorPanelController.gameObject.SetActive(false);
    }

    public void CancelCreation()
    {
        this.gameObject.SetActive(false);
        editorPanelController.gameObject.SetActive(true);
    }

    public void CreateMap()
    {
        string mapId = GenerateMapId();
        string displayName = FileUtils.SanitizeFilename(mapName.text);
        gameObject.SetActive(false);

        MapModelHeader mapModelHeader = new MapModelHeader()
        {
            MapId = mapId,
            DisplayName = displayName,
            DefinitionName = mapName.text,
            SpriteName = background.options[background.value].text,
            AvailableForMultiplayer = isForMultiplayer.isOn,
            AvailableForSingleplayer = isForSingleplayer.isOn
        };

        MapModel mapModel = new MapModel()
        {
            IsOfficial = false,
            MapId = mapId,
            DisplayName = displayName,
            DefinitionName = mapName.text,
            SpriteName = background.options[background.value].text,
            AvailableForMultiplayer = isForMultiplayer.isOn,
            AvailableForSingleplayer = isForSingleplayer.isOn,
            PlayerSlots = new List<MapPlayerSlotModel>(),
            Cities = new List<MapCityModel>(),
            Troops = new List<MapTroopModel>()
        };

        MapDAC.SaveMapHeader(mapModelHeader, GlobalConstants.RootPath);
        MapDAC.SaveMapDefinition(mapModel, GlobalConstants.RootPath);
        editorPanelController.gameObject.SetActive(true);
        editorPanelController.LoadAvailableMaps(displayName);
    }

    public void OnChangeBackground()
    {
        string spriteFilename = background.options[background.value].text;

        mapController.UpdateMap("MapSprites/" + spriteFilename);
        if (string.IsNullOrWhiteSpace(mapName.text))
        {
            mapName.text = spriteFilename;
        }
    }

    private void LoadBackgroundCombo()
    {
        Debug.Log("Loading available sprites");
        background.AddOptions(MapDAC.GetAvailableSprites(GlobalConstants.RootPath));
        background.RefreshShownValue();
        string spriteFilename = background.options[background.value].text;
        mapController.UpdateMap("MapSprites/" + spriteFilename);
    }

    private void SetBackgroundPath()
    {
        backgroundPath.text = Application.persistentDataPath + "/MapSprites/";
    }

    private string GenerateMapId()
    {
        List<MapModelHeader> availableMaps = MapDAC.GetAvailableMaps(GlobalConstants.RootPath);
        int maxId = availableMaps.Max(map => Convert.ToInt32(map.MapId));
        maxId += 1;

        Debug.Log($"Generating new map id: {maxId}");

        return Convert.ToString(maxId);
    }
}
