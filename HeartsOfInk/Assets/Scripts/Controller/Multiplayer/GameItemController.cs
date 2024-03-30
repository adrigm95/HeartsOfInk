using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameItemController : MonoBehaviour
{
    private GamesListController gamesListController;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        gamesListController = this.transform.GetComponentInParent<GamesListController>();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => SetSelected());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetSelected()
    {
        Debug.Log("OnMouseDown - Game Item Clicked");
        gamesListController.SetGameAsSelected(this);
    }
}
