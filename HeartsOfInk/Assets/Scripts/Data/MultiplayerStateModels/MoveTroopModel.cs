namespace Assets.Scripts.Data.MultiplayerStateModels
{
    public class MoveTroopModel
    {
        /// <summary>
        /// Tiempo desde el inicio de la partida en el cliente.
        /// </summary>
        public float TimeSinceStart { get; set; }
        /// <summary>
        /// Nombre de la tropa
        /// </summary>
        public string TroopName { get; set; }
        /// <summary>
        /// Posición de la tropa
        /// </summary>
        public string Position { get; set; }
    }
}