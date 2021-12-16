using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuPanelController : MonoBehaviour
{
    private float width;
    private RectTransform rectTransform;

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
