namespace Assets.Scripts.Data
{
    public class MapCityModel
    {
        public string Name { get; set; }
        public float[] Position { get; set; }
        public int Type { get; set; }
        public byte MapSocketId { get; set; }

        public override string ToString()
        {
            return $"'Name': {Name}, 'Position': {Position}, 'Type': {Type}, 'MapSocketId': {MapSocketId}";
        }
    }
}