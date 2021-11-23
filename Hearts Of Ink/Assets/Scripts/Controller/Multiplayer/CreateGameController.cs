using Assets.Scripts.Utils;
using NETCoreServer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameController : MonoBehaviour
{
    private GamesListController gamesListPanel;
    private ConfigGameController configGameController;
    public Button btnCreateGame;
    public Text gameNameText;
    public Text creatorNick;
    public GameObject configGamePanel;

    void Start()
    {
        gamesListPanel = FindObjectOfType<GamesListController>();
        configGameController = configGamePanel.GetComponent<ConfigGameController>();
    }

    private void Update()
    {

    }

    public async void CreateGame()
    {
        WebServiceCaller<CreateGameModel, BasicGameInfo> wsCaller = new WebServiceCaller<CreateGameModel, BasicGameInfo>();
        HOIResponseModel<BasicGameInfo> response;

        CreateGameModel newGame = new CreateGameModel
        {
            isPublic = true,
            name = gameNameText.text
        };

        response = await wsCaller.GenericWebServiceCaller(Method.POST, "api/CreateGame", newGame);

        if (response.internalResultCode == InternalStatusCodes.OKCode)
        {
            configGameController.GameCreatedByHost(response.serviceResponse);
            gamesListPanel.AddGameToPanel(response.serviceResponse, true);
            EnableDisableCreateGame(false);
            configGamePanel.SetActive(true);
            AddPlayerToDropdowns();
        }
    }

    public void EnableDisableCreateGame(bool enable)
    {
        btnCreateGame.interactable = enable;
        gameNameText.gameObject.SetActive(enable);
    }

    private void AddPlayerToDropdowns()
    {
        foreach (Transform childConfigPanel in configGamePanel.transform)
        {
            foreach (Transform childFile in childConfigPanel)
            {
                DropdownController dropdownController = childFile.GetComponent<DropdownController>();

                if (dropdownController != null && dropdownController.dropType == DropdownController.DropType.PlayerSelector)
                {
                    Dropdown dropdown = childFile.GetComponent<Dropdown>();
                    Dropdown.OptionData newOption = new Dropdown.OptionData(creatorNick.text);

                    dropdown.options.Add(newOption);

                    if (dropdownController.isDefaultFaction)
                    {
                        dropdown.value = dropdown.options.Count - 1;
                    }
                }
            }
        }
    }
}
