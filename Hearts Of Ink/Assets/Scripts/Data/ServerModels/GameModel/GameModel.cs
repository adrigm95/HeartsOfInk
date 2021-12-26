using System.Collections.Generic;

namespace NETCoreServer.Models.GameModel
{
    /// <summary>
    /// Valores de configuración de la partida.
    /// </summary>
    public class GameModel
    {
        public enum GameType { Single, MultiplayerClient, MultiplayerHost}

        public List<Player> Players { get; set;}
        public string MapDefinitionPath { get; set; }
        public GameType Gametype { get; set; }

        public GameModel(string mapDefinitionPath)
        {
            Players = new List<Player>();
            MapDefinitionPath = mapDefinitionPath;
        }
    }
}
