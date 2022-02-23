using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeController : MonoBehaviour
{
    public Transform multiplayerButton;
    public Transform creditsButton;
    public Transform mainMenuButton;
    public Transform exitToMenuButton;
    public Transform startGame;

    public enum Scenes
    {
        MainMenu = 0,
        InGameScene = 1,
        Credits = 2,
        Endgame = 3,
        Multiplayer = 4,
    }

    public void ChangeScene(Transform orderButton)
    {
        if (AreEquals(orderButton, creditsButton))
        {
            SceneManager.LoadScene(Convert.ToInt32(Scenes.Credits));
        }
        else if (AreEquals(orderButton, mainMenuButton) 
                 || AreEquals(orderButton, exitToMenuButton))
        {
            SceneManager.LoadScene(Convert.ToInt32(Scenes.MainMenu));
        }
        else if (AreEquals(orderButton, startGame))
        {
            SceneManager.LoadScene(Convert.ToInt32(Scenes.InGameScene));
        }
        else if (AreEquals(orderButton, multiplayerButton))
        {
            SceneManager.LoadScene(Convert.ToInt32(Scenes.Multiplayer));
        }
        else
        {
            Debug.LogWarning("Order button unknown: " + orderButton.name);
        }
    }

    public void ChangeScene(Scenes newScene)
    {
        SceneManager.LoadScene(Convert.ToInt32(newScene));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private bool AreEquals(Transform notNull, Transform canBeNull)
    {
        return canBeNull != null && notNull.Equals(canBeNull);
    }
}
