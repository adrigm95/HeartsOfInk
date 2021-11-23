using Rawgen.Literals;

public class BonusLiterals : LiteralsFactory
{
    private static BonusLiterals singleton = null;

    private Literal bonusCombat;
    private Literal bonusRecruitment;
    private Literal bonusSpeed;
    private Literal moreUnitsAtStart;

    private BonusLiterals(Language languageCode) : base(languageCode)
    {
        BuildBonusCombat();
        BuildBonusRecruitment();
        BuildBonusSpeed();
        BuildMoreUnitsAtStart();
    }

    public static BonusLiterals GetInstance(Language languageCode)
    {
        if (singleton == null)
        {
            singleton = new BonusLiterals(languageCode);
        }

        return singleton;
    }

    private void BuildBonusCombat()
    {
        bonusCombat = new Literal();
        bonusCombat.SetValue("Your units are strong in combat than other factions", Language.en);
        bonusCombat.SetValue("Tus unidades son más fuertes en combate que las de otras facciones", Language.es);
        bonusCombat.SetValue("Tus unidades son más fuertes en combate que las de otras facciones", Language.es_ES);
        bonusCombat.SetValue("Les teues unitats son mes fortes al combat que les de altres faccions", Language.ca);
    }

    private void BuildBonusRecruitment()
    {
        bonusRecruitment = new Literal();
        bonusRecruitment.SetValue("New troops have 90 units instead of 80", Language.en);
        bonusRecruitment.SetValue("Las nuevas tropas tienen 90 unidades en vez de 80", Language.es);
        bonusRecruitment.SetValue("Las nuevas tropas tienen 90 unidades en vez de 80", Language.es_ES);
        bonusRecruitment.SetValue("Les noves tropes son de 90 unitats en lloc de 80", Language.ca);
    }

    private void BuildBonusSpeed()
    {
        bonusSpeed = new Literal();
        bonusSpeed.SetValue("Your units are faster than other factions", Language.en);
        bonusSpeed.SetValue("Tus unidades son más rápidas que las de otras facciones", Language.es);
        bonusSpeed.SetValue("Tus unidades son más rápidas que las de otras facciones", Language.es_ES);
        bonusSpeed.SetValue("Les teues unitats son mes rápides que les de altres faccions", Language.ca);
    }

    private void BuildMoreUnitsAtStart()
    {
        moreUnitsAtStart = new Literal();
        moreUnitsAtStart.SetValue("Your faction have more units at start than other factions", Language.en);
        moreUnitsAtStart.SetValue("Tu facción tiene más unidades al inicio que otras facciones", Language.es);
        moreUnitsAtStart.SetValue("Tu facción tiene más unidades al inicio que otras facciones", Language.es_ES);
        moreUnitsAtStart.SetValue("La teua facció te mes unitats al inici que altres faccions", Language.ca);
    }

    public string BonusCombat { get => bonusCombat.GetValue(languageCode); }
    public string BonusRecruitment { get => bonusRecruitment.GetValue(languageCode); }
    public string BonusSpeed { get => bonusSpeed.GetValue(languageCode); }
    public string MoreUnitsAtStart { get => moreUnitsAtStart.GetValue(languageCode); }
}
