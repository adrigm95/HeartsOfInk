using Microsoft.AspNetCore.SignalR.Client;
using System;
using UnityEngine;

public class TroopDeadSignalR
{
    // Constants
    private const string SenderKey = "TroopDead";
    private const string ReceiverKey = "ReceiveTroopDead";
    

    // Singleton variables
    private static readonly Lazy<TroopDeadSignalR> _singletonReference = new Lazy<TroopDeadSignalR>(() => new TroopDeadSignalR());
    public static TroopDeadSignalR Instance => _singletonReference.Value;
    public static GlobalLogicController GlobalLogicController { get; set; }

    // Other Variables
    private IngameHOIHub signalRController;

    private TroopDeadSignalR()
    {
    }

    /// <summary>
    /// Suscriber, need to be called previously to start connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="signalRController"></param>
    public void SusbcribeReceiver(IngameHOIHub signalRController, HubConnection connection)
    {
        connection.On<string>(ReceiverKey, (troopDead) =>
        {
            ReceiveTroopDead(troopDead);
        });
        this.signalRController = signalRController;
    }

    /// <summary>
    /// Método para indicar al resto de jugadores de una partida multiplayer que una tropa ha sido derrotada.
    /// </summary>
    /// <param name="room"> Clave de partida. </param>
    /// <param name="troopDead"> Nombre de la tropa derrotada. </param>
    public async void SendTroopDead(string room, string troopDead)
    {
        HubConnection connection = await signalRController.GetConnection();

        try
        {
            await connection.InvokeAsync(SenderKey, room, troopDead);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="troopDead"> Nombre de la tropa derrotada. </param>
    public void ReceiveTroopDead(string troopDead)
    {
        try
        {
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
