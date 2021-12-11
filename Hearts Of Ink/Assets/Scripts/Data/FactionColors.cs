using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public static class FactionColors
    {
        public static Color GetColorByString(string color)
        {
            string[] splittedString = color.Split(',');

            return ColorUtils.BuildColorBase256(float.Parse(splittedString[0]), float.Parse(splittedString[1]), float.Parse(splittedString[2]));
        }
    }
}
