using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Models.In
{
    /// <summary>
    /// Modelo de información de entrada del servicio de creación de partida.
    /// </summary>
    public class CreateGameModelIn
    {
        public string name { get; set; }
        public bool isPublic { get; set; }
        public string playerName { get; set; }
        public string mapName { get; set; }
    }
}
