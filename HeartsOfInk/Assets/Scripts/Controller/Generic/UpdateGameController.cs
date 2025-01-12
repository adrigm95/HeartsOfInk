using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnalyticsServer.Models;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.DataAccess;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using NETCoreServer.Models;
using NETCoreServer.Models.Out;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateGameController : MonoBehaviour
{
    public enum UpdateGameState { GettingInfo = 1, DownloadingUpdates = 2}

    [SerializeField]
    private SceneChangeController sceneChangeController;

    private WebServiceCaller<LogExceptionDto, bool> exceptionSender =
        new WebServiceCaller<LogExceptionDto, bool>();
    private WebServiceCaller<LogDto, bool> logSender =
        new WebServiceCaller<LogDto, bool>();
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
        try
        {
            if (state == UpdateGameState.DownloadingUpdates)
            {
                if (mapModels.Count > 0)
                {
                    MapModelHeader newMapModelHeader = mapModels.Dequeue();
                    MapModelHeader currentMapModelHeader = MapDAC.LoadMapHeader(newMapModelHeader.DefinitionName, GlobalConstants.RootPath);

                    Debug.Log($"Checking map version.");
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        LogManager.SendLog(logSender, $"Checking map version.");
                    }

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
                        if (Application.platform == RuntimePlatform.Android)
                        {
                            LogManager.SendLog(logSender, "Map skipped, current version equal or greatter");
                        }
                    }
                }
                else
                {
                    sceneChangeController.ChangeScene(SceneChangeController.Scenes.AcceptPolicy);
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex, string.Empty, SceneManager.GetActiveScene().name);
            Debug.LogException(ex);
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

                if (Application.platform == RuntimePlatform.Android)
                {
                    LogManager.SendLog(logSender, "No maps to update in server.");
                }

                sceneChangeController.ChangeScene(SceneChangeController.Scenes.AcceptPolicy);
            }
            else
            {
                mapModels = new Queue<MapModelHeader>(mapModelsList);
                Debug.Log($"Updating {mapModels.Count} maps from server.");

                if (Application.platform == RuntimePlatform.Android)
                {
                    LogManager.SendLog(logSender, $"Updating {mapModels.Count} maps from server.");
                }
            }

            state = UpdateGameState.DownloadingUpdates;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            sceneChangeController.ChangeScene(SceneChangeController.Scenes.AcceptPolicy);
        }
    }

    private async Task<List<MapModelHeader>> GetMapsToUpdate()
    {
        WebServiceCaller<List<string>, List<MapModelHeader>> wsCaller = new WebServiceCaller<List<string>, List<MapModelHeader>>();
        HOIResponseModel<List<MapModelHeader>> response = null;

        try
        {
            Debug.Log("Trying to get maps to update");

            if (Application.platform == RuntimePlatform.Android)
            {
                LogManager.SendLog(logSender, "Trying to get maps to update");
            }
            //Todo: Send ids from downloaded non-official maps.
            response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.POST, "api/MapList", new List<string>());

        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex, string.Empty, SceneManager.GetActiveScene().name);
            Debug.LogException(ex);
        }

        return response.serviceResponse;
    }

    private async Task<MapModelOut> GetMapToUpdate(MapModelHeader mapHeader)
    {
        WebServiceCaller<MapModelOut> wsCaller = new WebServiceCaller<MapModelOut>();
        HOIResponseModel<MapModelOut> response = null;

        try
        {
            Debug.Log($"Updating map {mapHeader.DisplayName} from server");
            if (Application.platform == RuntimePlatform.Android)
            {
                LogManager.SendLog(logSender, $"Updating map {mapHeader.DisplayName} from server");
            }

            response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.GET, $"api/map/{mapHeader.MapId}");
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex, string.Empty, SceneManager.GetActiveScene().name);
            Debug.LogException(ex);
        }

        return response.serviceResponse;
    }
}
