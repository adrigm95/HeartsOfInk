using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class AllianceUtils
    {
        public static string ConvertToString(int alliance)
        {
            if (alliance == 0)
            {
                return string.Empty;
            }
            else
            {
                return Convert.ToString(alliance);
            }
        }
    }
}
