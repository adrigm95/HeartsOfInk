using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Data
{
    public static class GamesData
    {
        private static List<BasicGameInfo> publicGames = new List<BasicGameInfo>();
        private static List<BasicGameInfo> privateGames = new List<BasicGameInfo>();
        private static List<ServerGameInfo> publicGamesServerInfo = new List<ServerGameInfo>();
        private static List<ServerGameInfo> privateGamesServerInfo = new List<ServerGameInfo>();

        public static List<BasicGameInfo> GetPublicGames()
        {
            return publicGames;
        }

        public static List<BasicGameInfo> GetPrivateGames()
        {
            return privateGames;
        }

        public static List<ServerGameInfo> GetPublicGamesInfo()
        {
            return publicGamesServerInfo;
        }

        public static List<ServerGameInfo> GetPrivateGamesInfo()
        {
            return privateGamesServerInfo;
        }

        /// <summary>
        /// Busca la partida en las listas de partidas públicas y privadas y, si existe, notifica que está activa.
        /// </summary>
        /// <param name="gamekey"> Identificador de partida.</param>
        /// <returns> Si se ha encontrado o no.</returns>
        public static bool NotifyActive(string gamekey)
        {
            ServerGameInfo serverGameInfo;
            bool gameFinded;

            serverGameInfo = publicGamesServerInfo.Find(item => item.basicGameInfo.gameKey == gamekey);
            if (serverGameInfo == null)
            {
                serverGameInfo = privateGamesServerInfo.Find(item => item.basicGameInfo.gameKey == gamekey);
            }

            gameFinded = serverGameInfo != null;
            if (gameFinded)
            {
                serverGameInfo.lastNoticeFromHost = DateTime.UtcNow;
            }

            return gameFinded;
        }

        public static void AddGame(BasicGameInfo newGame)
        {
            try
            {
                if (newGame.isPublic)
                {
                    publicGames.Add(newGame);
                    publicGamesServerInfo.Add(new ServerGameInfo(newGame));
                }
                else
                {
                    privateGames.Add(newGame);
                    privateGamesServerInfo.Add(new ServerGameInfo(newGame));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveGame(ServerGameInfo serverGameInfo)
        {
            try
            {
                if (serverGameInfo.basicGameInfo.isPublic)
                {
                    publicGames.Remove(serverGameInfo.basicGameInfo);
                    publicGamesServerInfo.Remove(serverGameInfo);
                }
                else
                {
                    privateGames.Remove(serverGameInfo.basicGameInfo);
                    privateGamesServerInfo.Remove(serverGameInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool KeyExists(string gamekey)
        {
            if (!publicGames.Any(item => item.gameKey == gamekey))
            {
                return true;
            }
            else if (!privateGames.Any(item => item.gameKey == gamekey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static BasicGameInfo GetGameInfo(bool isPublic, string gameKey)
        {
            if (isPublic)
            {
                return GamesData.GetPublicGames().Where(x => x.gameKey == gameKey).FirstOrDefault();
            }
            else
            {
                return GamesData.GetPrivateGames().Where(x => x.gameKey == gameKey).FirstOrDefault();
            }
        }
    }
}
