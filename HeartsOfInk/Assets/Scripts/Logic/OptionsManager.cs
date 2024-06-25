using Assets.Scripts.Data.Literals;
using Assets.Scripts.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionsManager
{
    // Singleton variables
    private static readonly Lazy<OptionsManager> _singletonReference = new(() => new OptionsManager());
    public static OptionsManager Instance => _singletonReference.Value;

    private OptionsModel optionsModel;

    public OptionsModel OptionsModel { get { return optionsModel; } }

    private OptionsManager()
    {
        optionsModel = OptionsSceneDAC.LoadOptionsPreferences();

        if (optionsModel == null)
        {
            SaveDefaultOptions();
        }
    }

    private void SaveDefaultOptions()
    {
        optionsModel = new OptionsModel();
        optionsModel.MoveAttackPref = OptionsModel.LeftRightEnum.Right;
        optionsModel.SelectTroopPref = OptionsModel.LeftRightEnum.Left;
        optionsModel.MusicPref = 1;
        optionsModel.SoundEffectsPref = 1;
        optionsModel.Language = GetDefaultLanguage();
        OptionsSceneDAC.SaveOptionsPreferences(optionsModel);
    }

    private string GetDefaultLanguage()
    {
        Debug.Log("Application.systemLanguage" + Application.systemLanguage);

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Spanish:
                return LanguageConstants.Spanish_Spain;
            case SystemLanguage.English:
                return LanguageConstants.English;
            case SystemLanguage.Catalan:
                return LanguageConstants.Catalonian;
            default:
                return LanguageManager.DefaultLanguage;
        } 
    }
}
