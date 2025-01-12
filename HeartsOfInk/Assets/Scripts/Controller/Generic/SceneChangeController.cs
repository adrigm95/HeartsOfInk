using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeController : MonoBehaviour
{
    private string previousSceneKey = "PreviousScene";

    /// <summary>
    /// A button to go to multiplayer lobby scene.
    /// </summary>
    public Transform multiplayerButton;

    /// <summary>
    /// A button to go to credits scene.
    /// </summary>
    public Transform creditsButton;

    /// <summary>
    /// A button to return to main menu scene.
    /// </summary>
    public Transform mainMenuButton;

    /// <summary>
    /// A button to return to main menu scene.
    /// </summary>
    public Transform exitToMenuButton;

    /// <summary>
    /// A button to go to start scene.
    /// </summary>
    public Transform startGame;

    /// <summary>
    /// A button to go to options scene.
    /// </summary>
    public Transform optionsButton;

    /// <summary>
    /// A button to go to map editor scene.
    /// </summary>
    public Transform mapEditorButton;

    /// <summary>
    /// A button to go to login scene.
    /// </summary>
    public Transform loginButton;

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
        GameUpdaterScene = 9,
        Login = 10
    }

    /// <summary>
    /// Used for change scene when specific button is pressed and no specific logic is needed.
    /// </summary>
    /// <param name="orderButton"></param>
    public void ChangeScene(Transform orderButton)
    {
        PlayerPrefs.SetString(previousSceneKey, SceneManager.GetActiveScene().name);

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
        else if (AreEquals(orderButton, loginButton))
        {
            SceneManager.LoadScene(Convert.ToInt32(Scenes.Login));
        }
        else
        {
            Debug.LogWarning("Order button unknown: " + orderButton.name);
        }
    }

    /// <summary>
    /// Used when specific logic is needed for change scene or when change scene isn't started from an user interaction.
    /// </summary>
    /// <param name="newScene"></param>
    public void ChangeScene(Scenes newScene)
    {
        PlayerPrefs.SetString(previousSceneKey, SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(Convert.ToInt32(newScene));
    }

    public void ChangeToPreviousScene()
    {
        string previousScene = PlayerPrefs.GetString(previousSceneKey, string.Empty);
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
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
