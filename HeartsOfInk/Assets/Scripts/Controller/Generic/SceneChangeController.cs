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
    public Transform optionsButton;
    public Transform mapEditorButton;

    public enum Scenes
    {
        RawgenLogo = 0,
        MainMenu = 1,
        InGameScene = 2,
        Credits = 3,
        Endgame = 4,
        Multiplayer = 5,
        MapEditor = 6,
        AcceptPolicy = 7,
        Options = 8,
        GameUpdaterScene = 9
    }

    public void DirectChangeScene(Scenes scene)
    {
        SceneManager.LoadScene(Convert.ToInt32(scene));
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
        else if (AreEquals(orderButton, mapEditorButton))
        {
            SceneManager.LoadScene(Convert.ToInt32(Scenes.MapEditor));
        }
        else if (AreEquals(orderButton, optionsButton))
        {
            SceneManager.LoadScene(Convert.ToInt32(Scenes.Options));
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
        Debug.Log("Exit game called");
        Application.Quit();
    }

    private bool AreEquals(Transform notNull, Transform canBeNull)
    {
        return canBeNull != null && notNull.Equals(canBeNull);
    }
}
