using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    public SceneChangeController sceneChangeController;
    public float Speed;
    public float EndPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float newYPosition = transform.position.y + Speed * Time.deltaTime;

        if (newYPosition > EndPosition)
        {
            Debug.Log("Changing scene, newYPosition: " + newYPosition);
            sceneChangeController.ChangeScene(SceneChangeController.Scenes.MainMenu);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
        }
    }
}
