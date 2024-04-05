using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.MultiplayerStateModels;
using Assets.Scripts.DataAccess;
using NETCoreServer.Models;
using System.Collections.Generic;
using System.Linq;
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
    private WebServiceCaller<GameStateModelIn, bool> wsCaller;
    public Text txtIsMultiplayer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start - State Controller");

        wsCaller = new WebServiceCaller<GameStateModelIn, bool>();
        globalLogic = FindObjectOfType<GlobalLogicController>();
        GameStateModel = new GameStateModel();
        GameStateModel.CitiesStates = new Dictionary<string, CityStateModel>();
        GameStateModel.TroopsStates = new Dictionary<string, TroopStateModel>();
        GameStateModel.Gamekey = globalLogic?.gameModel?.GameKey;
        GameStateModel.TimeSinceStart = Time.realtimeSinceStartup;

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
            serviceParameters = $"playername={globalLogic.thisPcPlayer.Name}&gamekey={GameStateModel.Gamekey}";
            response = await wsCaller.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.GET, "api/StateGame?" + serviceParameters);

            if (response.serviceResponse.TimeSinceStart > GameStateModel.TimeSinceStart)
            {
                GameStateModel = response.serviceResponse;
                Debug.Log("GetStateGame - updated state; cities: " + GameStateModel.CitiesStates.Count + "; troops:" + GameStateModel.TroopsStates.Count);

                foreach (var city in GameStateModel.CitiesStates)
                {
                    GameObject currentCity = GameObject.Find(city.Key);
                    if (currentCity == null)
                    {

                    }
                    else
                    {
                        CityController cityController = currentCity.GetComponent<CityController>();
                        cityController.Owner = globalLogic.GetPlayer(city.Value.Owner);
                    }
                }

                foreach (var troop in GameStateModel.TroopsStates)
                {
                    GameObject currentTroop = GameObject.Find(troop.Key);
                    if (currentTroop == null)
                    {
                        globalLogic.InstantiateTroopMultiplayer(troop.Key, troop.Value.Size, troop.Value.GetPositionAsVector3(), troop.Value.Owner);
                    }
                    else
                    {
                        TroopController troopController = currentTroop.GetComponent<TroopController>();
                        troopController.troopModel.CurrentPosition = troop.Value.GetPositionAsVector3();
                        troopController.troopModel.Units = troop.Value.Size;
                        troopController.troopModel.Player = globalLogic.GetPlayer(troop.Value.Owner);
                    }
                }
            }
            else
            {
                Debug.LogWarning("GetStateGame - Received state is previous than current; received: " + response.serviceResponse.TimeSinceStart + " current: " + GameStateModel.TimeSinceStart);
            }
        }
    }

    public async void SendStateGame()
    {
        if (UpdateStateRequired())
        {
            GameStateModelIn sgm = new GameStateModelIn()
            {
                PlayerName = globalLogic.thisPcPlayer.Name,
                GameStateModel = GameStateModel
            };

            if (GameStateModel.Gamekey == null)
            {
                GameStateModel.Gamekey = globalLogic.gameModel.GameKey;
            }

            GameStateModel.TimeSinceStart = Time.realtimeSinceStartup;

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

        if (GameStateModel.CitiesStates.TryGetValue(cityName, out cityStateModel))
        {
            cityStateModel.Owner = owner.MapSocketId;
        }
        else
        {
            GameStateModel.CitiesStates.Add(cityName, new CityStateModel()
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

        if (GameStateModel.CitiesStates.TryGetValue(cityName, out cityStateModel))
        {
            owner = globalLogic.gameModel.Players.Find(player => player.MapSocketId == cityStateModel.Owner);
        }
        else
        {
            Debug.LogWarning("City not finded at GetCityOwner: " + cityName);
        }

        return owner;
    }

    public void SetTroopState(string troopName, int size, Vector3 position, Player player)
    {
        TroopStateModel troopState;

        if (!GameStateModel.TroopsStates.TryGetValue(troopName, out troopState))
        {
            troopState = new TroopStateModel()
            {
                Owner = player.MapSocketId,
                Size = size
            };
            troopState.SetPosition(position);

            GameStateModel.TroopsStates.Add(troopName, troopState);
            Debug.LogWarning("Owner not finded at SetTroopState: " + troopName);
        }

        troopState.SetPosition(position);
        troopState.Size = size;
    }

    public TroopStateModel GetTroopState(string troopName)
    {
        TroopStateModel result;

        GameStateModel.TroopsStates.TryGetValue(troopName, out result);

        return result;
    }
}
