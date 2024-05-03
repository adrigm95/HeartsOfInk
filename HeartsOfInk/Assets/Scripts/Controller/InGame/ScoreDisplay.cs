using System.Collections.Generic;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    //GameObject[] cities = GameObject.FindGameObjectsWithTag("City");
    public TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[4]; // Asignar en el editor de unity
    private float width;
    private RectTransform rectTransform;
    private GameObject activePanel;

    public float SizeClosed;
    public float SizeOpen;
    public GameObject scorePanel;
    private int[] scoreTotal = new int[4]; // se asume que hay 4 facciones pero habría que ajustarlo para que las detecte automáticamente
    public FactionScore factionScore;
    public GlobalLogicController globalLogicController;
    void Start()
    {
    
        rectTransform = transform.GetComponent<RectTransform>();
        width = rectTransform.sizeDelta.x;

        ActivatePanel(scorePanel);
        for (int i = 0; i < scoreTotal.Length; i++)
        {
            if (scoreTexts[i] != null)
            {
                scoreTexts[i].text = "Facción " + (i + 1) + ": " + scoreTotal[i];
            }
            else
            {
                Debug.LogError("scoreTexts[" + (i + 1) + "] nulo");
            }
        }
    }

    void Update()
    {
        GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");

        for (int index = 0; index < scoreTotal.Length; index++)
        {
            scoreTotal[index] = 0;
        }

        foreach (GameObject troop in troops)
        {
            TroopController troopController = troop.GetComponent<TroopController>();
            if (troopController != null)
            {
                int factionIndex = troopController.troopModel.Player.MapSocketId;
                
                scoreTotal[factionIndex] += troopController.troopModel.Units;
            }
        }

        for (int i = 0; i < scoreTotal.Length; i++)
        {
            Player player = globalLogicController.GetPlayer((byte)i);
            Color color = ColorUtils.GetColorByString(player.Color);
            scoreTexts[i].text = player.Name  + ": " + scoreTotal[i];
            scoreTexts[i].GetComponent<TextMeshProUGUI>().color = color;
        }
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
}
