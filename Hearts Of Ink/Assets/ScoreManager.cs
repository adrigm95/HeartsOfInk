using Assets.Scripts.Data;
using NETCoreServer.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float width;
    private RectTransform rectTransform;
    private GameObject activePanel;
    private StatisticsController statisticsControllerInstance;

    public float SizeClosed;
    public float SizeOpen;
    public GameObject scorePanel;
    public TMPro.TextMeshProUGUI playerID;
    public TMPro.TextMeshProUGUI score;
    public TMPro.TextMeshProUGUI[] playerTexts;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        width = rectTransform.sizeDelta.x;
        statisticsControllerInstance = FindObjectOfType<StatisticsController>();

        if (statisticsControllerInstance == null)
        {
            Debug.Log("StatisticsController not found.");
        }
        else
        {
            UpdatePlayerTexts();
        }
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
    //This method will be used to show the same faction if the troop is an ally, in the score txt
    public bool IsAlly(Player factionId, Player factionId2)
    {
        FactionStatistics faction1 = statisticsControllerInstance.GetFaction(factionId);
        FactionStatistics faction2 = statisticsControllerInstance.GetFaction(factionId2);
        //Check if both factions are the same
        return faction1 != null && faction2 != null && faction1 == faction2;
    }
    public void SetPlayerID(Player factionId)
    {
        playerID.text = factionId.ToString();
    }    
    public void UpdatePlayerTexts()
    {
        for (int i = 0; i < playerTexts.Length; i++)
        {
            Player currentPlayer = 
                (Player)System.Enum.Parse(typeof(Player), playerTexts[i].text.Split(':')[0]);
            FactionStatistics currentFaction = statisticsControllerInstance.GetFaction(currentPlayer);

            if (currentFaction != null)
            {
                playerTexts[i].text = currentPlayer.ToString() + ": " + currentFaction.Player.ToString();
            }
            else
            {
                Debug.Log("Faction not found.");
            }
        }
    }
}
