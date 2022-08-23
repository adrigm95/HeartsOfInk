using UnityEngine;

public class EditorPauseController : MonoBehaviour
{
    GameObject pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        pausePanel = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeInHierarchy);
        }
    }
}
