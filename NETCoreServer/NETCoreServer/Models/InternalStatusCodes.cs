using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Models
{
    public static class InternalStatusCodes
    {
        public const short OKCode = 600;

        /// <summary>
        /// Error inesperado. 
        /// </summary>
        public const short KOCode = 699;

        /// <summary>
        ///  La partida buscada no está registrada en el servidor.
        /// </summary>
        public const short GameNotFind = 701;

        /// <summary>
        /// La partida buscada existe pero está completa. 
        /// </summary>        
        public const short GameComplete = 702;

        /// <summary>
        /// El servidor está al máximo de su capacidad.
        /// </summary>        
        public const short ServerFull = 704;
    }
}
