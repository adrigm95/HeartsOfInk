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

public class ConfigLinesUpdater
{
    // Constants
    private const string SenderKey = "SendConfigLine";
    private const string ReceiverKey = "ReceiveConfigLine";

    // Singleton variables
    private static readonly Lazy<ConfigLinesUpdater> _singletonReference = new Lazy<ConfigLinesUpdater>(() => new ConfigLinesUpdater());
    public static ConfigLinesUpdater Instance => _singletonReference.Value;

    // Other Variables
    private Dictionary<byte, ConfigLineModel> receivedConfigLines;
    private LobbyHOIHub signalRController;

    private ConfigLinesUpdater()
    {
        receivedConfigLines = new Dictionary<byte, ConfigLineModel>();
    }

    /// <summary>
    /// Suscriber, need to be called previously to start connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="signalRController"></param>
    public void SusbcribeReceiver(LobbyHOIHub signalRController, HubConnection connection)
    {
        connection.On<ConfigLineModel>(ReceiverKey, (receivedObject) =>
        {
            receivedConfigLines.Add(receivedObject.MapSocketId, receivedObject);
        });

        this.signalRController = signalRController;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configLine"> ConfigLine updated.</param>
    public async void SendConfigLine(ConfigLineIn configLine)
    {
        HubConnection connection = await signalRController.GetConnection();

        try
        {
            await connection.InvokeAsync(SenderKey, configLine);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public List<ConfigLineModel> GetReceivedConfigLines()
    {
        List<ConfigLineModel> listToReturn = receivedConfigLines.Values.ToList();

        receivedConfigLines.Clear();
        return listToReturn;
    }
}
