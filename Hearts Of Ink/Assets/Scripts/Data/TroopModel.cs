using NETCoreServer.Models.GameModel;
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

        public Player Player { get; set; }
        public bool InCombat { get; set; }
        public int Units { get; set; }

        public TroopModel(Player player)
        {
            this.Player = player;
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
    }
}
