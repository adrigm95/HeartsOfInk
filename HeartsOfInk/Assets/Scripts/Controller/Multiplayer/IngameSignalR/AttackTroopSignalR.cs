using Assets.Scripts.Data.MultiplayerStateModels;
using LobbyHOIServer.Models.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackTroopSignalR
{
    // Constants
    private const string SenderKey = "AttackTroop";
    private const string ReceiverKey = "ReceiveAttackTroop";
    

    // Singleton variables
    private static readonly Lazy<AttackTroopSignalR> _singletonReference = new(() => new AttackTroopSignalR());
    public static AttackTroopSignalR Instance => _singletonReference.Value;

    // Other Variables
    private IngameHOIHub signalRController;
    private List<AttackTroopModel> attacksReceived;

    private AttackTroopSignalR()
    {
        attacksReceived = new List<AttackTroopModel>();
    }

    /// <summary>
    /// Suscriber, need to be called previously to start connection.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="signalRController"></param>
    public void SusbcribeReceiver(IngameHOIHub signalRController, HubConnection connection)
    {
        connection.On<AttackTroopModel>(ReceiverKey, (attackTroop) =>
        {
            ReceiveAttackTroop(attackTroop);
        });
        this.signalRController = signalRController;
    }

    /// <summary>
    /// Método para enviar una orden de ataque al host.
    /// </summary>
    /// <param name="room"> Clave de partida. </param>
    /// <param name="attacker"> Nombre de la tropa atacante </param>
    /// <param name="attacked"> Nombre de la tropa atacada </param>
    public async void SendAttackTroop(string room, string attacker, string attacked, float timeSinceStart)
    {
        HubConnection connection = await signalRController.GetConnection();

        try
        {
            AttackTroopModel attackTroopModel = new AttackTroopModel()
            {
                Attacker = attacker,
                Attacked = attacked,
                TimeSinceStart = timeSinceStart
            };
            await connection.InvokeAsync(SenderKey, room, attackTroopModel);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Metodo que recibe una orden de ataque.
    /// </summary>
    public void ReceiveAttackTroop(AttackTroopModel attackTroopModel)
    {
        try
        {
            attacksReceived.Add(attackTroopModel);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public List<AttackTroopModel> GetAttackTroopReceived()
    {
        // Hacemos una copia para devolverla y poder limpiar la lista local.
        List<AttackTroopModel> listToReturn = attacksReceived.Select(p => p).ToList();

        attacksReceived.Clear();
        return listToReturn;
    }
}
