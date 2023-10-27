using NETCoreServer.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float width;
    private RectTransform rectTransform;
    private GameObject activePanel;

    public float SizeClosed;
    public float SizeOpen;
    public GameObject scorePanel;
    public TMPro.TextMeshProUGUI playerID;
    public TMPro.TextMeshProUGUI score;
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
    private void ActivatePanel(GameObject gameObject)
    {
        if (activePanel != null)
        {
            activePanel.SetActive(false);
            activePanel.SetActive(true);
        }
        else if (activePanel == null)
        {
            activePanel.SetActive(true);
            ToggleVisibility();
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
        scorePanel.SetActive(true);
    }
    private void Hide()
    {
        rectTransform.sizeDelta = new Vector2(width, SizeClosed);
        scorePanel.SetActive(false);
    }
    //Esta función servirá para que si una tropa es aliada se muestre como si fuera la misma facción, en el txt de la puntuación
    public bool IsEnemy(Player factionId, Player factionId2)
    {
        return factionId != factionId2;
    }
    public void SetPlayerID(Player factionId)
    {
        playerID.text = factionId.ToString();
    }
    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }



   


}
