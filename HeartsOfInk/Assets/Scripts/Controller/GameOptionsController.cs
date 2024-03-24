using Assets.Scripts.Data;
using NETCoreServer.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptionsController : MonoBehaviour
{
    private static GameOptionsController _instance;

    public GameModel gameModel;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
