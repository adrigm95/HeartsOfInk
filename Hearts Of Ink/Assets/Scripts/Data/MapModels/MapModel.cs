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
        public short MapId { get; set; }

        /// <summary>
        /// Ruta relativa a la carpeta de StreamingAssets
        /// </summary>
        public string SpritePath { get; set; }

        public List<MapPlayerModel> Players { get; set; }

        public List<MapCityModel> Cities { get; set; }

        public List<MapTroopModel> Troops { get; set; }
    }
}
