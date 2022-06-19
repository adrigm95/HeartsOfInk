using Assets.Scripts.Data;
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
    public Transform cities;
    public Transform troops;
    public Dropdown cbMaps;
    public int startFactionLines;
    public int spacing;

    public void Start()
    {
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

    public void LoadFactionLine(MapPlayerModel player)
    {
        string prefabPath = "Prefabs/fileFactionSingleplayer";
        Transform newObject;
        Vector3 position;
        Dropdown cbFaction;
        Text txtFaction;
        Image btnColorFaction;
        GlobalInfoFaction faction;

        faction = globalInfo.Factions.Find(item => item.Id == player.FactionId);
        position = new Vector3(0, startFactionLines);
        position.y -= spacing * factions.Count;
        newObject = ((GameObject)Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine" + faction.Names[0].Value + "_" + faction.Id + "_" + player.MapSocketId;
        newObject.SetParent(this.transform, false);

        cbFaction = newObject.Find("cbFaction").GetComponent<Dropdown>();
        txtFaction = newObject.Find("txtFaction").GetComponent<Text>();
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Image>();

        cbFaction.value = player.IaId;
        txtFaction.text = faction.Names[0].Value;
        btnColorFaction.color = ColorUtils.GetColorByString(player.Color);

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
    }

    private void SetCitiesInCanvas()
    {
        //TODO
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

    private void SetMapInfoOnModel()
    {
        string mapName = cbMaps.options[cbMaps.value].text;

        //TODO
    }

    private void SaveCitiesInModel()
    {
        mapModel.Cities = new List<MapCityModel>();

        foreach (Transform city in cities)
        {
            EditorCity editorCity = city.GetComponent<EditorCity>();

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

        foreach (Transform troop in troops)
        {
            EditorTroop editorCity = troop.GetComponent<EditorTroop>();
            unitsText = troop.GetComponent<TextMeshProUGUI>();

            mapModel.Troops.Add(new MapTroopModel()
            {
                MapSocketId = Convert.ToByte(editorCity.ownerSocketId),
                Position = new float[] { editorCity.transform.position.x, editorCity.transform.position.y },
                Units = Convert.ToInt32(unitsText.text)
            });
        }
    }
}
