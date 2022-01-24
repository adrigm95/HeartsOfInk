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
    private List<MapModel> availableMaps;
    public int startFactionLines;
    public int spacing;
    public Text factionDescription;
    public Text bonusDescription;
    public Dropdown cbMaps;

    private void Start()
    {
        factions = new List<Dropdown>();
        availableMaps = MapDAC.GetAvailableMaps(false);
        availableMaps.ForEach(map => cbMaps.options.Add(new Dropdown.OptionData(map.DisplayName)));
        cbMaps.RefreshShownValue();
        LoadMap();
    }

    public void LoadMap()
    {
        //"0_Cartarena_v0_3_0";
        Debug.Log("ItemText: " + cbMaps.itemText.text);
        mapModel = MapDAC.LoadMapInfo(availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text).DefinitionName);
        globalInfo = MapDAC.LoadGlobalMapInfo();

        foreach (MapPlayerModel player in mapModel.Players)
        {
            LoadFactionLine(player);
        }
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
        newObject = ((GameObject) Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine" + faction.Names[0].Value + "_" + faction.Id + "_" + player.MapSocketId;
        newObject.SetParent(this.transform, false);

        cbFaction = newObject.Find("cbFaction").GetComponent<Dropdown>();
        txtFaction = newObject.Find("txtFaction").GetComponent<Text>();
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Image>();

        cbFaction.value = player.IaId;
        cbFaction.onValueChanged.AddListener(delegate { OnValueChange(cbFaction); });
        txtFaction.text = faction.Names[0].Value;
        btnColorFaction.color = ColorUtils.GetColorByString(player.Color);

        factions.Add(cbFaction);

        if (player.IaId == PlayerOwnerValue)
        {
            ChangeFactionDescriptions(faction);
        }
    }

    public void OnValueChange(Dropdown comboOrder)
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
        Debug.Log("New faction:" + faction.Names[0].Value);
        factionDescription.text = faction.Descriptions[0].Value;
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
