﻿using Assets.Scripts.Data;
using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.DataAccess;
using NETCoreServer.Models;
using NETCoreServer.Models.In;
using NETCoreServer.Models.Out;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameController : MonoBehaviour
{
    private List<MapModel> availableMaps;
    private ConfigGameController configGameController;
    public GamesListController gamesListPanel;
    public Button btnCreateGame;
    public InputField gameNameText;
    public Dropdown cbMaps;
    public InputField creatorNick;
    public GameObject configGamePanel;
    public Toggle checkIsPrivate;

    void Start()
    {
        configGameController = configGamePanel.GetComponent<ConfigGameController>();
        availableMaps = MapDAC.GetAvailableMaps(true);
        availableMaps.ForEach(map => cbMaps.options.Add(new Dropdown.OptionData(map.DisplayName)));
        cbMaps.RefreshShownValue();
    }

    private void Update()
    {

    }

    public async void CreateGame()
    {
        WebServiceCaller<CreateGameModelIn, CreateGameModelOut> wsCaller = new WebServiceCaller<CreateGameModelIn, CreateGameModelOut>();
        HOIResponseModel<CreateGameModelOut> response;

        CreateGameModelIn newGame = new CreateGameModelIn
        {
            isPublic = !checkIsPrivate.isOn,
            name = gameNameText.text,
            mapName = availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text).DefinitionName,
            playerName = creatorNick.text
        };

        response = await wsCaller.GenericWebServiceCaller(Method.POST, LobbyHOIControllers.CreateGame, newGame);

        if (response.internalResultCode == InternalStatusCodes.OKCode)
        {
            BasicGameInfo basicGameInfo = BasicGameInfo.FromCreateGameService(newGame, response.serviceResponse);

            configGameController.gameObject.SetActive(true);
            configGameController.GameCreatedByHost(response.serviceResponse.gameKey);
            gamesListPanel.AddGameToPanel(basicGameInfo, true);
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
