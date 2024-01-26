using Assets.Scripts.Data;
using NETCoreServer.Models;
using System;
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
            return;
        }
        else
        {
            ActivatePanel(scorePanel);
            UpdateTexts();
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
    public bool IsAlly(Player factionId, Player factionId2)
    {
        FactionStatistics faction1 = statisticsControllerInstance.GetFaction(factionId);
        FactionStatistics faction2 = statisticsControllerInstance.GetFaction(factionId2);
        return faction1 != null && faction2 != null && faction1 == faction2;
    }
    public void SetPlayerID(Player factionId)
    {
        playerID.text = factionId.ToString();
    }

    public void UpdateTexts()
    {
        for (int i = 0; i < playerTexts.Length; i++)
        {
            if (playerTexts[i].text != null)
            {
                string[] splitText = playerTexts[i].text.Split(':');

                if (splitText.Length >= 1)
                {
                    string playerText = splitText[0].Trim();
                    FactionStatistics currentFaction = statisticsControllerInstance.GetFactionByName(playerText);

                    if (currentFaction != null)
                    {
                        playerTexts[i].text = playerText + ": " + currentFaction.Player.ToString();
                    }
                    else
                    {
                        Debug.Log("Faction not found.");
                    }
                }
                else
                {
                    Debug.LogError("Invalid player text format: " + playerTexts[i].text);
                }
            }
        }
    }
}

