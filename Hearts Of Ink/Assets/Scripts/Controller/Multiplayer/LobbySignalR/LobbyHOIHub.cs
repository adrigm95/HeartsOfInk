using Assets.Scripts.Data.Constants;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyHOIHub
{
    // Singleton variables
    private static readonly Lazy<LobbyHOIHub> _singletonReference = new Lazy<LobbyHOIHub>(() => new LobbyHOIHub());
    public static LobbyHOIHub Instance => _singletonReference.Value;

    // Other variables
    private HubConnection connection;

    // Start is called before the first frame update
    private LobbyHOIHub()
    {
        connection = new HubConnectionBuilder()
            .WithUrl(ApiConfig.LobbyHOIServerUrl + "signalrhoi")
            .Build();
        connection.Closed += async (error) =>
        {
            Debug.LogWarning("Connection with SignalR closed, restarting");
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
                    ConfigLinesUpdater.Instance.SusbcribeReceiver(this, connection);
                    StartGameSignalR.Instance.SusbcribeReceiver(this, connection);
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

    public async void SuscribeToRoom(string room)
    {
        try
        {
            StartConnection();

            await connection.InvokeAsync("AddToGroup", room);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
