using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class ColorUtils
    {
        /// <summary>
        /// Convierte un color RGB con valores 0 a 255 a un color de Unity, que utiliza valores de 0 a 1
        /// </summary>
        /// <param name="r"> Rojo. Valor entre 0 y 255.</param>
        /// <param name="g"> Verde. Valor entre 0 y 255.</param>
        /// <param name="b"> Azul. Valor entre 0 y 255.</param>
        /// <returns></returns>
        public static Color BuildColorBase256(float r, float g, float b)
        {
            return new Color(r / 255, g / 255, b / 255);
        }
    }
}
