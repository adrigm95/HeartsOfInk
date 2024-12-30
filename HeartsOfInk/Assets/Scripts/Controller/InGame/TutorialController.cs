using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.TutorialModels;
using Assets.Scripts.Utils;
using HeartsOfInk.SharedLogic;
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
        string tutorialPath = Application.persistentDataPath + "/tutorial.json";

        step = 0;
        tutorialModel = JsonCustomUtils<TutorialModel>.ReadObjectFromFile(tutorialPath);
        tutorialStep = tutorialModel.Steps[step];
        globalLogic.ChangeSpeed(tutorialStep.PauseGame ? GameSpeedConstants.PauseSpeed : GameSpeedConstants.PlaySpeed);
        infoPanelController.DisplayDecision(this, tutorialStep.Title[0].Value, tutorialStep.Content[0].Value, false);
    }

    /// <summary>
    /// Obtiene los datos del siguiente mensaje a mostrar.
    /// </summary>
    /// <returns>True si hay que mostrar otro mensaje, false si se ha llegado al final de la cadena.</returns>
    public TutorialStep NextMessage()
    {
        TutorialStep tutorialStep;
        step++;

        Debug.Log($"Displaying step {step} of {tutorialModel.Steps.Count}");
        if (step < tutorialModel.Steps.Count)
        {
            tutorialStep = tutorialModel.Steps[step];
            globalLogic.ChangeSpeed(tutorialStep.PauseGame ? GameSpeedConstants.PauseSpeed : GameSpeedConstants.PlaySpeed);

            return tutorialStep;
        }
        else
        {
            globalLogic.ChangeSpeed(GameSpeedConstants.PlaySpeed);
            return null;
        }
    }

    public void DiscardTutorial()
    {
        //TODO: Guardar en preferencias que se omite el tutorial.
        globalLogic.ChangeSpeed(GameSpeedConstants.PlaySpeed);
    }
}
