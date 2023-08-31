using Assets.Scripts.Data;
using Assets.Scripts.Data.Constants;
using Assets.Scripts.Data.ServerModels.Constants;
using Assets.Scripts.DataAccess;
using HeartsOfInk.SharedLogic;
using LobbyHOIServer.Models.MapModels;
using NETCoreServer.Models;
using NETCoreServer.Models.In;
using NETCoreServer.Models.Out;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameController : MonoBehaviour
{
    private List<MapModelHeader> availableMaps;
    private ConfigGameController configGameController;
    [SerializeField]
    private InfoPanelController infoPanelController;
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
        availableMaps = MapDAC.GetAvailableMaps(GlobalConstants.RootPath, true);
        availableMaps.ForEach(map => cbMaps.options.Add(new Dropdown.OptionData(map.DisplayName)));
        cbMaps.RefreshShownValue();
        cbMaps.onValueChanged.AddListener(delegate { OnValueChange(); });
    }

    public async void CreateGame()
    {
        WebServiceCaller<CreateGameModelIn, CreateGameModelOut> wsCaller = new WebServiceCaller<CreateGameModelIn, CreateGameModelOut>();
        HOIResponseModel<CreateGameModelOut> response;

        CreateGameModelIn newGame = new CreateGameModelIn
        {
            
            IsPublic = !checkIsPrivate.isOn,
            Name = gameNameText.text,
            MapId = availableMaps.Find(map => map.DisplayName == cbMaps.options[cbMaps.value].text).MapId,
            PlayerName = creatorNick.text
        };
        
        response = await wsCaller.GenericWebServiceCaller(ApiConfig.LobbyHOIServerUrl, Method.POST, LobbyHOIControllers.CreateGame, newGame);

        if (string.IsNullOrEmpty(newGame.Name) || string.IsNullOrEmpty(newGame.PlayerName))
        {
            infoPanelController.DisplayMessage("Check all the fields", "Cannot be empty data.");
        }
        else if (response.internalResultCode == InternalStatusCodes.OKCode)
        {
            CreateGameModelOut responseModel = response.serviceResponse;
            
            configGameController.GameCreatedByHost(responseModel.gameKey, newGame.MapId);
            EnableDisableCreateGame(false);
            configGamePanel.SetActive(true);
            AddPlayerToDropdowns();
        }
        else
        {
            infoPanelController.DisplayMessage("Unexpected error", "Unexpected error on join game: " + response.internalResultCode);
        }
    }

    private void OnValueChange()
    {
        MapController.Instance.UpdateMap(availableMaps.Find(item => item.DisplayName == cbMaps.options[cbMaps.value].text).SpritePath);
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
