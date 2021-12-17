using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public ConfigGameController configGameController;

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

    private void CleanList()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private async void LoadGames()
    {
        WebServiceCaller<CreateGameModel, List<BasicGameInfo>> wsCaller = new WebServiceCaller<CreateGameModel, List<BasicGameInfo>>();
        HOIResponseModel<List<BasicGameInfo>> response;

        response = await wsCaller.GenericWebServiceCaller(Method.GET, "api/PublicGames", null);

        if (response.internalResultCode == InternalStatusCodes.OKCode)
        {
            PopulateList(response);
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
    }

    public async void RequestEntryGame()
    {
        WebServiceCaller<RequestEntryModel, BasicGameInfo> wsCaller = new WebServiceCaller<RequestEntryModel, BasicGameInfo>();
        HOIResponseModel<BasicGameInfo> response;
        RequestEntryModel requestEntryModel = new RequestEntryModel();

        requestEntryModel.gameKey = gameItemSelected.gameObject.name;
        requestEntryModel.isPublic = true;
        requestEntryModel.playerName = "PacoPepe";

        response = await wsCaller.GenericWebServiceCaller(Method.POST, "api/RequestEntry", requestEntryModel);

        if (response.internalResultCode == InternalStatusCodes.OKCode)
        {
            configGameController.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[RequestEntryGame] Unexpected result code: " + response.internalResultCode);
        }
    }
}
