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

    public Vector2 nextItemPosition;
    public float MinifiedSize;
    public float OneGreatAndFreeSize;

    // Start is called before the first frame update
    void Start()
    {
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
        GameObject newGameText;
        RectTransform rectTransform;
        Text text;

        newGameText = (GameObject) Instantiate(Resources.Load(ItemPrefab), nextItemPosition, transform.rotation, transform);
        newGameText.name = game.gameKey;
        rectTransform = newGameText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = nextItemPosition;
        text = newGameText.GetComponent<Text>();
        nextItemPosition.y -= lineSpacing;

        text.text = PanelUtils.GetGameName(game);
        Debug.Log("New game added to panel with key " + game.gameKey);

        if (isSelected)
        {
            SetGameAsSelected(newGameText);
        }
    }

    public void SetGameAsSelected(GameObject item)
    {
        const string color = "027AAA";
    }
}
