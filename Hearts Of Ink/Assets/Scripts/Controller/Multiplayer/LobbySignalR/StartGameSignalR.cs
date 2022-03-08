using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using Assets.Scripts.Data.Constants;
using LobbyHOIServer.Models.Models.In;
using LobbyHOIServer.Models.Models;
using System.Linq;
using NETCoreServer.Models;

public class StartGameSignalR
{
    // Constants
    private const string SenderKey = "StartGame";
    private const string ReceiverKey = "ReceiveStartGame";

    // Singleton variables
    private static readonly Lazy<StartGameSignalR> _singletonReference = new Lazy<StartGameSignalR>(() => new StartGameSignalR());
    public static StartGameSignalR Instance => _singletonReference.Value;
    public StartGameController StartGameController { get; set; }

    // Other Variables
    private LobbyHOIHub signalRController;

    private StartGameSignalR()
    {
    }

    /// <summary>
    /// Suscriber, need to be called previously to start connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="signalRController"></param>
    public void SusbcribeReceiver(LobbyHOIHub signalRController, HubConnection connection)
    {
        connection.On(ReceiverKey, ReceiveStartGame);
        this.signalRController = signalRController;
    }

    /// <summary>
    /// Avisa al servidor de que se ha requerido iniciar partida. Este método solo tiene que poder llamarlo el host de la partida.
    /// </summary>
    public async void SendStartGame(string room)
    {
        HubConnection connection = await signalRController.GetConnection();

        try
        {
            await connection.InvokeAsync(SenderKey, room);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void SendClientReady(string room)
    {
        HubConnection connection = await signalRController.GetConnection();

        try
        {
            await connection.InvokeAsync("ClientReady", room);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void ReceiveStartGame()
    {
        Debug.Log("ReceiveStartGame");
        StartGameController.StartGame(true);
    }
}
