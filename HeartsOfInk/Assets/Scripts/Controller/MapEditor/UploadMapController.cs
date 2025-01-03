using Assets.Scripts.Data.Constants;
using Assets.Scripts.DataAccess;
using LobbyHOIServer.Models.MapModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UploadMapController : MonoBehaviour
{
    [SerializeField]
    private EditorPanelController panelController;
    private WebServiceCallerReusable<MapModel, int> mapUploader;
    private string DummyToken = "insert here your dummy token or make a definitive implementation";

    private void Start()
    {
        mapUploader = new WebServiceCallerReusable<MapModel, int>(ApiConfig.LobbyHOIServerUrl);
    }

    public async void UploadMap()
    {
        //    // Todo: Utilizar un token no hardcodeado
        //    if (string.IsNullOrWhiteSpace(DummyToken))
        //    {
        //        // Todo: Proceso de login
        //        //Debug.LogWarning("Aquí habría que loguearse");
        //        SceneManager.LoadScene("Login");
        //        Debug.LogWarning("No se puede subir un mapa sin token");
        //    }

        //    else
        //    {
        //        mapUploader.AddAuthorizationToken(DummyToken);
        //        await mapUploader.GenericWebServiceCaller(Method.POST, "api/Map", panelController.MapModel);
        //    }
        //Lo dejo así para probar, lo anterior falla
        SceneManager.LoadScene("Login");
    }
}
