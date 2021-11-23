using System;
using Rawgen.Literals;

namespace Assets.Scripts.Data
{
    public class BonusData
    {
        public BonusLiterals bonusLiterals;
        public enum Id { None = -1, Recruitment, Combat, Speed, MoreUnitsAtStart }

        public BonusData(LiteralsFactory.Language languageCode)
        {
            bonusLiterals = BonusLiterals.GetInstance(languageCode);
        }

        public string GetBonusLiteralById(Id bonusId)
        {
            switch(bonusId)
            {
                case Id.MoreUnitsAtStart:
                    return bonusLiterals.MoreUnitsAtStart;
                case Id.None:
                    return string.Empty;
                case Id.Recruitment:
                    return bonusLiterals.BonusRecruitment;
                case Id.Combat:
                    return bonusLiterals.BonusCombat;
                case Id.Speed:
                    return bonusLiterals.BonusSpeed;
                default:
                    throw new Exception("Unexpected bonus id");
            };
        }
    }
}
