﻿using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.MultiplayerStateModels;
using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.DataAccess;
using NETCoreServer.Models;
using NETCoreServer.Models.In;
using NETCoreServer.Models.Out;
using System;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        globalLogic = FindObjectOfType<GlobalLogicController>();
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
        WebServiceCaller<GameStateModel> wsCaller = new WebServiceCaller<GameStateModel>();
        response = await wsCaller.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.GET, "api/StateGame");

        if (response.serviceResponse.timeSinceStart > GameStateModel.timeSinceStart)
        {
            GameStateModel = response.serviceResponse;
             //TODO
        }
    }

    public async void SendStateGame()
    {
        HOIResponseModel<bool> response;
        WebServiceCaller<GameStateModel, bool> wsCaller = new WebServiceCaller<GameStateModel, bool>();
        response = await wsCaller.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.POST, "api/StateGame", GameStateModel);
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