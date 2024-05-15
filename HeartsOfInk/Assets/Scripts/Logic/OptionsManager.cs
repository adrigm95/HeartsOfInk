using Assets.Scripts.Data.Literals;
using Assets.Scripts.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

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
        optionsModel.Language = LanguageManager.DefaultLanguage;
        OptionsSceneDAC.SaveOptionsPreferences(optionsModel);
    }
}
