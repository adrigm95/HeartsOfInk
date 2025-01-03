using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePlayerChange : MonoBehaviour
{
    private string previousSceneKey = "PreviousScene";

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString(previousSceneKey, SceneManager.GetActiveScene().name);
    }
    public void ChangeToScene(string sceneName)
    {
        PlayerPrefs.SetString(previousSceneKey, SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(sceneName);
    }
    public void ChangeToPreviousScene()
    {
        string previousScene = PlayerPrefs.GetString(previousSceneKey, string.Empty);
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
    }
}
