using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AnalyticsServer.Models;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.DataAccess;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using LobbyHOIServer.Models.Models;
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
    private Queue<FileDto> instalationFilesQueue;
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
                    LogManager.SendLog(logSender, $"Checking map version.");

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
                else if (instalationFilesQueue.Count > 0)
                {
                    FileDto newFile = instalationFilesQueue.Dequeue();

                    Debug.LogWarning($"Overwriting instalation file without verify current file version.");
                    string fileContentbase64 = await GetFileContent(newFile);
                    byte[] fileContentBytes = Convert.FromBase64String(fileContentbase64);

                    File.WriteAllBytes(GlobalConstants.RootPath + "/" +  newFile.Path, fileContentBytes);
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
        List<FileDto> instalationFiles;

        try
        {
            mapModelsList = await GetMapsToUpdate();
            if (mapModelsList.Count == 0)
            {
                Debug.LogWarning("No maps to update in server.");
                LogManager.SendLog(logSender, "No maps to update in server.");
                sceneChangeController.ChangeScene(SceneChangeController.Scenes.AcceptPolicy);
            }
            else
            {
                mapModels = new Queue<MapModelHeader>(mapModelsList);
                Debug.Log($"Updating {mapModels.Count} maps from server.");
                LogManager.SendLog(logSender, $"Updating {mapModels.Count} maps from server.");
            }

            instalationFiles = await GetFilesToUpdate();
            if (instalationFiles.Count == 0)
            {
                Debug.LogWarning("No files to update in server.");
                LogManager.SendLog(logSender, "No files to update in server.");
                sceneChangeController.ChangeScene(SceneChangeController.Scenes.AcceptPolicy);
            }
            else
            {
                instalationFilesQueue = new Queue<FileDto>(instalationFiles);
                Debug.Log($"Updating {instalationFiles.Count} files from server.");
                LogManager.SendLog(logSender, $"Updating {instalationFiles.Count} files from server.");
            }

            state = UpdateGameState.DownloadingUpdates;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            LogManager.SendException(exceptionSender, ex, "UpdateGameController.GetPendingUpdates()", SceneManager.GetActiveScene().name);
            sceneChangeController.ChangeScene(SceneChangeController.Scenes.AcceptPolicy);
        }
    }

    private async Task<List<FileDto>> GetFilesToUpdate()
    {
        WebServiceCaller<List<FileDto>> wsCaller = new WebServiceCaller<List<FileDto>>();
        HOIResponseModel<List<FileDto>> response = null;

        try
        {
            response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.GET, "api/Files");
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex, string.Empty, SceneManager.GetActiveScene().name);
            Debug.LogException(ex);
        }

        return response.serviceResponse;
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

    private async Task<string> GetFileContent(FileDto fileDto)
    {
        WebServiceCaller<string> wsCaller = new WebServiceCaller<string>();
        HOIResponseModel<string> response = null;

        try
        {
            response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.GET, $"api/File/{fileDto.Id}");
            Debug.Log($"GetFileContentResponse[Code: {response.internalResultCode}, Response: {response.serviceResponse}, ServiceError: {response.ServiceError}");
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex, string.Empty, SceneManager.GetActiveScene().name);
            Debug.LogException(ex);
        }

        return response.serviceResponse;
    }
}
