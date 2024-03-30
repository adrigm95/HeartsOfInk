using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelController : MonoBehaviour
{
    private UIAnimator uiAnimator;
    public GameObject otherPanel;
    public bool isOrigin;

    // Start is called before the first frame update
    void Start()
    {
        uiAnimator = FindObjectOfType<Canvas>().GetComponent<UIAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToOther()
    {
        transform.gameObject.SetActive(false);
        otherPanel.SetActive(true);

        if (isOrigin)
        {
            uiAnimator.MoveToTarget();
        }
        else
        {
            uiAnimator.MoveToOrigin();
        }
    }
}
