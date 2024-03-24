using Microsoft.AspNetCore.SignalR.Client;
using System;
using UnityEngine;

public class StartGameIngameSignalR
{
    // Constants
    private const string SenderKey = "StartGame";
    private const string ReceiverKey = "ReceiveStartGame";

    // Singleton variables
    private static readonly Lazy<StartGameIngameSignalR> _singletonReference = new Lazy<StartGameIngameSignalR>(() => new StartGameIngameSignalR());
    public static StartGameIngameSignalR Instance => _singletonReference.Value;
    public StartGameController StartGameController { get; set; }

    // Other Variables
    private IngameHOIHub signalRController;

    private StartGameIngameSignalR()
    {
    }

    /// <summary>
    /// Suscriber, need to be called previously to start connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="signalRController"></param>
    public void SusbcribeReceiver(IngameHOIHub signalRController, HubConnection connection)
    {
        connection.On(ReceiverKey, ReceiveStartGame);
        this.signalRController = signalRController;
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
        StartGameController.StartGame(false);
    }
}
