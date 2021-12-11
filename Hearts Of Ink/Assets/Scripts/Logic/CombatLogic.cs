using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Logic
{
    public class CombatLogic
    {
        const int MinDefense = 40;
        const int MaxDefense = 80;

        public CombatResults Combat(TroopController attacker, TroopController defensor)
        {
            CombatResults combatResults = new CombatResults();
            int attackerLosses;
            int defensorLosses;
            int bonusAttacker;
            int bonusDefensor;

            bonusAttacker = attacker.troopModel.Player.Faction.Bonus.BonusId == Bonus.Id.Combat ? 10 : 0;
            bonusDefensor = defensor.troopModel.Player.Faction.Bonus.BonusId == Bonus.Id.Combat ? 10 : 0;
            defensorLosses = attacker.troopModel.Units / (RandomUtils.Next(MinDefense, MaxDefense) + bonusDefensor) + 1;
            attackerLosses = defensor.troopModel.Units / (RandomUtils.Next(MinDefense, MaxDefense) + bonusAttacker) + 1;

            combatResults.DefensorRemainingUnits = CalcRemainingUnits(defensor.troopModel.Units, defensorLosses);
            combatResults.AttackerRemainingUnits = CalcRemainingUnits(attacker.troopModel.Units, attackerLosses);

            return combatResults;
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
