using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigGameController : MonoBehaviour
{
    /// <summary>
    /// Tiempo en segundos tras el cual hay que notificar al servidor que aun estamos configurando la partida.
    /// </summary>
    private const float timeToNotifyActive = 300;

    /// <summary>
    /// Indica si actualmente hay una partida creada por el jugador local en fase de configuración.
    /// </summary>
    private bool activeGameCreated;

    /// <summary>
    /// Momento en que se comunica con el server por última vez, expresado en segundos desde el inicio de la partida.
    /// </summary>
    private float lastAdviceToServer;

    private BasicGameInfo basicGameInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activeGameCreated)
        {
            if (Time.time > lastAdviceToServer + timeToNotifyActive)
            {
                NotifyActive();
                lastAdviceToServer = Time.time;
            }
        }
    }

    public void GameCreatedByHost(BasicGameInfo newGame)
    {
        basicGameInfo = newGame;
    }

    public async void NotifyActive()
    {
        WebServiceCaller<string, bool> wsCaller = new WebServiceCaller<string, bool>();
        HOIResponseModel<bool> response;

        try
        {
            response = await wsCaller.GenericWebServiceCaller(Method.POST, "api/NotifyActive", basicGameInfo.gameKey);

            if (!response.serviceResponse)
            {
                Debug.LogWarning("Server response false on 'NotifyActive' service call.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
