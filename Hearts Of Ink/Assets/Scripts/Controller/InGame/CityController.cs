using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private GlobalLogicController globalLogic;
    private List<TroopController> troopsInZone;
    private StateController stateHolder;
    private float recruitmentProgress = 0;
    public Player Owner;
    public bool IsCapital;

    public void Awake()
    {
        troopsInZone = new List<TroopController>();
    }

    // Start is called before the first frame update
    public void Start()
    {
        globalLogic = FindObjectOfType<GlobalLogicController>();
        stateHolder = FindObjectOfType<StateController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = ColorUtils.GetColorByString(Owner.Color);
    }

    // Update is called once per frame
    void Update()
    {
        if (globalLogic.IsMultiplayerHost || globalLogic.IsSingleplayer)
        {
            CleanUnitsInZoneList();
            UpdateOwner();
            RecruitTroops();

            if (globalLogic.IsMultiplayerHost)
            {
                stateHolder.SetCityOwner(Owner);
            }
        }
        else if (globalLogic.IsMultiplayerClient)
        {
            Owner = stateHolder.GetCityOwner(this.name);
        }
        else
        {
            Debug.LogWarning("Unexpected gametype on CityController");
        }
    }

    private void CleanUnitsInZoneList()
    {
        troopsInZone.RemoveAll(item => item == null || item.name == null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TroopController troopController;

        try
        {
            troopController = collision.gameObject.GetComponent<TroopController>();

            if (troopController != null)
            {
                troopsInZone.Add(troopController);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error detecting collision for city {this.name} and troop {collision.gameObject.name}, exception: {ex.Message}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TroopController troopController;

        troopController = collision.gameObject.GetComponent<TroopController>();

        if (troopController != null)
        {
            troopsInZone.Remove(troopController);
        }
    }

    private void UpdateOwner()
    {
        if (troopsInZone.Count == 1)
        {
            if (troopsInZone[0].troopModel.Player != Owner)
            {
                if (Owner.Alliance == Player.NoAlliance || troopsInZone[0].troopModel.Player.Alliance != Owner.Alliance)
                {
                    ChangeOwner(troopsInZone[0].troopModel.Player);
                }
            }
        }
        else if (troopsInZone.Count < 0)
        {
            bool ownerPresent = false;
            Dictionary<string, float> unitsInCombat = new Dictionary<string, float>();

            foreach (TroopController troopController in troopsInZone)
            {
                string troopFaction = troopController.troopModel.Player.Name;

                if (troopFaction == Owner.Name)
                {
                    ownerPresent = true;
                    break;
                }
                else if (unitsInCombat.ContainsKey(troopFaction))
                {
                    unitsInCombat[troopFaction] += troopController.troopModel.Units;
                }
                else
                {
                    unitsInCombat.Add(troopFaction, troopController.troopModel.Units);
                }
            }

            if (!ownerPresent)
            {
                unitsInCombat.OrderByDescending(item => item.Value);
                ChangeOwner(globalLogic.gameModel.Players.First(player => player.Name == unitsInCombat.First().Key));
            }
        }
    }

    private void ChangeOwner(Player newOwner)
    {
        Owner = newOwner;
        spriteRenderer.color = ColorUtils.GetColorByString(Owner.Color);
        recruitmentProgress = 0;
    }

    private void RecruitTroops()
    {
        const float recruitmentSpeed = 1f;
        float capitalBonus = 1;

        if (IsCapital)
        {
            capitalBonus = 1.5f;
        }

        if (Owner.Faction.Bonus.BonusId != Bonus.Id.NoArmy)
        {
            recruitmentProgress += Time.deltaTime * globalLogic.GameSpeed * capitalBonus * recruitmentSpeed;
        }

        if (recruitmentProgress > GlobalConstants.DefaultCompanySize)
        {
            globalLogic.InstantiateTroopSingleplayer(GlobalConstants.DefaultCompanySize, this.transform.position, Owner);
            recruitmentProgress = 0;
        }
    }
}
