using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class TroopModel
    {
        public GameObject Target { get; private set; }
        public Vector2 CurrentPosition { get; set; }
        public float Speed { get; set; }
        public Faction.Id FactionId { get; set; }
        public bool InCombat { get; set; }
        public int Units { get; set; }

        public TroopModel(string troopName)
        {
            SetFactionId(troopName);
            Speed = 1;
        }

        public void SetTarget(GameObject newTarget, GlobalLogicController globalLogic)
        {
            globalLogic.CleanTroopSelection(this);
            Target = newTarget;
        }

        public void MergeTroop(TroopModel otherTroop)
        {
            Units += otherTroop.Units;
        }

        private void SetFactionId(string troopName)
        {
            if (troopName.Contains(Faction.GovernmentTag))
            {
                FactionId = Faction.Id.GOVERNMENT;
            }
            else if (troopName.Contains(Faction.RebelsTag))
            {
                FactionId = Faction.Id.REBELS;
            }
            else if (troopName.Contains(Faction.VukisTag))
            {
                FactionId = Faction.Id.VUKIS;
            }
            else if (troopName.Contains(Faction.NomadsTag))
            {
                FactionId = Faction.Id.NOMADS;
            }
        }
    }
}
