using Assets.Scripts.Data;
using Assets.Scripts.Data.MapModels;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FactionsPanelController : MonoBehaviour
{
    private const int PlayerOwnerValue = 0;
    private MapModel mapModel;
    private List<Dropdown> factions;
    private BonusData bonusData;
    public int spacing;
    public Text description;
    public Text bonus;

    private void Start()
    {
        factions = new List<Dropdown>();
        bonusData = new BonusData(Rawgen.Literals.LiteralsFactory.Language.es_ES);
        LoadMap();
    }

    public void LoadMap()
    {
        string filePath = Application.streamingAssetsPath + "/MapDefinitions/0_Cartarena.json";

        mapModel = JsonCustomUtils<MapModel>.ReadObjectFromFile(filePath);
        
        foreach (MapFactionModel faction in mapModel.Factions)
        {
            LoadFactionLine(faction);
        }
    }

    public void LoadFactionLine(MapFactionModel faction)
    {
        string prefabPath = "Prefabs/fileFactionSingleplayer";
        Transform newObject;
        Vector3 position;
        Dropdown cbFaction;
        Text txtFaction;
        Image btnColorFaction;

        position = new Vector3(0, -50);
        position.y -= spacing * factions.Count;
        newObject = ((GameObject) Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine" + faction.Name + "_" + faction.Id;
        newObject.SetParent(this.transform, false);

        cbFaction = newObject.Find("cbFaction").GetComponent<Dropdown>();
        txtFaction = newObject.Find("txtFaction").GetComponent<Text>();
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Image>();

        cbFaction.value = faction.DefaultOwner;
        cbFaction.onValueChanged.AddListener(delegate { OnValueChange(cbFaction); });
        txtFaction.text = faction.Name;
        btnColorFaction.color = FactionColors.GetColorByFaction((Faction.Id) faction.Id);

        factions.Add(cbFaction);

        if (faction.DefaultOwner == PlayerOwnerValue)
        {
            ChangeFactionDescriptions(cbFaction);
        }
    }

    public void OnValueChange(Dropdown comboOrder)
    {
        if (comboOrder.value == 0)
        {
            QuitOtherPlayers(comboOrder);
            ChangeFactionDescriptions(comboOrder);
        }
    }

    private void ChangeFactionDescriptions(Dropdown comboOrder)
    {
        int comboFactionId = Convert.ToInt32(comboOrder.transform.parent.gameObject.name.Split('_')[1]);

        foreach (MapFactionModel faction in mapModel.Factions)
        {
            if (faction.Id == comboFactionId)
            {
                description.text = faction.Descriptions[0].Description;
                bonus.text = bonusData.GetBonusLiteralById((BonusData.Id) faction.BonusCode);
            }
        }
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
