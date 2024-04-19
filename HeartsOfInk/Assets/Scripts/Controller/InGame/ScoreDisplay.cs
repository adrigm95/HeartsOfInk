using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    //GameObject[] cities = GameObject.FindGameObjectsWithTag("City");
    public TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[4]; // Asignar en el editor de unity
    private int[] scoreTotal = new int[4]; // se asume que hay 4 facciones pero habría que ajustarlo para que las detecte automáticamente
    public FactionScore factionScore;

    void Start()
    {
        for (int i = 0; i < scoreTotal.Length; i++)
        {
            if (scoreTexts[i] != null)
            {
                scoreTexts[i].text = "Facción " + (i+1) + ": " + scoreTotal[i];
            }
            else
            {
                Debug.LogError("scoreTexts[" + (i+1) + "] nulo");
            }
        }
    }

    void Update()
    {
        GameObject[] cities = GameObject.FindGameObjectsWithTag("City");
        foreach (GameObject city in cities)
        {
            CityController cityController = city.GetComponent<CityController>();
            if (cityController != null)
            {
                int factionIndex = cityController.Faction;
                scoreTotal[factionIndex] += cityController.GetTotalUnitsInZone();
            }
        }

        for (int i = 0; i < scoreTotal.Length; i++)
        {
            scoreTexts[i].text = "Facción " + (i+1) + ": " + scoreTotal[i];
        }
    }
}
