using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public static class RandomUtils
    {
        private static Random random = new Random();

        public static int Next(int maxValue)
        {
            return random.Next(0, maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public static string RandomPlayerName()
        {
            return "player" + Next(99999);
        }
    }
}
