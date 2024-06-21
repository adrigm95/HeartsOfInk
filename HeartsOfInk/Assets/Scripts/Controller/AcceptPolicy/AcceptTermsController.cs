using UnityEngine;
using Assets.Scripts.DataAccess;
public class AcceptTermsController : MonoBehaviour
{
    public SceneChangeController SceneChangeController;

    public void Start()
    {
        string acceptTermsDate = AcceptTermsDAC.LoadAcceptTerms();

        if (!string.IsNullOrWhiteSpace(acceptTermsDate))
        {
            SceneChangeController.DirectChangeScene(SceneChangeController.Scenes.MainMenu);
        }
    }

    public static void SaveTerms()
    {
        AcceptTermsDAC.SaveAcceptTerms();
    }
}
