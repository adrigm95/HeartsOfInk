using UnityEngine;

public class TimedSceneChange : MonoBehaviour
{
    private float sceneStart;
    public float sceneLapse;
    public SceneChangeController.Scenes targetScene;

    // Start is called before the first frame update
    void Start()
    {
        sceneStart = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneStart == -1)
        {
            Debug.Log("Starting counter");
            sceneStart = Time.realtimeSinceStartup;
        }
        else if (Time.realtimeSinceStartup > (sceneStart + sceneLapse))
        {
            Debug.Log("Changing scene");
            SceneChangeController sceneChangeController = this.GetComponent<SceneChangeController>();
            sceneChangeController.DirectChangeScene(targetScene);
        }
    }
}
