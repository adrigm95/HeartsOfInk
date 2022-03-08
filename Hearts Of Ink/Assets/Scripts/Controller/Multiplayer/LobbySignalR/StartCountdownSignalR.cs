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

public class StartCountdownSignalR
{
    // Constants
    private const string SenderKey = "StartGame";
    private const string ReceiverKey = "StartCountdown";

    // Singleton variables
    private static readonly Lazy<StartCountdownSignalR> _singletonReference = new Lazy<StartCountdownSignalR>(() => new StartCountdownSignalR());
    public static StartCountdownSignalR Instance => _singletonReference.Value;
    public WaitingPanelController WaitingPanelController { get; set; }

    // Other Variables
    private LobbyHOIHub signalRController;

    private StartCountdownSignalR()
    {
    }

    /// <summary>
    /// Suscriber, need to be called previously to start connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="signalRController"></param>
    public void SusbcribeReceiver(LobbyHOIHub signalRController, HubConnection connection)
    {
        connection.On(ReceiverKey, StartCountdown);
        this.signalRController = signalRController;
    }

    public void StartCountdown()
    {
        Debug.Log("StartCountdown at: " + DateTime.Now);
        WaitingPanelController.StartGame();
    }
}
