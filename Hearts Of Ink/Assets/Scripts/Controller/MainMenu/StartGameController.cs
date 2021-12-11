using Assets.Scripts.Data;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.Data.Literals;
using Assets.Scripts.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour
{
    private SceneChangeController sceneChangeController;
    public Transform factionDropdownsHolder;
    public GameOptionsController gameOptionsController;

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
        gameOptionsController.gameModel = gameModel;
    }

    private void GetPlayerOptions(GameModel gameModel)
    {
        string globalInfoPath = Application.streamingAssetsPath + "/MapDefinitions/_GlobalInfo.json";
        GlobalInfo globalInfo = JsonCustomUtils<GlobalInfo>.ReadObjectFromFile(globalInfoPath);

        foreach (Transform holderChild in factionDropdownsHolder)
        {
            if (holderChild.name.StartsWith(GlobalConstants.FactionLineStart))
            {
                Player player = new Player();
                string[] holderNameSplitted = holderChild.name.Split('_');
                string factionId = holderNameSplitted[1];
                string mapSocketId = holderNameSplitted[2];
                Image btnColorFaction = holderChild.Find("btnColorFaction").GetComponent<Image>();
                Dropdown iaSelector = holderChild.GetComponentInChildren<Dropdown>();

                player.Faction.Id = Convert.ToInt32(factionId);
                player.Faction.Bonus = new Bonus(Rawgen.Literals.LiteralsFactory.Language.es, 
                    (Bonus.Id) globalInfo.Factions.Find(item => item.Id == player.Faction.Id).BonusId);
                player.MapSocketId = Convert.ToByte(mapSocketId);
                player.IaId = (Player.IA)(Convert.ToInt32(iaSelector.value));
                player.Color = btnColorFaction.color;
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
