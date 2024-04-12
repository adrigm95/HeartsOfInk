using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    //GameObject[] cities = GameObject.FindGameObjectsWithTag("City");

    public FactionScore factionScore; // Asignar en el editor de unity
    private Text scoreText;

    void Start()
    {
        scoreText = GetComponent<Text>();
    }

    void Update()
    {
    //GameObject[] cities = GameObject.FindGameObjectsWithTag("City");
    GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");
    int totalScore = 0;

    foreach (GameObject troop in troops)
    {
        TroopController troopController = troop.GetComponent<TroopController>();
        if (troopController != null)
        {
            totalScore += troopController.troopModel.Units; 
        }
    }

    scoreText.text = totalScore.ToString();
    }
}