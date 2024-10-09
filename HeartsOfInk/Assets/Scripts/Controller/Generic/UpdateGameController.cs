using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.DataAccess;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using NETCoreServer.Models;
using NETCoreServer.Models.Out;
using UnityEngine;

public class UpdateGameController : MonoBehaviour
{
    public enum UpdateGameState { GettingInfo = 1, DownloadingUpdates = 2}

    [SerializeField]
    private SceneChangeController sceneChangeController;

    private Queue<MapModelHeader> mapModels;
    private UpdateGameState state = UpdateGameState.GettingInfo;

    // Start is called before the first frame update
    void Start()
    {
        GetPendingUpdates();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGame();
    }

    private async void UpdateGame()
    {
        if (state == UpdateGameState.DownloadingUpdates)
        {
            if (mapModels.Count > 0)
            {
                MapModelHeader newMapModelHeader = mapModels.Dequeue();
                MapModelHeader currentMapModelHeader = MapDAC.LoadMapHeader(newMapModelHeader.DefinitionName, GlobalConstants.RootPath);

                Debug.Log($"Checking map version.");
                if (currentMapModelHeader == null || currentMapModelHeader.Version == default || newMapModelHeader.Version > currentMapModelHeader.Version)
                {
                    MapModelOut newMapModel = await GetMapToUpdate(newMapModelHeader);
                    MapDAC.SaveMapHeader(newMapModelHeader, GlobalConstants.RootPath);
                    MapDAC.SaveMapDefinition(newMapModel.MapModel, GlobalConstants.RootPath);
                    MapSpriteDAC.SaveMapSprite(GlobalConstants.RootPath, newMapModel.MapModel.SpriteName, newMapModel.BackgroundImage);
                }
                else
                {
                    Debug.Log("Map skipped, current version equal or greatter");
                }
            }
            else
            {
                sceneChangeController.DirectChangeScene(SceneChangeController.Scenes.AcceptPolicy);
            }
        }
    }

    private async void GetPendingUpdates()
    {
        List<MapModelHeader> mapModelsList;

        try
        {
            mapModelsList = await GetMapsToUpdate();
            if (mapModelsList.Count == 0)
            {
                Debug.LogWarning("No maps to update in server.");
                sceneChangeController.DirectChangeScene(SceneChangeController.Scenes.AcceptPolicy);
            }
            else
            {
                mapModels = new Queue<MapModelHeader>(mapModelsList);
                Debug.Log($"Updating {mapModels.Count} maps from server.");
            }

            state = UpdateGameState.DownloadingUpdates;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            sceneChangeController.DirectChangeScene(SceneChangeController.Scenes.AcceptPolicy);
        }
    }

    private async Task<List<MapModelHeader>> GetMapsToUpdate()
    {
        Debug.Log("Trying to get maps to update");
        WebServiceCaller<List<string>, List<MapModelHeader>> wsCaller = new WebServiceCaller<List<string>, List<MapModelHeader>>();
        HOIResponseModel<List<MapModelHeader>> response;

        //Todo: Send ids from downloaded non-official maps.
        response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.POST, "api/MapList", new List<string>());

        return response.serviceResponse;
    }

    private async Task<MapModelOut> GetMapToUpdate(MapModelHeader mapHeader)
    {
        Debug.Log($"Updating map {mapHeader.DisplayName} from server");
        WebServiceCaller<MapModelOut> wsCaller = new WebServiceCaller<MapModelOut>();
        HOIResponseModel<MapModelOut> response;

        response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.GET, $"api/map/{mapHeader.MapId}");

        return response.serviceResponse;
    }
}
