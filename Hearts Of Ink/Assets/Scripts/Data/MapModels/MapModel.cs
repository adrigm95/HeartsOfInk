using System;
using System.Collections.Generic;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class MapModel
    {
        /// <summary>
        /// Identificador del mapa dentro del juego. También aplica al servidor.
        /// </summary>
        public string MapId { get; set; }

        /// <summary>
        /// Nombre del fichero de definición del mapa. No contiene la extensión (.rgmd).
        /// </summary>
        public string DefinitionName { get; set; }

        /// <summary>
        /// Nombre de mapa a mostrar. 
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Ruta relativa a la carpeta de StreamingAssets
        /// </summary>
        public string SpritePath { get; set; }

        /// <summary>
        /// Indica si la configuración de mapa cargada se puede utilizar para partidas multijugador.
        /// </summary>
        public bool AvailableForMultiplayer { get; set; }

        /// <summary>
        /// Indica si la configuración de mapa cargada se puede utilizar para partidas singleplayer.
        /// </summary>
        public bool AvailableForSingleplayer { get; set; }

        public List<MapPlayerModel> Players { get; set; }

        public List<MapCityModel> Cities { get; set; }

        public List<MapTroopModel> Troops { get; set; }
    }
}
