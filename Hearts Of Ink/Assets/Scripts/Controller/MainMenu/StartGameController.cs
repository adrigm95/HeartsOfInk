using Assets.Scripts.Data;
using Assets.Scripts.Data.Literals;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour
{
    private SceneChangeController sceneChangeController;
    public Transform factionDropdownsHolder;

    private void Start()
    {
        sceneChangeController = FindObjectOfType<SceneChangeController>();
    }

    public void StartGame()
    {
        GameModel gameModel = new GameModel(Application.streamingAssetsPath + "/MapDefinitions/0_Cartarena_v0_3_0.json");
        gameModel.Gametype = GameModel.GameType.Single;
        
        GetPlayerOptions(gameModel);
        sceneChangeController.ChangeScene(transform);
        PlayerPrefs.SetString(PlayerPrefsData.GameModelKey, JsonUtility.ToJson(gameModel, false));
    }

    private void GetPlayerOptions(GameModel gameModel)
    {
        foreach (Transform holderChild in factionDropdownsHolder)
        {
            if (holderChild.name.StartsWith(GlobalConstants.FactionLineStart))
            {
                Player player = new Player();
                string[] holderNameSplitted = holderChild.name.Split('_');
                string factionId = holderNameSplitted[1];
                string mapSocketId = holderNameSplitted[2];
                Dropdown iaSelector = holderChild.GetComponentInChildren<Dropdown>();

                player.Faction.Id = Convert.ToInt32(factionId);
                player.MapSocketId = Convert.ToByte(mapSocketId);
                player.IaId = (Player.IA)(Convert.ToInt32(iaSelector.value));
                if (player.IaId == Player.IA.PLAYER)
                {
                    player.Name = "Player";
                }
                else
                {
                    // Todo: Indicar nombre obtenido en el mapa.
                    player.Name = factionId;
                }

                gameModel.Players.Add(player);
            }
        }
    }
}
