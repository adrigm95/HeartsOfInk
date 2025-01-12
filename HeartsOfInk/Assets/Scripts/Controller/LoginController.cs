using Assets.Scripts.Data.Constants;
using Assets.Scripts.DataAccess;
using LobbyHOIServer.Models.MapModels;
using LobbyHOIServer.Models.Models;
using NETCoreServer.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    private WebServiceCallerReusable<LoginModelIn, LoginModelOut> serviceCaller;

    [SerializeField]
    private SceneChangeController sceneChangeController;

    // Start is called before the first frame update
    void Start()
    {
        serviceCaller = new WebServiceCallerReusable<LoginModelIn, LoginModelOut>(ApiConfig.LobbyHOIServerUrl);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void Login()
    {
        string userName = "raulAdmin"; //Todo: Get from scene.
        string password = "raulAdmin"; //Todo: Get from scene.
        string hashedPassword;
        LoginModelIn loginModelIn;
        HOIResponseModel<LoginModelOut> response;

        hashedPassword = SecurityLogic.HashPassword(password);

        loginModelIn = new LoginModelIn()
        {
            Nickname = userName,
            Password = hashedPassword
        };

        response = await serviceCaller.GenericWebServiceCaller(Method.POST, "api/Login", loginModelIn);
        
        if (response.internalResultCode == InternalStatusCodes.OKCode)
        {
            SecurityLogic.SaveUserSesion(userName, hashedPassword);
            sceneChangeController.ChangeToPreviousScene();
        }
        else if (response.internalResultCode == InternalStatusCodes.WrongPassword)
        {
            //Todo: Login for worng password
            Debug.LogWarning("Wrong password!!!");
        }
        else
        {
            Debug.LogWarning("Unexpected result!!!");
        }
    }
}
