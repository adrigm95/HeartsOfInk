using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data;
using NETCoreServer.Models;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float width;
    private RectTransform rectTransform;
    private GameObject activePanel;

    public float SizeClosed;
    public float SizeOpen;
    public GameObject scorePanel;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        width = rectTransform.sizeDelta.x;

        ActivatePanel(scorePanel);
    }

    // Update is called once per frame
    void Update() { }

    private void ActivatePanel(GameObject gameObject)
    {
        if (activePanel != null)
        {
            activePanel.SetActive(false);
            activePanel.SetActive(true);
        }
        else if (activePanel == null)
        {
            ToggleVisibility();
        }
    }

    public void ToggleVisibility()
    {
        if (rectTransform.sizeDelta.y == SizeClosed)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        rectTransform.sizeDelta = new Vector2(width, SizeOpen);
        scorePanel.SetActive(true);
    }

    private void Hide()
    {
        rectTransform.sizeDelta = new Vector2(width, SizeClosed);
        scorePanel.SetActive(false);
    }
}
