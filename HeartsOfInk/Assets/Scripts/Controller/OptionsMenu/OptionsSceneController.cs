using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.DataAccess;
using HeartsOfInk.SharedLogic;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionsSceneController : MonoBehaviour
{
    public Scrollbar musicVolumeScrollbar;
    public Scrollbar effectsVolumeScrollbar;
    public Toggle selectTroopLeftBtn;
    public Toggle moveAttackLeftBtn;
    public Toggle selectTroopRightBtn;
    public Toggle moveAttackRightBtn;

    public void Start()
    {
        OptionsModel optionsModel = OptionsSceneDAC.LoadOptionsPreferences();

        if (optionsModel == null)
        {
            Debug.LogWarning("No se ha podido cargar el fichero de opciones.");
        }
        else
        {
            LanguageManager.Language = optionsModel.Language;

            // TODO: Modificar los valores de la pantalla de opciones para que usen los del modelo de opciones.
        }
    }

    public void SaveDefaultOptions()
    {
        OptionsModel optionsPreferences = new OptionsModel();
        optionsPreferences.MoveAttackPref = OptionsModel.LeftRightEnum.Right;
        optionsPreferences.SelectTroopPref = OptionsModel.LeftRightEnum.Left;
        optionsPreferences.MusicPref = 1;
        optionsPreferences.SoundEffectsPref = 1;
        optionsPreferences.Language = LanguageManager.DefaultLanguage;
        OptionsSceneDAC.SaveOptionsPreferences(optionsPreferences);
    }

    public void SaveOptions()
    {
        OptionsModel optionsPreferences = new OptionsModel();
        OptionsSceneDAC.SaveOptionsPreferences(optionsPreferences);
    }

    public void CheckBtn(Toggle buttonSelected)
    {
        if (buttonSelected.isOn && buttonSelected == selectTroopLeftBtn)
        {
            
        }
    }
}
