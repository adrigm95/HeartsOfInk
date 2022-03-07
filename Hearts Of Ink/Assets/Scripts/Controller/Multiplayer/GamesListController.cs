using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.Utils;
using Assets.Scripts.DataAccess;
using NETCoreServer.Models;
using NETCoreServer.Models.In;
using NETCoreServer.Models.Out;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Assets.Scripts.Data.Constants;

public class GamesListController : MonoBehaviour
{
    const string ItemPrefab = "Prefabs/PublicGameItem";
    const int lineSpacing = 25;
    private float width;
    private RectTransform rectTransform;
    private GameItemController gameItemSelected;

    public Vector2 nextItemPosition;
    public float MinifiedSize;
    public float OneGreatAndFreeSize;
    public JoinGameController joinGameController;
    public InfoPanelController infoPanelController;

    // Start is called before the first frame update
    void Start()
    {
        gameItemSelected = null;
        rectTransform = transform.GetComponent<RectTransform>();
        width = rectTransform.sizeDelta.x;
        LoadGames();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Recargar la lista cada cierto tiempo.
    }

    /// <summary>
    /// Hace que el listado reduzca su altura.
    /// </summary>
    public void Minify()
    {
        //TODO: https://forum.unity.com/threads/setting-top-and-bottom-on-a-recttransform.265415/
    }

    /// <summary>
    /// Hace que el listado aumente su altura.
    /// </summary>
    public void MakeListGreatAgain()
    {
        //TODO: https://forum.unity.com/threads/setting-top-and-bottom-on-a-recttransform.265415/
        //rectTransform.sizeDelta = new Vector2(width, SizeOpen);
    }

    /// <summary>
    /// Necesario para recargar la lista (Si todavia no tiene referencias es porque es a futuro, NO BORRAR).
    /// </summary>
    private void CleanList()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private async void LoadGames()
    {
        WebServiceCaller<List<BasicGameInfo>> wsCaller = new WebServiceCaller<List<BasicGameInfo>>();
        HOIResponseModel<List<BasicGameInfo>> response;

        response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.GET, LobbyHOIControllers.PublicGames);

        switch (response.internalResultCode)
        {
            case InternalStatusCodes.OKCode:
                PopulateList(response);
                break;
            case InternalStatusCodes.KOConnectionCode:
                infoPanelController.DisplayMessage("Connection error", "Error when try to connect to server.");
                break;
        }
    }

    private void PopulateList(HOIResponseModel<List<BasicGameInfo>> response)
    {
        List<BasicGameInfo> games = response.serviceResponse;
        foreach (BasicGameInfo game in games)
        {
            AddGameToPanel(game, false);
        }
    }

    public void AddGameToPanel(BasicGameInfo game, bool isSelected)
    {
        GameItemController gameItem;
        GameObject newGameText;
        RectTransform rectTransform;
        Text text;

        newGameText = (GameObject) Instantiate(Resources.Load(ItemPrefab), nextItemPosition, transform.rotation, transform);
        newGameText.name = game.gameKey;
        rectTransform = newGameText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = nextItemPosition;
        text = newGameText.GetComponent<Text>();
        gameItem = newGameText.GetComponent<GameItemController>();
        nextItemPosition.y -= lineSpacing;

        text.text = PanelUtils.GetGameName(game);
        Debug.Log("New game added to panel with key " + game.gameKey);

        if (isSelected)
        {
            SetGameAsSelected(gameItem);
        }
    }

    public void SetGameAsSelected(GameItemController gameItem)
    {
        Text text;

        text = gameItem.gameObject.GetComponent<Text>();
        text.fontStyle = FontStyle.Bold;

        gameItemSelected = gameItem;
        Debug.Log("Game item selected: " + gameItemSelected.gameObject.name);
    }

    public void RequestEntryGame()
    {
        joinGameController.RequestEntryGame(gameItemSelected.gameObject.name, true);
    }
}
