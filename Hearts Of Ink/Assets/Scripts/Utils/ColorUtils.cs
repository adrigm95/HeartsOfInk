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

        public static Color GetColorByString(string color)
        {
            try
            {
                string[] splittedString = color.Split(',');

                return BuildColorBase256(float.Parse(splittedString[0]), float.Parse(splittedString[1]), float.Parse(splittedString[2]));
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("Received empty color in GetColorByString, returning 100,100,100");
                return BuildColorBase256(100f, 100f, 100f);
            }
        }

        public static string GetStringByColor(Color color)
        {
            return Convert.ToString(color.r * 255) + "," + Convert.ToString(color.g * 255) + "," + Convert.ToString(color.b * 255);
        }

        public static Color NextColor(Color currentColor, List<string> availableColors)
        {
            string currentAsString = GetStringByColor(currentColor);
            int currentIndex = availableColors.FindIndex(item => item == currentAsString);
            int nextIndex = currentIndex + 1;

            try
            {
                if (currentIndex == -1 || nextIndex == availableColors.Count)
                {
                    Debug.Log($"Next color is first color in list {availableColors[0]}");
                    return GetColorByString(availableColors[0]);
                }
                else
                {
                    Debug.Log($"Next color: {availableColors[nextIndex]}");
                    return GetColorByString(availableColors[nextIndex]);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.LogError($"Error out of range, values: 'nextIndex' -> {nextIndex}; 'currentIndex' -> + {currentIndex}; availableColors -> {availableColors.Count}");
                Debug.LogException(ex);
                throw;
            }
        }
    }
}
