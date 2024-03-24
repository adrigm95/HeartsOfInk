using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Logic
{
    public class CombatLogic
    {
        const int MinDefense = 60;
        const int MaxDefense = 80;

        public CombatResults Combat(TroopController attacker, TroopController defensor)
        {
            CombatResults combatResults = new CombatResults();
            int attackerLosses;
            int defensorLosses;

            defensorLosses = CalculateLosses(attacker.troopModel, defensor.troopModel);
            attackerLosses = CalculateLosses(defensor.troopModel, attacker.troopModel);

            combatResults.DefensorRemainingUnits = CalcRemainingUnits(defensor.troopModel.Units, defensorLosses);
            combatResults.AttackerRemainingUnits = CalcRemainingUnits(attacker.troopModel.Units, attackerLosses);

            return combatResults;
        }

        private int CalculateLosses(TroopModel enemyTroop, TroopModel troop)
        {
            Bonus.Id bonusId = troop.Player.Faction.Bonus.BonusId;
            int yourUnits = troop.Units;
            int combatBonus = bonusId == Bonus.Id.Combat ? 15 : 0;
            int losses;

            losses = enemyTroop.Units / (RandomUtils.Next(MinDefense, MaxDefense) + combatBonus) + 1;

            if (bonusId == Bonus.Id.MoveOnCombat && (yourUnits - losses) < GlobalConstants.GuerrillaLimit)
            {
                losses = yourUnits;
            }

            return losses;
        }

        public int CalcRemainingUnits(int startUnits, int losses)
        {
            if (losses > startUnits)
            {
                return 0;
            }
            else
            {
                return startUnits - losses;
            }
        }
    }
}
