using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public static class FactionColors
    {
        public static Color GovernmentColor { get; } = ColorUtils.BuildColorBase256(88, 212, 255);
        public static Color RebelsColor { get; } = ColorUtils.BuildColorBase256(255, 0, 46);
        public static Color VukisColor { get; } = ColorUtils.BuildColorBase256(0, 234, 61);
        public static Color NomadsColor { get; } = ColorUtils.BuildColorBase256(255, 223, 0);

        public static Color GetColorByFaction(Faction.Id id)
        {
            switch (id)
            {
                case Faction.Id.GOVERNMENT:
                    return GovernmentColor;
                case Faction.Id.REBELS:
                    return RebelsColor;
                case Faction.Id.VUKIS:
                    return VukisColor;
                case Faction.Id.NOMADS:
                    return NomadsColor;
                default:
                    throw new Exception("Unexpected faction id in GetColorByFaction");
            }
        }
    }
}
