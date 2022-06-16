namespace Assets.Scripts.Data
{
    public class MapPlayerModel
    {
        public byte MapSocketId { get; set; }
        public int FactionId { get; set; }
        public int IaId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsPlayable { get; set; }
    }
}