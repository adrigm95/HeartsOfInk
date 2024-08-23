using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.DataAccess;
using LobbyHOIServer.Models.MapModels;
using NETCoreServer.Models;
using UnityEngine;

public class UpdateGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void UpdateGame()
    {
        List<String> mapList = new List<String>();
        WebServiceCaller<List<string>, List<MapModelHeader>> wsCaller = new WebServiceCaller<List<string>, List<MapModelHeader>>();
        HOIResponseModel<List<MapModelHeader>> response;

        response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.GET,"api/MapList", mapList);

        Debug.Log(response.internalResultCode);
    }
   /*  private async void LoadGames()
    {
        WebServiceCaller<List<BasicGameInfo>> wsCaller = new WebServiceCaller<List<BasicGameInfo>>();
        HOIResponseModel<List<BasicGameInfo>> response;

        response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.GET, LobbyHOIControllers.PublicGames);

        switch (response.internalResultCode)
        {
            case InternalStatusCodes.OKCode:
                PopulateList(response);
                break;
            case InternalStatusCodes.KOConnectionCode:
                infoPanelController.DisplayMessage("Connection error", "Error when try to connect to server.");
                break;
        }
    } */


}
