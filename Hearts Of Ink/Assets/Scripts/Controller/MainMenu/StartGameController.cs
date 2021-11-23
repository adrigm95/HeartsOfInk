using Assets.Scripts.Data;
using Assets.Scripts.Data.Literals;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour
{
    private SceneChangeController sceneChangeController;
    public Transform factionDropdownsHolder;

    private void Start()
    {
        sceneChangeController = FindObjectOfType<SceneChangeController>();
    }

    public void StartGame()
    {
        GameModel gameModel = new GameModel(string.Empty);

        PlayerPrefs.SetString(PlayerPrefsData.GametypeKey, PlayerPrefsData.GametypeSingle);

        AddFactionIaId(gameModel);
        sceneChangeController.ChangeScene(transform);
    }

    private void AddFactionIaId(GameModel gameModel)
    {
        foreach (Transform holderChild in factionDropdownsHolder)
        {
            if (holderChild.name.StartsWith(GlobalConstants.FactionLineStart))
            {
                Faction faction = new Faction();
                string factionId = holderChild.name.Split('_')[1];
                Dropdown iaSelector = holderChild.GetComponentInChildren<Dropdown>();

                faction.FactionId = (Faction.Id) (Convert.ToInt32(factionId));

                PlayerPrefs.SetInt(factionId, iaSelector.value);
            }
        }
    }
}
