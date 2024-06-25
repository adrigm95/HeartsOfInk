using System.Collections.Generic;

namespace Assets.Scripts.Data.MultiplayerStateModels
{
    public class GameStateModel
    {
        /// <summary>
        /// Tiempo desde el inicio de la partida en el host.
        /// </summary>
        public float TimeSinceStart { get; set; }

        /// <summary>
        /// Clave de partida.
        /// </summary>
        public string Gamekey { get; set; }

        /// <summary>
        /// Listado con el estado de las distintas ciudades
        /// </summary>
        public Dictionary<string, CityStateModel> CitiesStates { get; set; }

        /// <summary>
        /// Listado con el estado de las distintas troopsState
        /// </summary>
        public Dictionary<string, TroopStateModel> TroopsStates { get; set; }
    }
}
