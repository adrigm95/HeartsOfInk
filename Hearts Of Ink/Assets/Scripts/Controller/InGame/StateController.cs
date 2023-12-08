using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.MultiplayerStateModels;
using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.DataAccess;
using NETCoreServer.Models;
using NETCoreServer.Models.In;
using NETCoreServer.Models.Out;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

//Todo: SEPT-23-001 Completar modelo y lógica.

/// <summary>
/// Contiene la información actualizada del estado de ciudades y tropas en las partidas multiplayer. No se usa en las partidas singleplayer.
/// </summary>
public class StateController : MonoBehaviour
{
    /// <summary>
    /// Se usa en multiplayer par parte de los clientes (no host) para saber cual es la última tropa instanciada en la partida en local.
    /// </summary>
    public int LastTroopAdded { get; set; }

    private GameStateModel GameStateModel { get; set; }
    private GlobalLogicController globalLogic { get; set; }
    private float lastStateUpdate = 0;

    // Start is called before the first frame update
    void Start()
    {
        globalLogic = FindObjectOfType<GlobalLogicController>();
        GameStateModel = new GameStateModel();
        GameStateModel.citiesStates = new Dictionary<string, CityStateModel>();
        GameStateModel.troopsStates = new Dictionary<string, TroopStateModel>();
        GameStateModel.gamekey = globalLogic?.gameModel?.GameKey;
        GameStateModel.timeSinceStart = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (globalLogic.IsMultiplayerHost)
        {
            SendStateGame();
        }
        else if (globalLogic.IsMultiplayerClient)
        {
            GetStateGame();
        }
    }

    public async void GetStateGame()
    {
        HOIResponseModel<GameStateModel> response;
        WebServiceCaller<GameStateModel> wsCaller;

        if (GetIfUpdateStateRequired())
        {
            Debug.Log($"lastStateUpdate: {lastStateUpdate}; realTime: {Time.realtimeSinceStartup}");
            lastStateUpdate = Time.realtimeSinceStartup;
            wsCaller = new WebServiceCaller<GameStateModel>();
            response = await wsCaller.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.GET, "api/StateGame");

            if (response.serviceResponse.timeSinceStart > GameStateModel.timeSinceStart)
            {
                GameStateModel = response.serviceResponse;
            }
        }
    }

    public async void SendStateGame()
    {
        WebServiceCaller<GameStateModel, bool> wsCaller;

        if (GameStateModel.gamekey == null)
        {
            GameStateModel.gamekey = globalLogic.gameModel.GameKey;
        }

        GameStateModel.timeSinceStart = Time.realtimeSinceStartup;

        if (GetIfUpdateStateRequired())
        {
            Debug.Log($"lastStateUpdate: {lastStateUpdate}; realTime: {Time.realtimeSinceStartup}");
            lastStateUpdate = Time.realtimeSinceStartup;
            wsCaller = new WebServiceCaller<GameStateModel, bool>();
            await wsCaller.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.POST, "api/StateGame", GameStateModel);
        }
    }

    private bool GetIfUpdateStateRequired()
    {
        return lastStateUpdate + (1 / ApiConfig.DelayBetweenStateUpdates) <= Time.realtimeSinceStartup;
    }

    public void SetCityOwner(string cityName, Player owner)
    {
        CityStateModel cityStateModel;

        if (GameStateModel.citiesStates.TryGetValue(cityName, out cityStateModel))
        {
            cityStateModel.Owner = owner.MapSocketId;
        }
        else
        {
            GameStateModel.citiesStates.Add(cityName, new CityStateModel()
            {
                Owner = owner.MapSocketId
            });
            Debug.LogWarning("City not finded at GetCityOwner: " + cityName);
        }
    }

    public Player GetCityOwner(string cityName)
    {
        CityStateModel cityStateModel;
        Player owner = null;

        if (GameStateModel.citiesStates.TryGetValue(cityName, out cityStateModel))
        {
            owner = globalLogic.gameModel.Players.Find(player => player.MapSocketId == cityStateModel.Owner);
        }
        else
        {
            Debug.LogWarning("City not finded at GetCityOwner: " + cityName);
        }

        return owner;
    }

    public void SetTroopState(string troopName, int size, Vector3 position)
    {
        TroopStateModel troopState;

        if (!GameStateModel.troopsStates.TryGetValue(troopName, out troopState))
        {
            troopState = new TroopStateModel();
            GameStateModel.troopsStates.Add(troopName, troopState);
            Debug.LogWarning("Owner not finded at SetTroopState: " + troopName);
        }

        troopState.SetPosition(position);
        troopState.size = size;
    }

    public TroopStateModel GetTroopState(string troopName)
    {
        TroopStateModel result;

        GameStateModel.troopsStates.TryGetValue(troopName, out result);

        return result;
    }
}
