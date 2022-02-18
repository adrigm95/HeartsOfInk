using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using Assets.Scripts.Data.Constants;
using LobbyHOIServer.Models.Models.In;

public class ConfigLinesUpdater
{
    // Constants
    private const string SenderKey = "SendConfigLine";
    private const string ReceiverKey = "ReceiveConfigLine";

    // Singleton variables
    private static readonly Lazy<ConfigLinesUpdater> _singletonReference = new Lazy<ConfigLinesUpdater>(() => new ConfigLinesUpdater());
    public static ConfigLinesUpdater Instance => _singletonReference.Value;

    // Other Variables
    private Dictionary<byte, ConfigLineIn> receivedConfigLines;
    private LobbyHOIHub signalRController;

    private ConfigLinesUpdater()
    {
        receivedConfigLines = new Dictionary<byte, ConfigLineIn>();
    }

    /// <summary>
    /// Suscriber, need to be called previously to start connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="signalRController"></param>
    public void SusbcribeReceiver(LobbyHOIHub signalRController, HubConnection connection)
    {
        connection.On<ConfigLineIn>(ReceiverKey, (receivedObject) =>
        {
            ReceiveConfigLine(receivedObject);
        });

        this.signalRController = signalRController;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configLine"> ConfigLine updated.</param>
    /// <param name="room"> Gamekey.</param>
    public async void SendConfigLine(ConfigLineIn configLine, string room)
    {
        HubConnection connection = await signalRController.GetConnection();

        try
        {
            await connection.InvokeAsync(SenderKey, configLine, room);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void ReceiveConfigLine(ConfigLineIn configLine)
    {
        receivedConfigLines.Add(configLine.configLineModel.MapSocketId, configLine);
    }
}
