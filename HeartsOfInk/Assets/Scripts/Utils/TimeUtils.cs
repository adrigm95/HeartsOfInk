using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class TimeUtils
    {
        /// <summary>
        /// Velocidad por defecto de la partida en Unity.
        /// </summary>
        public const float DefaultTimeScale = 1f;

        /// <summary>
        /// Ponemos la velocidad del juego por defecto, previene que el scroll no funcione en caso de que se haya salido de una partida en pausa, lo cual deja el valor de Time.timeScale a 0.
        /// </summary>
        public static void SetTimeToDefualt()
        {
            Time.timeScale = DefaultTimeScale;
        }
    }
}
