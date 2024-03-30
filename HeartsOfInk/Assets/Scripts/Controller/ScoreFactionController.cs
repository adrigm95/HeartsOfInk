using Assets.Scripts.Data;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controller
{
    internal class ScoreFactionController
    {
        List<FactionStatistics> factionStatistics;

        public ScoreFactionController()
        {
            factionStatistics = new List<FactionStatistics>();
        }

        public FactionStatistics GetFaction(Player factionId)
        {
            return factionStatistics.Find(item => item.Player == factionId);
        }
        //Esta función servirá para saber cuando una tropa es aliada o enemiga
        public bool IsEnemy(Player factionId, Player factionId2)
        {
            return factionId != factionId2;
        }


    }
}
