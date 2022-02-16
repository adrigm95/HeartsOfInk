using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using Assets.Scripts.Data.Constants;
using LobbyHOIServer.Models.Models.In;

public class SignalRController : MonoBehaviour
{
    private HubConnection connection;

    // Start is called before the first frame update
    void Start()
    {
        connection = new HubConnectionBuilder()
            .WithUrl(ApiConfig.NETCoreServerUrl + "signalrhoi")
            /*.ConfigureLogging(logging => 
            {
                logging.
            })*/
            .Build();
        connection.Closed += async (error) =>
        {
            Debug.LogWarning("Connection with SignalR closed, restarting");
            await Task.Delay(1000); // don't want to hammer the network
            await connection.StartAsync();
        };
    }

    private async void StartConnection()
    {
        if (connection.State == HubConnectionState.Disconnected)
        {
            bool retry = false;

            while (true)
            {
                do
                {
                    try
                    {
                        Debug.Log("Setting signalR connection.ON");
                        /*connection.On<string>("ReceiveMessage", (messageReceived) =>
                        {
                            return ReceiveMessage(messageReceived);
                        });*/

                        Debug.Log("Starting connection with signalR");
                        await connection.StartAsync();
                        return;
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(1000);
                        Debug.LogError("Connection to signalR failed, reconnecting in 1 second");
                        retry = true;
                    }
                }
                while (retry);
            }
        }
    }

    private async Task ReceiveMessage(string msg)
    {
        Debug.LogWarning("Message received from signalR but not catched");
    }

    public async void SuscribeToGameConfig(string room)
    {
        /*connection.On<string>("AddToGroup", (sadfsaf) =>
        {
            return Task.FromResult(room);
        });*/

        try
        {
            StartConnection();

            Task.Delay(10000);
            await connection.InvokeAsync("AddToGroup", room);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void SendConfigLine(ConfigLineIn configLine)
    {
        connection.On<ConfigLineIn>("SendConfigLine", (gahfga) =>
        {
            return Task.FromResult(configLine);
        });

        try
        {
            await connection.StartAsync();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
