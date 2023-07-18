using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public static class PanelUtils
    {
        public static string GetGameName(BasicGameInfo game)
        {
            string fullGamename = string.Empty;

            fullGamename += game.Name;
            fullGamename += "(" + game.PlayersInside + "/4)";

            return fullGamename;
        }
    }
}
