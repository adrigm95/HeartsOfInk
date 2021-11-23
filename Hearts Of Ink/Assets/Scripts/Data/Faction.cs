using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class Faction
    {
        public const string GovernmentTag = "Government";
        public const string RebelsTag = "Rebels";
        public const string VukisTag = "Vukis";
        public const string NomadsTag = "Nomads";

        public enum Id { GOVERNMENT, REBELS, VUKIS, NOMADS }
        public enum IA { PLAYER, IA, NEUTRAL}

        public Id FactionId { get; set; }
        public IA IaId { get; set; }

        public static string GetFactionById(Id factionId)
        {
            switch (factionId)
            {
                case Id.GOVERNMENT:
                    return GovernmentTag;
                case Id.REBELS:
                    return RebelsTag;
                case Id.VUKIS:
                    return VukisTag;
                case Id.NOMADS:
                    return NomadsTag;
                default:
                    throw new Exception("Unexpected faction id");
            }
        }
    }
}
