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
            scoreTexts[i].text = "Facción " + (i+1) + ": " + scoreTotal[i];
        }
    }
}
