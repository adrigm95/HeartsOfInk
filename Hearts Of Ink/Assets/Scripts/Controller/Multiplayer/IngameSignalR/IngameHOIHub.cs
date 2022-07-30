using Assets.Scripts.Data.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class IngameHOIHub
{
    // Singleton variables
    private static readonly Lazy<IngameHOIHub> _singletonReference = new Lazy<IngameHOIHub>(() => new IngameHOIHub());
    public static IngameHOIHub Instance => _singletonReference.Value;

    // Other variables
    private HubConnection connection;

    // Start is called before the first frame update
    private IngameHOIHub()
    {
        connection = new HubConnectionBuilder()
            .WithUrl(ApiConfig.IngameServerUrl + "signalrhoi")
            .Build();
        connection.Closed += async (error) =>
        {
            Debug.LogWarning($"Connection with SignalR closed, restarting; url {ApiConfig.IngameServerUrl}");
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
        bool retry = false;

        while (true)
        {
            do
            {
                try
                {
                    Debug.Log("Setting signalR connection.ON");
                    StartGameIngameSignalR.Instance.SusbcribeReceiver(this, connection);
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

    public async void SuscribeToRoom(string room, string playerName)
    {
        try
        {
            StartConnection();

            await connection.InvokeAsync("AddToGroup", room, playerName);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
