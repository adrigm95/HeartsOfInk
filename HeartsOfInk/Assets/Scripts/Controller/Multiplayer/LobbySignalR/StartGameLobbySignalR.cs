using Microsoft.AspNetCore.SignalR.Client;
using System;
using UnityEngine;

public class StartGameLobbySignalR
{
    // Constants
    private const string SenderKey = "StartGame";
    private const string ReceiverKey = "ReceiveStartGame";

    // Singleton variables
    private static readonly Lazy<StartGameLobbySignalR> _singletonReference = new Lazy<StartGameLobbySignalR>(() => new StartGameLobbySignalR());
    public static StartGameLobbySignalR Instance => _singletonReference.Value;
    private bool startGameReceived;

    // Other Variables
    private LobbyHOIHub signalRController;

    private StartGameLobbySignalR()
    {
        startGameReceived = false;
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

    public async void ReceiveStartGame()
    {
        try
        {
            Debug.Log("ReceiveStartGame - Lobby");
            startGameReceived = true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public bool IsStartGameReceived()
    {
        if (startGameReceived)
        {
            startGameReceived = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
