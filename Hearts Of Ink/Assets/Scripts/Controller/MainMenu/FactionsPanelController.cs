using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionsPanelController : MonoBehaviour
{
    private const int PlayerOwnerValue = 0;
    private MapModel mapModel;
    private GlobalInfo globalInfo;
    private List<Dropdown> factions;
    private List<MapModelHeader> availableMaps;
    [SerializeField]
    private StartGameController startGameController;
    public int startFactionLines;
    public int spacing;
    public Text factionDescription;
    public Text bonusDescription;
    public Dropdown cbMaps;

    void Start()
    {
        factions = new List<Dropdown>();
        availableMaps = MapDAC.GetAvailableMaps(false);
        availableMaps.ForEach(map => cbMaps.options.Add(new Dropdown.OptionData(map.DisplayName)));
        cbMaps.RefreshShownValue();
        cbMaps.onValueChanged.AddListener(delegate { LoadMap(); });
        LoadMap();
    }

    public void LoadMap()
    {
        Debug.Log("Loading map: " + cbMaps.itemText.text);
        mapModel = MapDAC.LoadMapInfo(GetMapDefinitionName());
        startGameController.MapId = mapModel.MapId;
        globalInfo = MapDAC.LoadGlobalMapInfo();

        CleanFactionLines();
        foreach (MapPlayerModel player in mapModel.Players)
        {
            LoadFactionLine(player);
        }
        MapController.Instance.UpdateMap(mapModel.SpritePath);
    }

    private void CleanFactionLines()
    {
        foreach (Dropdown cbFaction in factions)
        {
            Destroy(cbFaction.transform.parent.gameObject);
        }

        factions.Clear();
    }

    public string GetMapDefinitionName()
    {
        return availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text).DefinitionName;
    }

    public short GetMapDefinitionId()
    {
        return availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text).MapId;
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
        Text txtAlliance;

        faction = globalInfo.Factions.Find(item => item.Id == player.FactionId);
        position = new Vector3(0, startFactionLines);
        position.y -= spacing * factions.Count;
        newObject = ((GameObject) Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine" + faction.NameLiteral + "_" + faction.Id + "_" + player.MapSocketId;
        newObject.SetParent(this.transform, false);
        newObject.gameObject.SetActive(player.IsPlayable);

        cbFaction = newObject.Find("cbFaction").GetComponent<Dropdown>();
        txtFaction = newObject.Find("txtFaction").GetComponent<Text>();
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Image>();
        txtAlliance = newObject.Find("btnAlliance").GetComponentInChildren<Text>();

        cbFaction.value = player.IaId;
        cbFaction.onValueChanged.AddListener(delegate { CbFaction_OnValueChange(cbFaction); });
        txtFaction.text = faction.NameLiteral;
        btnColorFaction.color = ColorUtils.GetColorByString(player.Color);
        txtAlliance.text = AllianceUtils.ConvertToString(player.Alliance);

        factions.Add(cbFaction);

        if (player.IaId == PlayerOwnerValue)
        {
            ChangeFactionDescriptions(faction);
        }
    }

    public void CbFaction_OnValueChange(Dropdown comboOrder)
    {
        if (comboOrder.value == 0)
        {
            int comboFactionId = Convert.ToInt32(comboOrder.transform.parent.gameObject.name.Split('_')[1]);
            GlobalInfoFaction globalInfoFaction = globalInfo.Factions.Find(faction => faction.Id == comboFactionId);

            QuitOtherPlayers(comboOrder);
            ChangeFactionDescriptions(globalInfoFaction);
        }
    }

    private void ChangeFactionDescriptions(GlobalInfoFaction faction)
    {
        Debug.Log("New faction:" + faction.NameLiteral);
        factionDescription.text = faction.DescriptionLiteral;
        bonusDescription.text = globalInfo.Bonus.Find(bonus => bonus.Id == faction.BonusId).Descriptions[0].Value;
    }

    private void QuitOtherPlayers(Dropdown comboOrder)
    {
        foreach (Dropdown faction in factions)
        {
            if (faction.value == 0 && !faction.Equals(comboOrder))
            {
                faction.value = 1;
            }
        }
    }
}
