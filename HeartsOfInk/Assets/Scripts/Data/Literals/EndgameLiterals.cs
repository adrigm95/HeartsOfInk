using Rawgen.Literals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EndgameLiterals : LiteralsFactory
{
    private static EndgameLiterals singleton = null;

    private Literal victory;
    private Literal defeat;

    private EndgameLiterals(Language languageCode) : base(languageCode)
    {
        BuildVictory();
        BuildDefeat();
    }

    public static EndgameLiterals GetInstance(Language languageCode)
    {
        if (singleton == null)
        {
            singleton = new EndgameLiterals(languageCode);
        }

        return singleton;
    }

    private void BuildVictory()
    {
        victory = new Literal();
        victory.SetValue("Victory", Language.en);
        victory.SetValue("Victoria", Language.es);
        victory.SetValue("Victoria", Language.es_ES);
        victory.SetValue("Victoria", Language.ca);
    }

    private void BuildDefeat()
    {
        defeat = new Literal();
        defeat.SetValue("Defeat", Language.en);
        defeat.SetValue("Derrota", Language.es);
        defeat.SetValue("Derrota", Language.es_ES);
        defeat.SetValue("Derrota", Language.ca);
    }

    public string Victory { get => victory.GetValue(languageCode); }
    public string Defeat { get => defeat.GetValue(languageCode); }

}
