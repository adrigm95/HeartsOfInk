using Assets.Scripts.Controller.InGame;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using Boo.Lang;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class MapEditorLogicController : MonoBehaviour
{
    private const string PrefabCity = "Prefabs/EditorCityPrefab";
    private const string PrefabCapital = "Prefabs/EditorCapital";
    private const string PrefabTroop = "Prefabs/EditorTroop";

    private SelectionModel selection;
    public Transform citiesHolder;
    public Transform troopsHolder;
    public CameraController cameraController;
    public Toggle editMapEnabled;
    public Toggle citiesEnabled;
    public Toggle troopsEnabled;
    public Toggle selectionEnabled;
    public Toggle editEntitiesEnabled;
    public Toggle aditionEnabled;
    public Toggle moveEntitiesEnabled;
    public Toggle deleteEnabled;
    public EditorPanelController editorPanelController;
    public EditorEntitiesController editorEntitiesController;
    public float entityMovementSpeed;
    public float entitySpeedMultiplier;

    public bool AddingCities
    {
        get { return citiesEnabled.isOn; }
    }

    public bool AddingTroops
    {
        get { return troopsEnabled.isOn; }
    }

    // Start is called before the first frame update
    void Start()
    {
        selection = new SelectionModel();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUnitAnimation();
        UpdateMultiselect();
        InputManagement();
    }

    private void UpdateMultiselect()
    {
        selection.UpdateMultiselect(cameraController.ScreenToWorldPoint(), troopsHolder, -1);
    }

    public void InputManagement()
    {

        cameraController.movementEnabled = !moveEntitiesEnabled.isOn;
        if (moveEntitiesEnabled.isOn && selection.HaveObjectSelected)
        {
            float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? entitySpeedMultiplier : 1f;
            float xMovement = 0;
            float yMovement = 0;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                xMovement -= (entityMovementSpeed * speedMultiplier);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                xMovement += (entityMovementSpeed * speedMultiplier);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                yMovement += (entityMovementSpeed * speedMultiplier);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                yMovement -= (entityMovementSpeed * speedMultiplier);
            }

            foreach (GameObject gameObject in selection.SelectionObjects)
            {
                Vector3 position = gameObject.transform.position;
                position.x += xMovement;
                position.y += yMovement;
                gameObject.transform.position = position;
            }
        }

        if (selection.MultiselectOrigin != null)
        {
            if (Input.GetMouseButtonUp(KeyConstants.LeftClick))
            {
                selection.MultiselectOrigin = null;
            }
        }
    }

    public void ClickReceivedFromCity(EditorCityController city)
    {
        if (citiesEnabled.isOn)
        {
            EndSelection();
            selection.ChangeSelection(city.gameObject, typeof(EditorCityController));
            editorEntitiesController.SetValues(selection);
        }
    }

    public void ClickReceivedFromTroop(EditorTroopController troop)
    {
        if (troopsEnabled.isOn)
        {
            EndSelection();
            selection.ChangeSelection(troop.gameObject, typeof(EditorTroopController));
            editorEntitiesController.SetValues(selection);
        }
    }

    public void SetCityInCanvas(MapCityModel mapCityModel)
    {
        EditorCityController newObject;
        MapPlayerModel player;
        SpriteRenderer spriteRenderer;
        Vector3 position = VectorUtils.FloatVectorToVector3(mapCityModel.Position);
        string resourceName = mapCityModel.Type == 1 ? PrefabCity : PrefabCapital;

        newObject = ((GameObject)Instantiate(
                        Resources.Load(resourceName),
                        position,
                        citiesHolder.rotation,
                        citiesHolder)
                        ).GetComponent<EditorCityController>();

        spriteRenderer = newObject.GetComponent<SpriteRenderer>();
        newObject.name = mapCityModel.Name;
        newObject.isCapital = !Convert.ToBoolean(mapCityModel.Type);
        newObject.ownerSocketId = mapCityModel.MapSocketId;
        newObject.editorLogicController = this;
        player = editorPanelController.MapModel.Players.Find(p => p.MapSocketId == mapCityModel.MapSocketId);
        spriteRenderer.color = ColorUtils.GetColorByString(player.Color);
    }

    public void SetTroopInCanvas(MapTroopModel mapTroopModel)
    {
        EditorTroopController newObject;
        MapPlayerModel player;
        TextMeshProUGUI unitsText;
        Vector3 position = VectorUtils.FloatVectorToVector3(mapTroopModel.Position);

        newObject = ((GameObject)Instantiate(
                        Resources.Load(PrefabTroop),
                        position,
                        troopsHolder.rotation,
                        troopsHolder)
                        ).GetComponent<EditorTroopController>();

        unitsText = newObject.GetComponent<TextMeshProUGUI>();
        newObject.name = "Troop";
        newObject.ownerSocketId = mapTroopModel.MapSocketId;
        newObject.editorLogicController = this;
        player = editorPanelController.MapModel.Players.Find(p => p.MapSocketId == mapTroopModel.MapSocketId);
        unitsText.text = mapTroopModel.Units.ToString();
        unitsText.color = ColorUtils.GetColorByString(player.Color);
    }

    public void ChangeOption_OnClick(Toggle sender)
    {
        Debug.Log($"ChangeOption_OnClick - Sender: {sender}");
        if (sender.Equals(citiesEnabled))
        {
            if (citiesEnabled.isOn)
            {
                Debug.LogWarning("TODO: Enable cities selection");
            }
            else
            {
                Debug.LogWarning("TODO: Remove cities from selection");
            }
        }
        else if (sender.Equals(troopsEnabled))
        {
            if (troopsEnabled.isOn)
            {
                Debug.LogWarning("TODO: Enable troops selection");
            }
            else
            {
                Debug.LogWarning("TODO: Remove troops from selection");
            }
        }
        else if (sender.Equals(editMapEnabled))
        {
            editorPanelController.gameObject.SetActive(editMapEnabled.isOn);
        }
        else if (sender.Equals(editEntitiesEnabled))
        {
            editorEntitiesController.gameObject.SetActive(editEntitiesEnabled.isOn);
            editorEntitiesController.SetValues(selection);
        }
        else if (sender.Equals(deleteEnabled))
        {
            if (deleteEnabled.isOn)
            {
                DeleteSelection();
            }
        }
        else
        {
            Debug.Log($"ChangeOption_OnClick - Cities: {citiesEnabled}");
            Debug.Log($"ChangeOption_OnClick - Troops: {troopsEnabled}");
        }
    }

    public void ResetSelection()
    {
        selection.EndSelection();
    }

    public MapPlayerModel GetSocketOwner(byte mapSocketId)
    {
        Debug.Log($"Getting owner for {mapSocketId}");
        MapPlayerModel owner = editorPanelController.MapModel.Players.FirstOrDefault(player => player.MapSocketId == mapSocketId);

        if (owner != null)
        {
            Debug.Log($"Getted owner {owner.FactionId}");
        }
        else
        {
            Debug.LogWarning($"No owner getted for mapSocketId {mapSocketId}");
        }

        return owner;
    }

    public MapPlayerModel GetNextOwner(string ownerColor)
    {
        MapPlayerModel currentOwner = editorPanelController.MapModel.Players.FirstOrDefault(player => player.Color == ownerColor);
        byte nextSocketId = Convert.ToByte(currentOwner.MapSocketId + Convert.ToByte(1));
        MapPlayerModel nextOwner = editorPanelController.MapModel.Players.FirstOrDefault(player => player.MapSocketId == nextSocketId);

        if (nextOwner == null)
        {
            nextSocketId = 0;
            nextOwner = editorPanelController.MapModel.Players.FirstOrDefault(player => player.MapSocketId == nextSocketId);
        }

        while (nextOwner == null && editorPanelController.MapModel.Players.Any())
        {
            nextSocketId++;
            nextOwner = editorPanelController.MapModel.Players.FirstOrDefault(player => player.MapSocketId == nextSocketId);
        }

        Debug.Log($"Total players {editorPanelController.MapModel.Players.Count}");
        Debug.Log($"Getted owner {nextOwner.FactionId}");
        return nextOwner;
    }

    public MapPlayerModel GetSocketOwner(string ownerColor)
    {
        return editorPanelController.MapModel.Players.FirstOrDefault(player => player.Color == ownerColor);
    }

    private void EndSelection()
    {
        if (selection.HaveObjectSelected)
        {
            foreach (GameObject selectedTroopObject in selection.SelectionObjects)
            {
                IObjectAnimator selectedTroop = selectedTroopObject.GetComponent<IObjectAnimator>();
                if (selectedTroop != null)
                {
                    selectedTroop.EndAnimation();
                }
            }

            selection.SetAsNull();
        }
    }

    /// <summary>
    /// Actualiza el estado de la animación de parpadeo de la unidad seleccionada.
    /// </summary>
    public void UpdateUnitAnimation()
    {
        if (selection.HaveObjectSelected)
        {
            foreach (GameObject selectedObject in selection.SelectionObjects)
            {
                IObjectAnimator selectedAnimator = selectedObject.GetComponent<IObjectAnimator>();

                selectedAnimator?.Animate();
            }
        }
    }

    public void ClickReceivedFromMap(KeyCode mouseKeyPressed)
    {
        if (selectionEnabled.isOn)
        {
            SelectionFromMap(mouseKeyPressed);
        }
        else if (aditionEnabled.isOn)
        {
            AditionFromMap(mouseKeyPressed);
        }
    }

    private void SelectionFromMap(KeyCode mouseKeyPressed)
    {
        switch (mouseKeyPressed)
        {
            case KeyCode.Mouse0: // Left mouse button
                EndSelection();
                selection.StartMultiselect(cameraController.ScreenToWorldPoint(), typeof(IObjectSelectable));
                Debug.Log($"MultiselectOrigin assignated {selection.MultiselectOrigin}");
                break;
            default:
                Debug.Log($"Unexpected map click {mouseKeyPressed}");
                break;
        }
    }

    public void AditionFromMap(KeyCode mouseKeyPressed)
    {
        switch (mouseKeyPressed)
        {
            case KeyCode.Mouse0: // Left mouse button
                if (citiesEnabled.isOn)
                {
                    CreateNewCity();
                }
                else if (troopsEnabled.isOn)
                {
                    CreateNewTroop();
                }
                break;
            default:
                Debug.Log($"Unexpected map click {mouseKeyPressed}");
                break;
        }
    }

    private void CreateNewCity()
    {
        EditorCityController newObject;
        SpriteRenderer spriteRenderer;
        MapPlayerModel player;
        Vector3 mouseClickPosition;

        mouseClickPosition = cameraController.ScreenToWorldPoint();
        Debug.Log("Adding new city for position: " + mouseClickPosition);
        newObject = ((GameObject)Instantiate(
                            Resources.Load(PrefabCity),
                            mouseClickPosition,
                            citiesHolder.rotation,
                            citiesHolder)
                            ).GetComponent<EditorCityController>();

        spriteRenderer = newObject.GetComponent<SpriteRenderer>();
        player = editorPanelController.MapModel.Players[0];
        newObject.name = "New City";
        newObject.isCapital = false;
        newObject.ownerSocketId = player.MapSocketId;
        newObject.editorLogicController = this;
        spriteRenderer.color = ColorUtils.GetColorByString(player.Color);
    }

    private void CreateNewTroop()
    {
        EditorTroopController newObject;
        TextMeshProUGUI unitsText;
        MapPlayerModel player;
        Vector3 mouseClickPosition;

        mouseClickPosition = cameraController.ScreenToWorldPoint();
        Debug.Log("Adding new troop for position: " + mouseClickPosition);
        newObject = ((GameObject)Instantiate(
                            Resources.Load(PrefabTroop),
                            mouseClickPosition,
                            troopsHolder.rotation,
                            troopsHolder)
                            ).GetComponent<EditorTroopController>();

        unitsText = newObject.GetComponent<TextMeshProUGUI>();
        player = editorPanelController.MapModel.Players[0];
        newObject.name = "Troop";
        newObject.ownerSocketId = player.MapSocketId;
        newObject.editorLogicController = this;
        unitsText.text = Convert.ToString(GlobalConstants.DefaultCompanySize);
        unitsText.color = ColorUtils.GetColorByString(player.Color);
    }

    private void DeleteSelection()
    {
        Debug.Log("Start - DeleteSelection");
        if (selection.HaveObjectSelected)
        {
            foreach (GameObject selectedObject in selection.SelectionObjects)
            {
                Destroy(selectedObject);
            }

            selection.SetAsNull();
        }
        else
        {
            Debug.LogWarning("No objects selected");
        }

        selectionEnabled.isOn = true;
    }
}
