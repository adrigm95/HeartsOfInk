using UnityEngine;

namespace Assets.Scripts.Data
{
    public class Player
    {
        public enum IA { PLAYER = 0, IA = 1, NEUTRAL = 2, OTHER_PLAYER = 3}

        public Faction Faction { get; set; }
        public IA IaId { get; set; }

        /// <summary>
        /// Identificador unico de jugador, sirve como id.
        /// </summary>
        public string Name { get; set; }
        public Color Color { get; set; }
        public byte MapSocketId { get; set; }

        public Player()
        {
            Faction = new Faction();
        }
    }
}
