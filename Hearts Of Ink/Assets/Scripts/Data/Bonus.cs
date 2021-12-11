using System;
using Rawgen.Literals;

namespace Assets.Scripts.Data
{
    public class Bonus
    {
        public enum Id { None = -1, Recruitment, Combat, Speed, MoreUnitsAtStart }

        public Id BonusId { get; }
        public string bonusLiteral { get; }

        public Bonus(LiteralsFactory.Language languageCode, Id bonusId)
        {
            BonusId = bonusId;
            bonusLiteral = BonusLiterals.GetInstance(languageCode).GetBonusLiteralById(bonusId);
        }
    }
}
