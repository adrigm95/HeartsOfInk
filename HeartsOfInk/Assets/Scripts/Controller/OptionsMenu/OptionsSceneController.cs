using Assets.Scripts.Data.GlobalInfo;
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

    // Start is called before the first frame update
    //void Start()
    //{

    //}
    public static OptionsModel LoadOptionsPreferences()
    {
        string optionsPreferencesPath = Application.streamingAssetsPath + OptionsModel.PreferencesInfoFile;

        return JsonCustomUtils<OptionsModel>.ReadObjectFromFile(optionsPreferencesPath);
    }
    public void SaveOptions()
    {
        OptionsModel optionsPreferences
        SaveOptionsPreferences();
    }
    public void SaveOptionsPreferences(OptionsModel optionsPreferences)
    {
        string optionsPreferencesPath = Application.streamingAssetsPath + OptionsModel.PreferencesInfoFile;

        JsonCustomUtils<OptionsModel>.SaveObjectIntoFile(optionsPreferences, optionsPreferencesPath);
    }
    public void CheckBtn(Toggle buttonSelected)
    {
        if (buttonSelected.isOn && buttonSelected == selectTroopLeftBtn)
        {
            
        } 
        else if () 
        {

        }
    }
    // Update is called once per frame
    //void Update()
    //{

    //}
}
