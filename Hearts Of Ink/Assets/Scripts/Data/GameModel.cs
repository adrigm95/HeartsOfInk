using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Valores de configuración de la partida.
    /// </summary>
    public class GameModel
    {
        private List<Faction> Factions;
        public string MapDefinitionPath;

        public GameModel(string mapDefinitionPath)
        {
            Factions = new List<Faction>();
            MapDefinitionPath = mapDefinitionPath;
        }

        public void AddFaction(Faction faction)
        {
            Factions.Add(faction);
        }
    }
}
