using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsController : MonoBehaviour
{
    List<FactionStatistics> factionsStatistics;

    // Start is called before the first frame update
    void Start()
    {
        factionsStatistics = new List<FactionStatistics>();

        //factionsStatistics.Add(new FactionStatistics(Faction.Id.GOVERNMENT));
        //factionsStatistics.Add(new FactionStatistics(Faction.Id.NOMADS));
        //factionsStatistics.Add(new FactionStatistics(Faction.Id.REBELS));
        //factionsStatistics.Add(new FactionStatistics(Faction.Id.VUKIS));

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FactionStatistics GetFaction(Player factionId)
    {
        return factionsStatistics.Find(item => item.Player == factionId);
    }

    public void ReportArmyDefeated(TroopController destroyed, TroopController destroyer)
    {
        Player destroyedFaction = destroyed.troopModel.Player;
        Player destroyerFaction = destroyer.troopModel.Player;

        foreach (FactionStatistics factionStatistics in factionsStatistics)
        {
            if (factionStatistics.Player == destroyedFaction ||
                factionStatistics.Player == destroyerFaction)
            {
                factionStatistics.ReportArmyDefeated(destroyedFaction);
            }
        }
    }

    public void ReportGameEnd(List<CityController> cities)
    {
        foreach (FactionStatistics factionStats in factionsStatistics)
        {
            factionStats.SetCitiesAtEnd(cities);
        }
    }
}
