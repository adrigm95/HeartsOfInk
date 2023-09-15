using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.GlobalInfo;
using Assets.Scripts.DataAccess;
using Assets.Scripts.Utils;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.Models;
using NETCoreServer.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour
{
    private ConfigGameController configGameController;
    private GameOptionsController gameOptionsController;
    private SceneChangeController sceneChangeController;
    private WebServiceCaller<Exception, bool> _logger = new WebServiceCaller<Exception, bool>();
    public Transform factionDropdownsHolder;
    public string MapId { get; set; }

    private void Start()
    {
        configGameController = FindObjectOfType<ConfigGameController>();
        gameOptionsController = FindObjectOfType<GameOptionsController>();
        sceneChangeController = FindObjectOfType<SceneChangeController>();
    }

    private void Update()
    {
        if (StartGameLobbySignalR.Instance.IsStartGameReceived())
        {
            StartGame(false, true);
        }
    }

    /// <summary>
    /// Realiza la lógica previa al comienzo de partida.
    /// </summary>
    /// <param name="sendStartToServer"> Solo es true si lo llama el host al darle a comenzar partida en multiplayer.</param>
    public async void StartGame(bool sendStartToServer)
    {
        StartGame(sendStartToServer, false);
    }

    private async void StartGame(bool sendStartToServer, bool startReceivedFromServer)
    {
        bool readyForChangeScene = true;
        GameModel gameModel = new GameModel("0");
        gameModel.MapId = MapId;
        gameModel.Gametype = sendStartToServer ? GameModel.GameType.MultiplayerHost : GameModel.GameType.Single;


        if (sendStartToServer)
        {
            GetMultiplayerOptions(gameModel);
            readyForChangeScene = await StartGameInServer(gameModel);
        }
        else if (startReceivedFromServer)
        {
            GetMultiplayerOptions(gameModel);
        }
        else
        {
            GetSingleplayerOptions(gameModel);
        }

        if (readyForChangeScene)
        {
            gameOptionsController.gameModel = gameModel;
            sceneChangeController.ChangeScene(transform);
        }
    }

    private async Task<bool> StartGameInServer(GameModel gameModel)
    {
        WebServiceCaller<GameModel, bool> wsCaller = new WebServiceCaller<GameModel, bool>();
        HOIResponseModel<bool> ingameServerResponse;

        try
        {
            gameModel.GameKey = configGameController.txtGamekey.text;
            ingameServerResponse = await wsCaller.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.POST, "api/GameRoom", gameModel);

            if (ingameServerResponse.serviceResponse)
            {
                StartGameLobbySignalR.Instance.SendStartGame(gameModel.GameKey);
            }
            else
            {
                Debug.LogError($"Error on StartGameInServer: {ingameServerResponse.ServiceError}");
            }

            return ingameServerResponse.serviceResponse;
        }
        catch (Exception ex)
        {
            await _logger.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.POST, "api/Log", ex);
            throw ex;
        }
        
    }

    private void GetSingleplayerOptions(GameModel gameModel)
    {
        // Singleplayer: newObject.name = "factionLine" + faction.Names[0].Value + "_" + faction.Id + "_" + player.MapSocketId;

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

    private void GetMultiplayerOptions(GameModel gameModel)
    {
        // Multiplayer: string ObjetName = "factionLine" + "_" + configLine.MapSocketId;
        string globalInfoPath = Application.streamingAssetsPath + "/_GlobalInfo.json";
        GlobalInfo globalInfo = JsonCustomUtils<GlobalInfo>.ReadObjectFromFile(globalInfoPath);

        foreach (ConfigLineModel configLine in configGameController.GetConfigLinesForMultiplayer())
        {
            Player player = configLine.ConvertToPlayer();
            player.Faction.Bonus = new Bonus((Bonus.Id)globalInfo.Factions.Find(item => item.Id == player.Faction.Id).BonusId);

            gameModel.Players.Add(player);
        }
    }
}

