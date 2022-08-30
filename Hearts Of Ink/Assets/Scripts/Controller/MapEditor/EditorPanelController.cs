using Assets.Scripts.Data;
using Assets.Scripts.Data.EditorModels;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorPanelController : MonoBehaviour
{
    private MapModel mapModel;
    private GlobalInfo globalInfo;
    private List<Dropdown> factions;
    private List<MapModelHeader> availableMaps;
    private List<EditorConfigLine> configLines;
    public Transform citiesHolder;
    public Transform troopsHolder;
    public Dropdown cbMaps;
    public int startFactionLines;
    public int spacing;

    public void Start()
    {
        factions = new List<Dropdown>();
        configLines = new List<EditorConfigLine>();
        availableMaps = MapDAC.GetAvailableMaps(false);
        availableMaps.ForEach(map => cbMaps.options.Add(new Dropdown.OptionData(map.DisplayName)));
        cbMaps.RefreshShownValue();
        cbMaps.onValueChanged.AddListener(delegate { LoadMap(); });
        LoadMap();
    }

    public void LoadMap()
    {
        Debug.Log("Loading map: " + cbMaps.itemText.text);
        mapModel = MapDAC.LoadMapInfo(availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text).DefinitionName);
        globalInfo = MapDAC.LoadGlobalMapInfo();

        MapController.Instance.UpdateMap(mapModel.SpritePath);
        UpdateCanvas();
    }

    public void AddNewFactionLine()
    {
        LoadFactionLine(new MapPlayerModel());
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
        Dropdown cbFaction;
        Dropdown cbPlayerType;
        Image btnColorFaction;
        GlobalInfoFaction faction;
        Toggle tgIsPlayable;
        EditorConfigLine configLine = new EditorConfigLine();

        faction = globalInfo.Factions.Find(item => item.Id == player.FactionId);
        position = new Vector3(0, startFactionLines);
        position.y -= spacing * factions.Count;
        newObject = ((GameObject)Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine" + faction.Names[0].Value + "_" + faction.Id + "_" + player.MapSocketId;
        newObject.SetParent(this.transform, false);

        cbFaction = newObject.Find("cbFaction").GetComponent<Dropdown>();
        cbPlayerType = newObject.Find("cbPlayerType").GetComponent<Dropdown>();
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Image>();
        tgIsPlayable = newObject.Find("tgIsPlayable").GetComponent<Toggle>();

        cbPlayerType.value = player.IaId;
        btnColorFaction.color = ColorUtils.GetColorByString(player.Color);
        LoadFactionsCombo(cbFaction, player.FactionId);
        tgIsPlayable.isOn = player.IsPlayable;

        configLines.Add(configLine);
        factions.Add(cbFaction);
    }

    public void SaveMap()
    {
        UpdateModel();
        MapDAC.SaveMapDefinition(mapModel);
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
            string resourceName = mapCityModel.Type == 1 ? "Prefabs/EditorCityPrefab" : "Prefabs/EditorCapital";

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
                //TODO: Modificar con evolutivo de multilenguaje.
                text = factionInfo.Names[0].Value
            });
        }

        factionsCombo.AddOptions(dropdownOptions);
        factionsCombo.RefreshShownValue();
        factionsCombo.value = factionId;
    }

    private void SetMapInfoOnModel()
    {
        string mapName = cbMaps.options[cbMaps.value].text;

        //TODO
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
        configLines.Clear();
    }
}