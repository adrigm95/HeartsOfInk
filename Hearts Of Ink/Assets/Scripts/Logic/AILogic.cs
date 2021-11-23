using Assets.Scripts.Data;
using Rawgen.Math.Logic.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AILogic
{
    private GlobalLogicController globalLogic;
    public Faction.Id FactionId { get; set; }

    public AILogic(Faction.Id factionId, GlobalLogicController globalLogicController)
    {
        FactionId = factionId;
        globalLogic = globalLogicController;
    }

    public void TroopMovementRequest(TroopModel troopModel)
    {
        if (troopModel.Target == null)
        {
            troopModel.SetTarget(GetAttackTarget(troopModel), globalLogic);
        }
        else 
        {
            CityController cityController = troopModel.Target.GetComponent<CityController>();

            if (cityController != null && cityController.Owner == troopModel.FactionId)
            {
                troopModel.SetTarget(GetAttackTarget(troopModel), globalLogic);
            }
        }
    }

    private GameObject GetAttackTarget(TroopModel troopModel)
    {
        GameObject target = null;
        Vector2 currentPos = troopModel.CurrentPosition;
        GameObject[] cities = GameObject.FindGameObjectsWithTag("City");
        Dictionary<int, float> distances = new Dictionary<int, float>(); 

        for (int index = 0; index < cities.Length; index++)
        {
            CityController cityController = cities[index].GetComponent<CityController>();

            if (cityController.Owner != FactionId)
            {
                distances.Add(index, MathUtils.ExperimentalDistance(currentPos.x, currentPos.y, cities[index].transform.position.x, cities[index].transform.position.y));
            }
        }

        if (distances.Count > 0)
        {
            int key = distances.OrderBy(item => item.Value).First().Key;
            target = cities[key];
        }
        else if (distances.Count == 1)
        {
            target = cities[distances.Keys.First()];
        }

        return target;
    }
}
