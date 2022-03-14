using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour
{
    private GameOptionsController gameOptionsController;
    private SceneChangeController sceneChangeController;
    public Transform factionDropdownsHolder;

    private void Start()
    {
        gameOptionsController = FindObjectOfType<GameOptionsController>();
        sceneChangeController = FindObjectOfType<SceneChangeController>();
        StartGameSignalR.Instance.StartGameController = this;
    }

    public async void StartGame(bool isMultiplayer)
    {
        bool readyForChangeScene = true;
        GameModel gameModel = new GameModel(0);
        gameModel.Gametype = isMultiplayer ? GameModel.GameType.MultiplayerHost : GameModel.GameType.Single;
        
        GetPlayerOptions(gameModel);

        if (isMultiplayer)
        {
            readyForChangeScene = await StartGameInServer(gameModel);
        }

        if (readyForChangeScene)
        {
            sceneChangeController.ChangeScene(transform);
            gameOptionsController.gameModel = gameModel;
        }
    }

    private async Task<bool> StartGameInServer(GameModel gameModel)
    {
        WebServiceCaller<GameModel, bool> wsCaller = new WebServiceCaller<GameModel, bool>();
        HOIResponseModel<bool> ingameServerResponse;

        ingameServerResponse = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyIngameServerUrl, Method.POST, "api/GameRoom", gameModel);

        if (ingameServerResponse.serviceResponse)
        {
            StartGameSignalR.Instance.SendStartGame(gameModel.gameKey);
        }
        else
        {
            Debug.LogError($"Error on StartGameInServer: {ingameServerResponse.ServiceError}");
        }

        return ingameServerResponse.serviceResponse;
    }

    private void GetPlayerOptions(GameModel gameModel)
    {
        string globalInfoPath = Application.streamingAssetsPath + "/_GlobalInfo.json";
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
                Text txtBtnAlliance = holderChild.Find("btnAlliance").Find("txtBtnAlliance").GetComponent<Text>();
                Dropdown iaSelector = holderChild.GetComponentInChildren<Dropdown>();

                player.Faction.Id = Convert.ToInt32(factionId);
                player.Faction.Bonus = new Bonus((Bonus.Id) globalInfo.Factions.Find(item => item.Id == player.Faction.Id).BonusId);
                player.MapSocketId = Convert.ToByte(mapSocketId);
                player.IaId = (Player.IA)(Convert.ToInt32(iaSelector.value));
                player.Color = ColorUtils.GetStringByColor(btnColorFaction.color);
                player.Alliance = string.IsNullOrEmpty(txtBtnAlliance.text) ? (byte) 0 : Convert.ToByte(txtBtnAlliance.text);
                
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
