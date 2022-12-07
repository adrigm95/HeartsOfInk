using Assets.Scripts.Data;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using System;
using System.Collections;
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
        short mapId = GenerateMapId();
        string displayName = FileUtils.SanitizeFilename(mapName.text);
        gameObject.SetActive(false);

        MapModelHeader mapModelHeader = new MapModelHeader()
        {
            MapId = mapId,
            DisplayName = displayName,
            DefinitionName = mapName.text,
            SpritePath = "MapSprites/" + background.options[background.value].text,
            AvailableForMultiplayer = isForMultiplayer.isOn,
            AvailableForSingleplayer = isForSingleplayer.isOn
        };

        MapModel mapModel = new MapModel()
        {
            MapId = mapId,
            DisplayName = displayName,
            DefinitionName = mapName.text,
            SpritePath = "MapSprites/" + background.options[background.value].text,
            AvailableForMultiplayer = isForMultiplayer.isOn,
            AvailableForSingleplayer = isForSingleplayer.isOn,
            Players = new List<MapPlayerModel>(),
            Cities = new List<MapCityModel>(),
            Troops = new List<MapTroopModel>()
        };

        MapDAC.SaveMapHeader(mapModelHeader);
        MapDAC.SaveMapDefinition(mapModel);
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
        background.AddOptions(MapDAC.GetAvailableSprites());
        background.RefreshShownValue();
        string spriteFilename = background.options[background.value].text;
        mapController.UpdateMap("MapSprites/" + spriteFilename);
    }

    private void SetBackgroundPath()
    {
        backgroundPath.text = Application.streamingAssetsPath + "/MapSprites/";
    }

    private short GenerateMapId()
    {
        List<MapModelHeader> availableMaps = MapDAC.GetAvailableMaps();
        short maxId = availableMaps.Max(map => map.MapId);

        Debug.Log($"Generating new map id: {maxId}");

        return Convert.ToInt16(maxId + 1);
    }
}
