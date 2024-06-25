using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    private float width;
    private RectTransform rectTransform;

    /// <summary>
    /// Listado de puntuaciones de cada jugador. La posición en la lista se corresponde con el MapPlayerSlotId.
    /// </summary>
    private List<int> scoreTotal;

    [SerializeField]
    private GlobalLogicController globalLogicController;

    /// <summary>
    /// Listado con todas las líneas de puntuación.
    /// </summary>
    public List<TextMeshProUGUI> scoreTexts;

    /// <summary>
    /// Tamaño que tiene el panel cuando no está desplegado.
    /// </summary>
    public float SizeClosed;

    /// <summary>
    /// Tamaño mínimo que tiene el panel cuando está desplegado.
    /// </summary>
    public float MinSizeOpen;

    /// <summary>
    /// Referencia al panel de puntuaciones.
    /// </summary>
    public GameObject scorePanel;

    /// <summary>
    /// Tamaño que ocupa una línea de puntuación.
    /// </summary>
    public int ScoreLineSize;

    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        width = rectTransform.sizeDelta.x;

        Hide();
        scoreTotal = new List<int>() { 0 }; // Siempre va a haber al menos una línea de puntuación, por eso agregamos un valor inicial, para que la lista tenga un registro.
    }

    void Update()
    {
        GameObject[] troops = GameObject.FindGameObjectsWithTag(Tags.Troop);

        ResetScore();
        CalculateScore(troops);
        UpdateScoresInFront();
    }

    /// <summary>
    /// Actualizamos los textos del panel indicando nombre y puntuación.
    /// </summary>
    private void UpdateScoresInFront()
    {
        for (int i = 0; i < scoreTotal.Count; i++)
        {
            Player player = globalLogicController.GetPlayer((byte)i);
            Color color = ColorUtils.GetColorByString(player.Color);
            scoreTexts[i].text = player.Name + ": " + scoreTotal[i];
            scoreTexts[i].GetComponent<TextMeshProUGUI>().color = color;
        }
    }

    /// <summary>
    /// Calcula la puntuación para los distintos jugadores y añade al panel los que falten.
    /// </summary>
    /// <param name="troops"> Array con todas las tropas existentes en la partida.</param>
    private void CalculateScore(GameObject[] troops)
    {
        foreach (GameObject troop in troops)
        {
            TroopController troopController = troop.GetComponent<TroopController>();
            if (troopController != null)
            {
                int factionIndex = troopController.troopModel.Player.MapPlayerSlotId;

                if (factionIndex >= scoreTotal.Count)
                {
                    InstantiateScoreLine(factionIndex);
                }

                scoreTotal[factionIndex] += troopController.troopModel.Units;
            }
        }
    }

    /// <summary>
    /// Añade una facción más al panel de puntuaciones.
    /// </summary>
    private void InstantiateScoreLine(int factionIndex)
    {
        while (scoreTexts.Count <= factionIndex)
        {
            GameObject newLine = Instantiate(scoreTexts[0].gameObject, scoreTexts[0].gameObject.transform.parent);
            Debug.Log("Instantiated object: " + newLine.name + " parent: " + newLine.transform.parent.name);

            RectTransform rectTransform = newLine.GetComponent<RectTransform>();
            float newYPosition = rectTransform.localPosition.y - (scoreTexts.Count * ScoreLineSize);
            Vector3 newPosition = new Vector3(rectTransform.localPosition.x, newYPosition, rectTransform.localPosition.z);
            rectTransform.localPosition = newPosition;

            scoreTexts.Add(newLine.GetComponent<TextMeshProUGUI>());
            scoreTotal.Add(0);
        }

        Debug.Log($"InstantiateScoreLine. Current size of list: {scoreTexts.Count}:{scoreTotal.Count}; factionIndex required {factionIndex}");
    }

    /// <summary>
    /// Reseteo a cero de la puntuación.
    /// </summary>
    private void ResetScore()
    {
        for (int index = 0; index < scoreTotal.Count; index++)
        {
            scoreTotal[index] = 0;
        }
    }

    /// <summary>
    /// Controla si se tiene que mostrar u ocultar el panel de puntuaciones.
    /// </summary>
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

    /// <summary>
    /// Despliega el panel de puntuaciones.
    /// </summary>
    private void Show()
    {
        float sizeOpen = MinSizeOpen + (scoreTexts.Count * ScoreLineSize);
        rectTransform.sizeDelta = new Vector2(width, sizeOpen);
        scorePanel.SetActive(true);
    }

    /// <summary>
    /// Oculta el panel de puntuaciones.
    /// </summary>
    private void Hide()
    {
        rectTransform.sizeDelta = new Vector2(width, SizeClosed);
        scorePanel.SetActive(false);
    }
}
