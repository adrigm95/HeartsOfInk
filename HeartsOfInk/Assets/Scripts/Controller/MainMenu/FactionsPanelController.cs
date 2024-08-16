using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using LobbyHOIServer.Models.Models.In;
using LobbyHOIServer.Models.Models;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionsPanelController : MonoBehaviour
{
    private const int PlayerOwnerValue = 0;
    private const byte NoAlliance = 0;

    private MapModel mapModel;
    private GlobalInfo globalInfo;
    private List<Dropdown> factions;
    private List<MapModelHeader> availableMaps;
    public List<ConfigLineModel> _configLinesState;
    [SerializeField]
    private StartGameController startGameController;
    public int startFactionLines;
    public int spacing;
    public Text factionDescription;
    public Text txtGamekey;
    public Text bonusDescription;
    public Dropdown cbMaps;

    void Start()
    {
        factions = new List<Dropdown>();
        availableMaps = MapDAC.GetAvailableMaps(GlobalConstants.RootPath, false);
        availableMaps.ForEach(map => cbMaps.options.Add(new Dropdown.OptionData(map.DisplayName)));
        cbMaps.RefreshShownValue();
        cbMaps.onValueChanged.AddListener(delegate { LoadMap(); });
        LoadMap();
    }

    public void LoadMap()
    {
        Debug.Log("Loading map: " + cbMaps.itemText.text);
        mapModel = MapDAC.LoadMapInfoByName(GetMapDefinitionName(), GlobalConstants.RootPath);
        startGameController.SetMapId(mapModel.MapId);
        globalInfo = GlobalInfoDAC.LoadGlobalMapInfo();

        CleanFactionLines();
        foreach (MapPlayerSlotModel playerSlot in mapModel.PlayerSlots)
        {
            LoadFactionLine(playerSlot);
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

    public string GetMapDefinitionId()
    {
        return availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text).MapId;
    }

    public void LoadFactionLine(MapPlayerSlotModel playerSlot)
    {
        string prefabPath = "Prefabs/fileFactionSingleplayer";
        Transform newObject;
        Vector3 position;
        Dropdown cbFaction;
        Text txtFaction;
        Image btnColorImage;
        Button btnColorFaction;
        GlobalInfoFaction faction;
        Text txtAlliance;

        faction = globalInfo.Factions.Find(item => item.Id == playerSlot.FactionId);
        position = new Vector3(0, startFactionLines);
        position.y -= spacing * factions.Count;
        newObject = ((GameObject)Instantiate(Resources.Load(prefabPath), position, transform.rotation)).transform;
        newObject.name = "factionLine" + faction.NameLiteral + "_" + faction.Id + "_" + playerSlot.Id;
        newObject.SetParent(this.transform, false);
        newObject.gameObject.SetActive(playerSlot.IsPlayable);

        cbFaction = newObject.Find("cbFaction").GetComponent<Dropdown>();
        txtFaction = newObject.Find("txtFaction").GetComponent<Text>();
        btnColorFaction = newObject.Find("btnColorFaction").GetComponent<Button>();
        btnColorImage = newObject.Find("btnColorFaction").GetComponent<Image>();
        txtAlliance = newObject.Find("btnAlliance").GetComponentInChildren<Text>();
        Player.IA slotPlayerType = (Player.IA)playerSlot.IaId;
        cbFaction.value = slotPlayerType == Player.IA.OTHER_PLAYER ? (int)Player.IA.IA : playerSlot.IaId;
        cbFaction.onValueChanged.AddListener(delegate { CbFaction_OnValueChange(cbFaction); });
        txtFaction.text = faction.NameLiteral;
        btnColorImage.color = ColorUtils.GetColorByString(playerSlot.Color);
        txtAlliance.text = AllianceUtils.ConvertToString(playerSlot.Alliance);

        factions.Add(cbFaction);

        if (playerSlot.IaId == PlayerOwnerValue)
        {
            ChangeFactionDescriptions(faction);
        }
        btnColorFaction.onClick.AddListener(delegate { OnClick_PlayerColor(btnColorImage); });
    }

    public void OnClick_PlayerColor(Image colorImage)
    {
        Debug.Log($"Color changed for image {colorImage.name}; color: {colorImage.color}");
        Debug.Log($"Current global info {globalInfo}");
        colorImage.color = ColorUtils.NextColor(colorImage.color, globalInfo.AvailableColors);
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
