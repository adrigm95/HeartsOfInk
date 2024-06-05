using Assets.Scripts.Data.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class IngameHOIHub
{
    private static string LastRoomConnected;
    // Singleton variables
    private static readonly Lazy<IngameHOIHub> _singletonReference = new Lazy<IngameHOIHub>(() => new IngameHOIHub());
    public static IngameHOIHub Instance => _singletonReference.Value;

    // Other variables
    private HubConnection connection;

    // Start is called before the first frame update
    private IngameHOIHub()
    {
        connection = new HubConnectionBuilder()
            .WithUrl(ApiConfig.IngameServerUrl + ApiConfig.SignalRHUBName)
            .Build();
        connection.Closed += async (error) =>
        {
            Debug.LogWarning($"Connection with SignalR closed, restarting; url {ApiConfig.IngameServerUrl}; error {error}");
            await Task.Delay(1000); // don't want to hammer the network
            await connection.StartAsync();
        };
    }

    public async Task<HubConnection> GetConnection()
    {
        if (connection.State == HubConnectionState.Disconnected)
        {
            StartConnection();
            await Task.Delay(1000);
        }

        return connection;
    }

    private async void StartConnection()
    {
        bool retry;

        while (true)
        {
            do
            {
                try
                {
                    Debug.Log("Setting signalR connection.ON");
                    StartGameIngameSignalR.Instance.SusbcribeReceiver(this, connection);
                    TroopDeadSignalR.Instance.SusbcribeReceiver(this, connection);
                    AttackTroopSignalR.Instance.SusbcribeReceiver(this, connection);
                    MoveTroopSignalR.Instance.SusbcribeReceiver(this, connection);
                    Debug.Log("Starting connection with signalR");
                    await connection.StartAsync();
                    return;
                }
                catch (Exception ex)
                {
                    await Task.Delay(1000);
                    Debug.LogError("Connection to signalR failed, reconnecting in 1 second. Exception message: " + ex.Message);
                    retry = true;
                }
            }
            while (retry);
        }
    }

    public async void SuscribeToRoom(string room, byte playerId)
    {
        try
        {
            LastRoomConnected = room;
            StartConnection();

            await connection.InvokeAsync("AddToGroup", room, playerId);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
