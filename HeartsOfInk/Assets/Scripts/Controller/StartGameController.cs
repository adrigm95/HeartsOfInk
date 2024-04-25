using AnalyticsServer.Models;
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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour
{
    private ConfigGameController configGameController;
    private GameOptionsController gameOptionsController;
    private SceneChangeController sceneChangeController;
    private WebServiceCaller<LogDto, bool> logSender = new WebServiceCaller<LogDto, bool>();
    private WebServiceCaller<Exception, bool> exceptionSender = new WebServiceCaller<Exception, bool>();
    private GameModel gameModel;
    public Transform factionDropdownsHolder;
    public GameModel GameModel { get { return gameModel; } }

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
            Debug.Log("StartGameController - Update - IsStartGameReceived = true");
            StartGame(false, true);
        }
    }

    /// <summary>
    /// Realiza la lógica previa al comienzo de partida.
    /// </summary>
    /// <param name="sendStartToServer"> Solo es true si lo llama el host al darle a comenzar partida en multiplayer.</param>
    public void StartGame(bool sendStartToServer)
    {
        if (configGameController == null)
        {
            // Si no tiene configGame es una partida singleplayer.
            StartGame(false, false);
        }
        else if (configGameController.isGameHost)
        {
            StartGame(sendStartToServer, false);
        }
        else
        {
            string warning = "Opción de método StartGame incorrecta: El client multiplayer no debería llamar a este método.";
            LogManager.SendLog(logSender, warning);
            Debug.LogWarning(warning);
        }
    }

    public void StartGameFromServer()
    {
        string log = "Start received from server";
        LogManager.SendLog(logSender, log);
        Debug.Log(log);
        StartGame(false, true);
    }

    private async void StartGame(bool sendStartToServer, bool startReceivedFromServer)
    {
        bool readyForChangeScene = true;

        if (sendStartToServer)
        {
            GameModel.Gametype = GameModel.GameType.MultiplayerHost;
            GetMultiplayerOptions(GameModel);
            readyForChangeScene = await StartGameInServer(GameModel);
        }
        else if (startReceivedFromServer)
        {
            GameModel.Gametype = GameModel.GameType.MultiplayerClient;
            GetMultiplayerOptions(GameModel);
        }
        else
        {
            GameModel.Gametype = GameModel.GameType.Single;
            GetSingleplayerOptions(GameModel);
        }

        if (readyForChangeScene)
        {
            Debug.Log("ReadyForChangeScene - GameModel.gamekey: " + gameModel.GameKey);
            gameOptionsController.gameModel = GameModel;
            sceneChangeController.ChangeScene(transform);
        }
    }

    public void UpdateGameModel(GameModel gameModel)
    {
        if (GameModel == null)
        {
            this.gameModel = gameModel;
        }
        else
        {
            GameModel.MapId = StringUtils.ReplaceValueIfNotEmpty(gameModel.MapId, GameModel.MapId);
            GameModel.GameKey = StringUtils.ReplaceValueIfNotEmpty(gameModel.GameKey, GameModel.GameKey);
            GameModel.Name = StringUtils.ReplaceValueIfNotEmpty(gameModel.Name, GameModel.Name);
            GameModel.Gametype = gameModel.Gametype;
            GameModel.IsPublic = gameModel.IsPublic;
            GameModel.Players = gameModel.Players;
        }
    }

    public void SetMapId(string mapId)
    {
        if (GameModel == null)
        {
            this.gameModel = new GameModel();
        }

        this.gameModel.MapId = mapId;
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
            await exceptionSender.GenericWebServiceCaller(ApiConfig.IngameServerUrl, Method.POST, "api/Log", ex);
            throw ex;
        }
        
    }

    private void GetSingleplayerOptions(GameModel gameModel)
    {
        // Singleplayer: newObject.name = "factionLine" + faction.Names[0].Value + "_" + faction.Id + "_" + player.MapSocketId;
        GlobalInfo globalInfo = GlobalInfoDAC.LoadGlobalMapInfo();

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
        GlobalInfo globalInfo = GlobalInfoDAC.LoadGlobalMapInfo();

        foreach (ConfigLineModel configLine in configGameController.GetConfigLinesForMultiplayer())
        {
            Player player = configLine.ConvertToPlayer();
            player.Faction.Bonus = new Bonus((Bonus.Id)globalInfo.Factions.Find(item => item.Id == player.Faction.Id).BonusId);

            gameModel.Players.Add(player);
            Debug.Log("GetMultiplayerOptions - player: " + player.Name);
        }
    }
}

