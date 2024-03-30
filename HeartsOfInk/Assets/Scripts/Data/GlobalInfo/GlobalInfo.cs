using System.Collections.Generic;

namespace Assets.Scripts.Data.GlobalInfo
{
    public class GlobalInfo
    {
        public List<GlobalInfoFaction> Factions { get; set; }
        public List<GlobalInfoBonus> Bonus { get; set; }
        public List<string> AvailableColors { get; set; }

        public override string ToString()
        {
            return "{ 'Factions':" + Factions + ", 'Bonus':" + Bonus + ", 'AvailableColors': " + AvailableColors + " }";
        }
    }
}
