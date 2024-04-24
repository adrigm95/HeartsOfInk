using Assets.Scripts.Data.MultiplayerStateModels;
using LobbyHOIServer.Models.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveTroopSignalR
{
    // Constants
    private const string SenderKey = "MoveTroop";
    private const string ReceiverKey = "ReceiveMoveTroop";
    

    // Singleton variables
    private static readonly Lazy<MoveTroopSignalR> _singletonReference = new(() => new MoveTroopSignalR());
    public static MoveTroopSignalR Instance => _singletonReference.Value;

    // Other Variables
    private IngameHOIHub signalRController;
    private List<MoveTroopModel> movementsReceived;

    private MoveTroopSignalR()
    {
        movementsReceived = new List<MoveTroopModel>();
    }

    /// <summary>
    /// Suscriber, need to be called previously to start connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="signalRController"></param>
    public void SusbcribeReceiver(IngameHOIHub signalRController, HubConnection connection)
    {
        connection.On<MoveTroopModel>(ReceiverKey, (moveTroop) =>
        {
            ReceiveMoveTroop(moveTroop);
        });
        this.signalRController = signalRController;
    }

    /// <summary>
    /// Método para enviar una orden de movimiento al host.
    /// </summary>
    /// <param name="room"> Clave de partida. </param>
    public async void SendMoveTroop(string room, string position, string troopName, float timeSinceStart)
    {
        HubConnection connection = await signalRController.GetConnection();

        try
        {
            MoveTroopModel attackTroopModel = new MoveTroopModel()
            {
                Position = position,
                TroopName = troopName,
                TimeSinceStart = timeSinceStart
            };
            await connection.InvokeAsync(SenderKey, room, attackTroopModel);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Metodo que recibe una orden de ataque.
    /// </summary>
    public void ReceiveMoveTroop(MoveTroopModel moveTroopModel)
    {
        try
        {
            movementsReceived.Add(moveTroopModel);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public List<MoveTroopModel> GetMoveTroopReceived()
    {
        // Hacemos una copia para devolverla y poder limpiar la lista local.
        List<MoveTroopModel> listToReturn = movementsReceived.Select(p => p).ToList();

        movementsReceived.Clear();
        return listToReturn;
    }
}
