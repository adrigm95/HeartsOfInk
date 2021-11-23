using NETCoreServer.Config;
using NETCoreServer.Data;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NETCoreServer.InternalLogic
{
    /// <summary>
    /// Clase destinada a realizar operaciones de mantenimiento de las listas de partidas activas.
    /// </summary>
    public class GamesListManteinance
    {
        public void CleanInactiveGames()
        {
            CleanLogic(GamesData.GetPublicGamesInfo());
            CleanLogic(GamesData.GetPrivateGamesInfo());
        }

        private void CleanLogic(List<ServerGameInfo> gamesList)
        {
            long minExpectedTime = DateTime.UtcNow.Ticks - Convert.ToInt64(TimeSpan.TicksPerSecond * SharedConfig.NoNoticeTimeout);
            Stack<ServerGameInfo> gamesToRemove = new Stack<ServerGameInfo>();

            foreach (ServerGameInfo game in gamesList)
            {
                if (game.lastNoticeFromHost.Ticks < minExpectedTime)
                {
                    gamesToRemove.Push(game);
                }
            }

            while (gamesToRemove.Count > 0)
            {
                GamesData.RemoveGame(gamesToRemove.Pop());
            }
        }
    }
}
