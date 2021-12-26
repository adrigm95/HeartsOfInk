namespace NETCoreServer.Models.GameModel
{
    public class Bonus
    {
        public enum Id { None = -1, Recruitment, Combat, Speed, MoreUnitsAtStart }

        public Id BonusId { get; }

        public Bonus(Id bonusId)
        {
            BonusId = bonusId;
        }
    }
}
