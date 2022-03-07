using Assets.Scripts.Data.TutorialModels;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private int step;
    private TutorialModel tutorialModel;
    public InfoPanelController infoPanelController;
    public GlobalLogicController globalLogic;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void DisplayTutorial()
    {
        TutorialStep tutorialStep;
        string tutorialPath = Application.streamingAssetsPath + "/tutorial.json";

        step = 0;
        tutorialModel = JsonCustomUtils<TutorialModel>.ReadObjectFromFile(tutorialPath);
        tutorialStep = tutorialModel.Steps[step];
        globalLogic.SetPauseState(tutorialStep.PauseGame, null);
        infoPanelController.DisplayDecision(this, tutorialStep.Title[0].Value, tutorialStep.Content[0].Value, false);
    }

    /// <summary>
    /// Obtiene los datos del siguiente mensaje a mostrar.
    /// </summary>
    /// <returns>True si hay que mostrar otro mensaje, false si se ha llegado al final de la cadena.</returns>
    public bool NextMessage()
    {
        TutorialStep tutorialStep;
        step++;

        if (step < tutorialModel.Steps.Count)
        {
            tutorialStep = tutorialModel.Steps[step];

            globalLogic.SetPauseState(tutorialStep.PauseGame, null);
            infoPanelController.DisplayMessage(tutorialStep.Title[0].Value, tutorialStep.Content[0].Value);
            return true;
        }
        else
        {
            globalLogic.SetPauseState(false, null);
            return false;
        }
    }

    public void DiscardTutorial()
    {
        //TODO: Guardar en preferencias que se omite el tutorial.
        globalLogic.SetPauseState(false, null);
    }
}
