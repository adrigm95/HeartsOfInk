using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class StringUtils
    {
        public static int ToInt32(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(number);
            }
        }
    }
}
