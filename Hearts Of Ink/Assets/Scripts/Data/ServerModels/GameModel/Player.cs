namespace NETCoreServer.Models.GameModel
{
    public class Player
    {
        public const byte NoAlliance = 0;
        public enum IA { PLAYER = 0, IA = 1, NEUTRAL = 2, OTHER_PLAYER = 3}

        public Faction Faction { get; set; }
        public IA IaId { get; set; }

        /// <summary>
        /// Identificador unico de jugador, sirve como id.
        /// </summary>
        public string Name { get; set; }
        public string Color { get; set; }
        public byte MapSocketId { get; set; }

        /// <summary>
        /// Indica el número de alianza al que pertenece el jugador.
        /// </summary>
        public byte Alliance { get; set; }

        public Player()
        {
            Faction = new Faction();
            Alliance = NoAlliance;
        }
    }
}
