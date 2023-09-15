using System.Collections.Generic;

namespace Assets.Scripts.Data.MultiplayerStateModels
{
    public class GameStateModel
    {
        /// <summary>
        /// Tiempo desde el inicio de la partida en el host.
        /// </summary>
        float timeSinceStart;

        /// <summary>
        /// Clave de partida.
        /// </summary>
        string gamekey;

        /// <summary>
        /// Listado con el estado de las distintas ciudades
        /// </summary>
        List<CityStateModel> citiesStates;

        /// <summary>
        /// Listado con el estado de las distintas troopsState
        /// </summary>
        List<TroopStateModel> troopsStates;
    }
}
