using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private GameObject troopsCanvas;
    private GlobalLogicController globalLogic;
    private List<TroopController> troopsInZone;
    private float recruitmentProgress = 0;
    public Faction.Id Owner;
    public bool IsCapital;

    // Start is called before the first frame update
    void Start()
    {
        troopsCanvas = FindObjectsOfType<Canvas>().Where(item => item.name == "TroopsCanvas").First().gameObject;
        globalLogic = FindObjectOfType<GlobalLogicController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        troopsInZone = new List<TroopController>();
    }

    // Update is called once per frame
    void Update()
    {
        CleanUnitsInZoneList();
        UpdateOwner();
        RecruitTroops();
    }

    private void CleanUnitsInZoneList()
    {
        troopsInZone.RemoveAll(item => item == null || item.name == null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TroopController troopController;

        troopController = collision.gameObject.GetComponent<TroopController>();

        if (troopController != null)
        {
            troopsInZone.Add(troopController);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        TroopController troopController;

        troopController = collision.gameObject.GetComponent<TroopController>();

        if (troopController != null)
        {
            troopsInZone.Remove(troopController);
        }
    }

    private void OnMouseDown()
    {
        globalLogic.ClickReceivedFromCity(this);
        Debug.Log("On click: " + this);
    }

    private void UpdateOwner()
    {
        if (troopsInZone.Count == 1)
        {
            if (troopsInZone[0].troopModel.FactionId != Owner)
            {
                ChangeOwner(troopsInZone[0].troopModel.FactionId);
            }
        }
        else if (troopsInZone.Count < 0)
        {
            bool ownerPresent = false;
            Dictionary<Faction.Id, float> unitsInCombat = new Dictionary<Faction.Id, float>();

            foreach (TroopController troopController in troopsInZone)
            {
                Faction.Id troopFaction = troopController.troopModel.FactionId;

                if (troopFaction == Owner)
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
                ChangeOwner(unitsInCombat.First().Key);
            }
        }
    }

    private void ChangeOwner(Faction.Id newOwner)
    {
        Owner = newOwner;
        spriteRenderer.color = FactionColors.GetColorByFaction(Owner);
        recruitmentProgress = 0;
    }

    private void RecruitTroops()
    {
        const int CompanySize = 80;
        const float recruitmentSpeed = 0.5f;
        string troopName;
        float capitalBonus = 1;
        UnityEngine.Object newObject;

        if (IsCapital)
        {
            capitalBonus = 1.5f;
        }

        recruitmentProgress += Time.deltaTime * globalLogic.GameSpeed * capitalBonus * recruitmentSpeed;

        if (recruitmentProgress > CompanySize)
        {
            troopName = "Prefabs/" + Faction.GetFactionById(Owner) + "Troop";
            newObject = Instantiate(Resources.Load(troopName), transform.position, transform.rotation, troopsCanvas.transform);
            newObject.name += globalLogic.unitsCounter;

            globalLogic.unitsCounter++;
            recruitmentProgress = 0;
        }
    }
}
