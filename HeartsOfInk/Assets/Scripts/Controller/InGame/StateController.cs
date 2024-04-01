using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.MultiplayerStateModels;
using Assets.Scripts.DataAccess;
using NETCoreServer.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private WebServiceCaller<StateGameModelIn, bool> wsCaller;
    public Text txtIsMultiplayer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start - State Controller");

        wsCaller = new WebServiceCaller<StateGameModelIn, bool>();
        globalLogic = FindObjectOfType<GlobalLogicController>();
        GameStateModel = new GameStateModel();
        GameStateModel.citiesStates = new Dictionary<string, CityStateModel>();
        GameStateModel.troopsStates = new Dictionary<string, TroopStateModel>();
        GameStateModel.gamekey = globalLogic?.gameModel?.GameKey;
        GameStateModel.timeSinceStart = Time.realtimeSinceStartup;

        Debug.Log("Start - Gamekey from globalLogic: " + globalLogic?.gameModel?.GameKey);
    }

    // Update is called once per frame
    void Update()
    {
        txtIsMultiplayer.text = "Is multiplayer host?: " + globalLogic.IsMultiplayerHost;
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
        string serviceParameters;

        if (UpdateStateRequired())
        {
            Debug.Log($"lastStateUpdate (as client): {lastStateUpdate}; realTime: {Time.realtimeSinceStartup}");
            lastStateUpdate = Time.realtimeSinceStartup;
            wsCaller = new WebServiceCaller<GameStateModel>();
            serviceParameters = $"playername={globalLogic.thisPcPlayer.Name}&gamekey={GameStateModel.gamekey}";
            response = await wsCaller.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.GET, "api/StateGame?" + serviceParameters);

            if (response.serviceResponse.timeSinceStart > GameStateModel.timeSinceStart)
            {
                GameStateModel = response.serviceResponse;
                Debug.Log("GetStateGame - updated state; cities: " + GameStateModel.citiesStates.Count + "; troops:" + GameStateModel.troopsStates.Count);

                foreach (var troop in GameStateModel.troopsStates)
                {
                    GameObject currentTroop = GameObject.Find(troop.Key);
                    if (currentTroop == null)
                    {
                        //globalLogic.InstantiateTroopSingleplayer(troop.Value.size, troop.Value.position, troop.Value.o);
                    }
                }
            }
            else
            {
                Debug.LogWarning("GetStateGame - Received state is previous than current; received: " + response.serviceResponse.timeSinceStart + " current: " + GameStateModel.timeSinceStart);
            }
        }
    }

    public async void SendStateGame()
    {
        if (UpdateStateRequired())
        {
            StateGameModelIn sgm = new StateGameModelIn()
            {
                playerName = globalLogic.thisPcPlayer.Name,
                gameStateModel = GameStateModel
            };

            if (GameStateModel.gamekey == null)
            {
                GameStateModel.gamekey = globalLogic.gameModel.GameKey;
            }

            GameStateModel.timeSinceStart = Time.realtimeSinceStartup;

            Debug.Log($"lastStateUpdate (as Host): {lastStateUpdate}; realTime: {Time.realtimeSinceStartup}");
            lastStateUpdate = Time.realtimeSinceStartup;
            await wsCaller.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.POST, "api/StateGame", sgm);
        }
    }

    private bool UpdateStateRequired()
    {
        return lastStateUpdate + ApiConfig.DelayBetweenStateUpdates < Time.realtimeSinceStartup;
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
