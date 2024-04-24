namespace Assets.Scripts.Data.MultiplayerStateModels
{
    public class AttackTroopModel
    {
        /// <summary>
        /// Tiempo desde el inicio de la partida en el cliente.
        /// </summary>
        public float TimeSinceStart { get; set; }
        /// <summary>
        /// Tropa atacante
        /// </summary>
        public string Attacker { get; set; }

        /// <summary>
        /// Tropa atacada
        /// </summary>
        public string Attacked { get; set; }
    }
}