using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public FactionScore factionScore; // Asignar en el editor de unity
    private Text scoreText;

    void Start()
    {
        scoreText = GetComponent<Text>();
    }

    void Update()
    {
        scoreText.text = factionScore.GetTotalScore().ToString();
    }
}