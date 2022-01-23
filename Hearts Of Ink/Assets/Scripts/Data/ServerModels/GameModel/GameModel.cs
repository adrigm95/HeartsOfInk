using System.Collections.Generic;

namespace NETCoreServer.Models
{
    /// <summary>
    /// Valores de configuración de la partida.
    /// </summary>
    public class GameModel
    {
        public enum GameType { Single, MultiplayerClient, MultiplayerHost}

        public List<Player> Players { get; set;}
        public string MapName { get; set; }
        public GameType Gametype { get; set; }

        public GameModel(string mapName)
        {
            Players = new List<Player>();
            MapName = mapName;
        }
    }
}
