using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuPanelController : MonoBehaviour
{
    public enum SubMenuPanels { PublicGames = 0, JoinGame = 1, CreateGame = 2}

    private float width;
    private RectTransform rectTransform;
    private GameObject activePanel;

    [SerializeField]
    private GameObject publicGames;

    [SerializeField]
    private GameObject joinGame;

    [SerializeField]
    private GameObject createGame;

    public GamesListController gamesList;
    public GameObject options;
    public float SizeClosed;
    public float SizeOpen;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        width = rectTransform.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivatePanel(int subMenuPanel)
    {
        switch ((SubMenuPanels) subMenuPanel)
        {
            case SubMenuPanels.PublicGames:
                ActivatePanel(publicGames);
                break;
            case SubMenuPanels.JoinGame:
                ActivatePanel(joinGame);
                break;
            case SubMenuPanels.CreateGame:
                ActivatePanel(createGame);
                break;
            default:
                Debug.LogError("[Activate Panel] Unexpected panel: " + subMenuPanel);
                break;
        }
    }

    private void ActivatePanel(GameObject panelObject)
    {
        if (activePanel == null)
        {
            ToggleVisibility();
            activePanel = panelObject;
            panelObject.SetActive(true);
        }
        else if (activePanel != panelObject)
        {
            activePanel.SetActive(false);
            activePanel = panelObject;
            panelObject.SetActive(true);
        }
    }

    public void ToggleVisibility()
    {
        if (rectTransform.sizeDelta.y == SizeClosed)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        rectTransform.sizeDelta = new Vector2(width, SizeOpen);
        options.SetActive(true);
        gamesList.Minify();
    }

    private void Hide()
    {
        rectTransform.sizeDelta = new Vector2(width, SizeClosed);
        options.SetActive(false);
        gamesList.MakeListGreatAgain();
    }
}
