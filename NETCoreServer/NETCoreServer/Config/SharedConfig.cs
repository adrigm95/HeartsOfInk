using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Config
{
    public static class SharedConfig
    {
        /// <summary>
        /// Tiempo en segundos que tarda el servidor en eliminar una partida de la lista si no recibe información de esta.
        /// </summary>
        public const float NoNoticeTimeout = 1000;
    }
}
