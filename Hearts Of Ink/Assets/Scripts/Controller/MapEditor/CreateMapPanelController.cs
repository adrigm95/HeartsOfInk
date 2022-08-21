using Assets.Scripts.Data;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMapPanelController : MonoBehaviour
{
    [SerializeField]
    private EditorPanelController rightPanelController;
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
        rightPanelController.gameObject.SetActive(false);
    }

    public void CancelCreation()
    {
        this.gameObject.SetActive(false);
        rightPanelController.gameObject.SetActive(true);
    }

    public void CreateMap()
    {
        MapModelHeader mapModelHeader = new MapModelHeader()
        {
            MapId = GenerateMapId(),
            DisplayName = FileUtils.SanitizeFilename(mapName.text),
            DefinitionName = mapName.text,
            SpritePath = "MapSprites/" + background.options[background.value].text,
            AvailableForMultiplayer = isForMultiplayer.isOn,
            AvailableForSingleplayer = isForSingleplayer.isOn
        };

        MapModel mapModel = new MapModel()
        {
            MapId = GenerateMapId(),
            DisplayName = FileUtils.SanitizeFilename(mapName.text),
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
        short maxId = short.MinValue;

        foreach (MapModelHeader map in availableMaps)
        {
            if (map.MapId > maxId)
            {
                maxId = map.MapId;
            }
        }

        return maxId++;
    }
}
