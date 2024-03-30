using Rawgen.Literals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FactionNames : LiteralsFactory
{
    private static FactionNames singleton = null;

    private Literal governmentName;
    private Literal rebelsName;
    private Literal vukisName;
    private Literal nomadsName;

    private FactionNames(Language languageCode) : base(languageCode)
    {
        BuildGovernmentName();
        BuildRebelsName();
        BuildVukisName();
        BuildNomadsName();
    }

    public static FactionNames GetInstance(Language languageCode)
    {
        if (singleton == null)
        {
            singleton = new FactionNames(languageCode);
        }

        return singleton;
    }

    private void BuildGovernmentName()
    {
        governmentName = new Literal();
        governmentName.SetValue("Government", Language.en);
        governmentName.SetValue("Gobierno", Language.es);
        governmentName.SetValue("Gobierno", Language.es_ES);
        governmentName.SetValue("Govern", Language.ca);
    }

    private void BuildRebelsName()
    {
        rebelsName = new Literal();
        rebelsName.SetValue("Rebels", Language.en);
        rebelsName.SetValue("Rebeldes", Language.es);
        rebelsName.SetValue("Rebeldes", Language.es_ES);
        rebelsName.SetValue("Rebelds", Language.ca);
    }

    private void BuildVukisName()
    {
        vukisName = new Literal();
        vukisName.SetValue("Vookies", Language.en);
        vukisName.SetValue("Vukis", Language.es);
        vukisName.SetValue("Vukis", Language.es_ES);
        vukisName.SetValue("Vuquis", Language.ca);
    }

    private void BuildNomadsName()
    {
        nomadsName = new Literal();
        nomadsName.SetValue("Nomads", Language.en);
        nomadsName.SetValue("Nomadas", Language.es);
        nomadsName.SetValue("Nomadas", Language.es_ES);
        nomadsName.SetValue("Nomades", Language.ca);
    }

    public string GovernmentName { get => governmentName.GetValue(languageCode); }
    public string RebelsName { get => rebelsName.GetValue(languageCode); }
    public string VukisName { get => vukisName.GetValue(languageCode); }
    public string NomadsName { get => nomadsName.GetValue(languageCode); }

}
