using Assets.Scripts.Data.Constants;
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
            // Todo SEPT-23-004
        }
        else if (globalLogic.IsMultiplayerClient)
        {
            
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
    public void SetCityOwner(Player owner)
    {
        //globalLogic.gameModel
        //Todo (SEPT-23-006): Si es multiplayer, actualizar owner en el stateHolder
        throw new NotImplementedException();
    }

    public Player GetCityOwner(string cityName)
    {
        Player owner = null;

        //Todo (SEPT-23-006): Añadir lógica en la que se actualiza el estado de la ciudad a partir del StateHolder

        throw new NotImplementedException();
    }

    public void SetTroopState(int size, Vector3 position)
    {
        //TODO: SEPT-23-007
        throw new NotImplementedException();
    }

    public TroopStateModel GetTroopState()
    {
        //TODO: SEPT-23-007
        throw new NotImplementedException();
    }
}
