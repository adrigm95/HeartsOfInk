using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
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
    public int spacing;
    public Text factionDescription;
    public Text bonusDescription;

    private void Start()
    {
        factions = new List<Dropdown>();
        LoadMap();
    }

    public void LoadMap()
    {
        string mapPath = Application.streamingAssetsPath + "/MapDefinitions/0_Cartarena_v0_3_0.json";
        string globalInfoPath = Application.streamingAssetsPath + "/MapDefinitions/_GlobalInfo.json";

        mapModel = JsonCustomUtils<MapModel>.ReadObjectFromFile(mapPath);
        globalInfo = JsonCustomUtils<GlobalInfo>.ReadObjectFromFile(globalInfoPath);

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
        position = new Vector3(0, -50);
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
