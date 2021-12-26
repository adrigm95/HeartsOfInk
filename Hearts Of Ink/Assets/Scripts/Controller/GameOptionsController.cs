using Assets.Scripts.Data;
using NETCoreServer.Models.GameModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptionsController : MonoBehaviour
{
    public GameModel gameModel;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
