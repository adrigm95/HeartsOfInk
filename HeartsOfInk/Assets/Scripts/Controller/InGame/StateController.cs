using AnalyticsServer.Models;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.MultiplayerStateModels;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
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

    private WebServiceCaller<LogExceptionDto, bool> exceptionSender = new WebServiceCaller<LogExceptionDto, bool>();

    private GameStateModel GameStateModel { get; set; }
    private GlobalLogicController globalLogic { get; set; }
    private float lastStateUpdate = 0;
    private WebServiceCallerReusable<GameStateModelIn, bool> wsCallerSend;
    private WebServiceCallerReusable<GameStateModel> wsCallerReceive;
    private Dictionary<string, AttackTroopModel> attackTroopOrders;
    private Dictionary<string, MoveTroopModel> moveTroopOrders;
    public Text txtIsMultiplayer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start - State Controller");

        wsCallerSend = new WebServiceCallerReusable<GameStateModelIn, bool>(ApiConfig.IngameServerUrl);
        wsCallerReceive = new WebServiceCallerReusable<GameStateModel>(ApiConfig.IngameServerUrl);
        globalLogic = FindObjectOfType<GlobalLogicController>();
        GameStateModel = new GameStateModel();
        GameStateModel.CitiesStates = new Dictionary<string, CityStateModel>();
        GameStateModel.TroopsStates = new Dictionary<string, TroopStateModel>();
        GameStateModel.Gamekey = globalLogic?.gameModel?.GameKey;
        GameStateModel.TimeSinceStart = Time.realtimeSinceStartup;

        attackTroopOrders = new Dictionary<string, AttackTroopModel>();
        moveTroopOrders = new Dictionary<string, MoveTroopModel>();

        Debug.Log("Start - Gamekey from globalLogic: " + globalLogic?.gameModel?.GameKey);
    }

    // Update is called once per frame
    void Update()
    {
        txtIsMultiplayer.text = "Is multiplayer host?: " + globalLogic.IsMultiplayerHost;
        if (globalLogic.IsMultiplayerHost)
        {
            SendStateGame();
            ReceiveAttackOrders();
            ReceiveMoveOrders();
        }
        else if (globalLogic.IsMultiplayerClient)
        {
            GetStateGame();
        }
    }

    public async void GetStateGame()
    {
        HOIResponseModel<GameStateModel> response;
        string serviceParameters;

        if (UpdateStateRequired())
        {
            Debug.Log($"lastStateUpdate (as client): {lastStateUpdate}; realTime: {Time.realtimeSinceStartup}");
            lastStateUpdate = Time.realtimeSinceStartup;
            serviceParameters = $"playername={globalLogic.thisPcPlayer.Name}&gamekey={GameStateModel.Gamekey}";
            response = await wsCallerReceive.GenericWebServiceCaller(Method.GET, "api/StateGame?" + serviceParameters);

            if (response.serviceResponse.TimeSinceStart > GameStateModel.TimeSinceStart)
            {
                //UpdateStateModelV1(response);
                UpdateStateModelV2(response);
            }
            else
            {
                Debug.LogWarning("GetStateGame - Received state is previous than current; received: " + response.serviceResponse.TimeSinceStart + " current: " + GameStateModel.TimeSinceStart);
            }
        }
    }

    private void UpdateStateModelV2(HOIResponseModel<GameStateModel> response)
    {
        Dictionary<string, TroopStateModel> updatedTroops = new Dictionary<string, TroopStateModel>();
        GameStateModel = response.serviceResponse;

        // Actualizar ciudades.
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(Tags.City))
        {
            CityStateModel city;

            if (GameStateModel.CitiesStates.TryGetValue(gameObject.name, out city))
            {
                CityController cityController = gameObject.GetComponent<CityController>();
                cityController.Owner = globalLogic.GetPlayer(city.Owner);
            }
        }

        // Actualizar tropas y eliminar las que ya no estén en el modelo.
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(Tags.Troop))
        {
            TroopStateModel troop;

            if (GameStateModel.TroopsStates.TryGetValue(gameObject.name, out troop))
            {
                TroopController troopController = gameObject.GetComponent<TroopController>();
                updatedTroops.Add(gameObject.name, troop);
                troopController.troopModel.CurrentPosition = troop.GetPositionAsVector3();
                troopController.troopModel.Units = troop.Size;
                troopController.troopModel.Player = globalLogic.GetPlayer(troop.Owner);
            }
            else
            {
                globalLogic.DestroyUnit(gameObject, null);
            }
        }

        // Añadir las tropas nuevas.
        foreach (var troop in GameStateModel.TroopsStates)
        {
            if (!updatedTroops.ContainsKey(troop.Key))
            {
                globalLogic.InstantiateTroopMultiplayer(troop.Key, troop.Value.Size, troop.Value.GetPositionAsVector3(), troop.Value.Owner);
            }
        }
    }

    [Obsolete("Obsoleto, comprobar si funciona el V2 y si es así borrar este método.")]
    private void UpdateStateModelV1(HOIResponseModel<GameStateModel> response)
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

    public void TroopDistroyed(TroopController troop)
    {
        GameStateModel.TroopsStates.Remove(troop.transform.name);
    }

    public async void SendStateGame()
    {
        if (UpdateStateRequired() && !wsCallerSend.MakingCall)
        {
            GameStateModelIn stateModel = new GameStateModelIn()
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
            await wsCallerSend.GenericWebServiceCaller(Method.POST, "api/StateGame", stateModel);
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

    private void ReceiveMoveOrders()
    {
        foreach (MoveTroopModel moveTroopModel in MoveTroopSignalR.Instance.GetMoveTroopReceived())
        {
            ReceiveMoveOrder(moveTroopModel);
        }
    }

    public void ReceiveMoveOrder(MoveTroopModel moveTroopModel)
    {
        AttackTroopModel currentAttackOrder;
        MoveTroopModel currentMoveOrder;
        bool haveAttackOrder;
        bool haveMoveOrder;

        try
        {
            haveAttackOrder = attackTroopOrders.TryGetValue(moveTroopModel.TroopName, out currentAttackOrder);
            haveMoveOrder = moveTroopOrders.TryGetValue(moveTroopModel.TroopName, out currentMoveOrder);

            if (haveAttackOrder)
            {
                if (currentAttackOrder.TimeSinceStart < moveTroopModel.TimeSinceStart)
                {
                    attackTroopOrders.Remove(moveTroopModel.TroopName);
                    moveTroopOrders.Add(moveTroopModel.TroopName, moveTroopModel);
                }
            }
            else if (haveMoveOrder)
            {
                if (currentMoveOrder.TimeSinceStart < moveTroopModel.TimeSinceStart)
                {
                    moveTroopOrders.Remove(moveTroopModel.TroopName);
                    moveTroopOrders.Add(moveTroopModel.TroopName, moveTroopModel);
                }
            }
            else
            {
                moveTroopOrders.Add(moveTroopModel.TroopName, moveTroopModel);
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
        }
    }

    private void ReceiveAttackOrders()
    {
        foreach (AttackTroopModel attackTroopModel in AttackTroopSignalR.Instance.GetAttackTroopReceived())
        {
            ReceiveAttackOrder(attackTroopModel);
        }
    }

    public void ReceiveAttackOrder(AttackTroopModel attackTroopModel)
    {
        AttackTroopModel currentAttackOrder;
        MoveTroopModel currentMoveOrder;
        bool haveAttackOrder;
        bool haveMoveOrder;

        try
        {
            haveAttackOrder = attackTroopOrders.TryGetValue(attackTroopModel.Attacker, out currentAttackOrder);
            haveMoveOrder = moveTroopOrders.TryGetValue(attackTroopModel.Attacker, out currentMoveOrder);

            if (haveAttackOrder)
            {
                if (currentAttackOrder.TimeSinceStart < attackTroopModel.TimeSinceStart)
                {
                    attackTroopOrders.Remove(attackTroopModel.Attacker);
                    attackTroopOrders.Add(attackTroopModel.Attacker, attackTroopModel);
                }
            }
            else if (haveMoveOrder)
            {
                if (currentMoveOrder.TimeSinceStart < attackTroopModel.TimeSinceStart)
                {
                    moveTroopOrders.Remove(attackTroopModel.Attacker);
                    attackTroopOrders.Add(attackTroopModel.Attacker, attackTroopModel);
                }
            }
            else
            {
                attackTroopOrders.Add(attackTroopModel.Attacker, attackTroopModel);
            }
        }
        catch (Exception ex)
        {
            LogManager.SendException(exceptionSender, ex);
        }
    }

    public void SetTroopOrderInModel(
        string troopName, 
        TroopModel troopModel, 
        GameObject emptyTargetsHolder,
        TargetPositionMarkerController targetMarkerController)
    {
        AttackTroopModel attackedTroop;
        MoveTroopModel moveTroopModel;

        if (attackTroopOrders.TryGetValue(troopName, out attackedTroop))
        {
            //TODO: Buscar la forma de encontrar la tropa deseada.
            //troopModel.SetTarget(newSelection.gameObject, globalLogic);
            attackTroopOrders.Remove(troopName);
        }
        else if (moveTroopOrders.TryGetValue(troopName, out moveTroopModel))
        {
            Vector3 mouseClickPosition = VectorUtils.StringToVector3(moveTroopModel.Position);

            troopModel.SetTarget(new GameObject(GlobalConstants.EmptyTargetName), globalLogic);
            troopModel.Target.transform.position = mouseClickPosition;
            troopModel.Target.transform.parent = emptyTargetsHolder.transform;
            targetMarkerController.SetTargetPosition(mouseClickPosition, false);
            moveTroopOrders.Remove(troopName);
        }
    }
}
