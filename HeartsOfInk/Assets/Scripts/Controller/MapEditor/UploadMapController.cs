using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.Security;
using Assets.Scripts.DataAccess;
using LobbyHOIServer.Models.MapModels;
using UnityEngine;

public class UploadMapController : MonoBehaviour
{
    private WebServiceCallerReusable<MapModel, int> mapUploader;
    private UserSession userSession;

    [SerializeField]
    private EditorPanelController panelController;

    [SerializeField]
    private SceneChangeController sceneChangeController;

    private void Start()
    {
        mapUploader = new WebServiceCallerReusable<MapModel, int>(ApiConfig.LobbyHOIServerUrl);
        userSession = SecurityLogic.LoadUserSession();
    }

    public async void UploadMap()
    {
        if (userSession == null)
        {
            Debug.Log("User not logged, redirecting to Login scene");
            sceneChangeController.ChangeScene(SceneChangeController.Scenes.Login);
        }
        else
        {
            // This service doesn't work in server yet (12/01/2025)
            mapUploader.AddAuthorizationToken(userSession.Token);
            await mapUploader.GenericWebServiceCaller(Method.POST, "api/Map", panelController.MapModel);
        }
    }
}
