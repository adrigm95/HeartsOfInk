using Assets.Scripts.Data;
using Assets.Scripts.Data.Literals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalEndgameController : MonoBehaviour
{
    private StatisticsController statisticsController;
    private SceneChangeController sceneChangeController;
    public Text resultTitle;
    public Transform btnGoToMenu;

    // Start is called before the first frame update
    void Start()
    {
        statisticsController = FindObjectOfType<StatisticsController>();
        sceneChangeController = FindObjectOfType<SceneChangeController>();

        // Puede ser null solo en caso de iniciarse el juego directamente en la pantalla.
        if (statisticsController != null)
        {
            SetResultTitle();
        }
        else
        {
            Debug.LogWarning("statisticsController is null");
        }
    }

    private void SetResultTitle()
    {
        FactionStatistics playerFactionStatistics;
        int playerFactionId = PlayerPrefs.GetInt(PlayerPrefsData.PlayerFactionIdKey, 1);

        playerFactionStatistics = statisticsController.GetFaction((Faction.Id)playerFactionId);

        if (playerFactionStatistics.CitiesAtEnd == 0)
        {
            resultTitle.text = "Derrota";
        }
        else
        {
            resultTitle.text = "Victoria";
        }
    }

    public void GoBackToMenu()
    {
        if (statisticsController != null)
        {
            Destroy(statisticsController.gameObject);
        }
        
        sceneChangeController.ChangeScene(btnGoToMenu);
    }
}
