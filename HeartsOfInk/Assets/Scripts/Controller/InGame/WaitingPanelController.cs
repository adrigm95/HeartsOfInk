using Assets.Scripts.Data.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPanelController : MonoBehaviour
{
    private const string WaitingForPlayers = "Esperando al resto de jugadores ...";
    private const string StartingIn = "Comenzando en ..";
    private float startTime;
    [SerializeField]
    private GlobalLogicController globalLogicController;
    [SerializeField]
    private Text txtCentralMessage;
    [SerializeField]
    private Text txtCountdown;

    // Start is called before the first frame update
    void Start()
    {
        startTime = float.MinValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime != float.MinValue)
        {
            int leftToStart = Convert.ToInt32(startTime - Time.realtimeSinceStartup);

            if (leftToStart >= 0)
            {
                txtCountdown.text = Convert.ToString(leftToStart);
            }
            else
            {
                globalLogicController.ChangeSpeed(GameSpeedConstants.PlaySpeed);
            }
        }
    }

    public void Show(GlobalLogicController globalLogicController)
    {
        this.globalLogicController = globalLogicController;
        startTime = float.MinValue;
        this.gameObject.SetActive(true);
        txtCountdown.text = string.Empty;
        txtCentralMessage.text = WaitingForPlayers;
    }

    public void StartGame()
    {
        startTime = Time.realtimeSinceStartup + 3;
        txtCountdown.text = "3";
        txtCentralMessage.text = StartingIn;
    }
}
