namespace Assets.Scripts.Data.MultiplayerStateModels
{
    public class CityStateModel
    {
        /// <summary>
        /// Identificador del dueño actual de la ciudad. Se corresponde con el mapSocketId de Player.
        /// </summary>
        public byte Owner { get; set; }
    }
}
