using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.DataAccess;
using NETCoreServer.Models;
using NETCoreServer.Models.In;
using NETCoreServer.Models.Out;
using UnityEngine;
using UnityEngine.UI;

public class JoinGameController : MonoBehaviour
{
    public ConfigGameController configGameController;
    public InfoPanelController infoPanelController;
    public InputField inpGameKey;
    public InputField playerName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TryJoinGame()
    {
        string gameKey = inpGameKey.text;

        if (gameKey.Length != 6)
        {
            infoPanelController.DisplayMessage("Invalid gamekey", "The gamekey must have 6 characters");
        }
        else
        {
            RequestEntryGame(gameKey, null);
        }
    }

    /// <summary>
    /// Solicita la entrada a la partida.
    /// </summary>
    /// <param name="gamekey"> Clave de la partida.</param>
    /// <param name="isPublic"> Determina si la partida es pública (true), privada (false), o puede ser de cualquiera de los dos tipos (null)</param>
    public async void RequestEntryGame(string gamekey, bool? isPublic)
    {
        WebServiceCaller<RequestEntryModelIn, RequestEntryModelOut> wsCaller = new WebServiceCaller<RequestEntryModelIn, RequestEntryModelOut>();
        HOIResponseModel<RequestEntryModelOut> response;
        RequestEntryModelIn requestEntryModel = new RequestEntryModelIn();

        requestEntryModel.gameKey = gamekey;
        requestEntryModel.isPublic = isPublic;
        requestEntryModel.playerName = playerName.text;

        if (string.IsNullOrEmpty(requestEntryModel.playerName))
        {
            infoPanelController.DisplayMessage("Invalid player name", "Player name cannot be empty");
        }
        else
        {
            response = await wsCaller.GenericWebServiceCaller(Method.POST, LobbyHOIControllers.RequestEntry, requestEntryModel);

            switch (response.internalResultCode)
            {
                case InternalStatusCodes.OKCode:
                    configGameController.LoadConfigGame(response.serviceResponse.ConfigLines, response.serviceResponse.MapId, false);
                    break;
                case InternalStatusCodes.GameNotFind:
                    infoPanelController.DisplayMessage("Invalid gamekey", "No game find for the introduced gamekey");
                    break;
                default:
                    infoPanelController.DisplayMessage("Unexpected error", "Unexpected error on join game: " + response.internalResultCode);
                    Debug.LogWarning("[RequestEntryGame] Unexpected result code: " + response.internalResultCode);
                    break;
            }
        }
    }
}
