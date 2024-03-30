using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using LobbyHOIServer.Models.MapModels;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorEntitiesController : MonoBehaviour
{
    private SelectionModel selectionModel;
    public MapEditorLogicController logicController;
    public Image ownerColor;
    public InputField entityName;
    public Toggle isCapital;
    public InputField troopSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEntity();
    }

    public void SetValues(SelectionModel selection)
    {
        if (selection.SelectionObjects != null && selection.SelectionObjects.Count > 0)
        {
            GameObject selected = selection.SelectionObjects[0];

            if (selection.SelectionType == typeof(EditorCityController))
            {
                SetCitiesValues(selected.GetComponent<EditorCityController>());
            }
            else if (selection.SelectionType == typeof(EditorTroopController))
            {
                SetTroopsValues(selected.GetComponent<EditorTroopController>());
            }
            else
            {
                Debug.LogError("Unexpected selection type on SetValues method.");
            }

            selectionModel = selection;
        }
    }

    public void SetCitiesValues(EditorCityController cityModel)
    {
        isCapital.gameObject.SetActive(true);
        troopSize.transform.parent.gameObject.SetActive(false);

        SetOwner((byte)cityModel.ownerSocketId);
        entityName.text = cityModel.gameObject.name;
        isCapital.isOn = cityModel.isCapital;
    }

    public void SetTroopsValues(EditorTroopController troopModel)
    {
        TextMeshProUGUI unitsText;

        isCapital.gameObject.SetActive(false);
        troopSize.transform.parent.gameObject.SetActive(true);

        SetOwner((byte)troopModel.ownerSocketId);
        unitsText = troopModel.GetComponent<TextMeshProUGUI>();
        troopSize.text = unitsText.text;
        entityName.text = troopModel.gameObject.name;
    }

    public void CbFaction_OnValueChange()
    {
        string parsedColor = ColorUtils.GetStringByColor(ownerColor.color);
        MapPlayerModel playerModel = logicController.GetNextOwner(parsedColor);
        Color newColor = ColorUtils.GetColorByString(playerModel.Color);
        ownerColor.color = newColor;
    }

    private void SetOwner(byte mapSocketId)
    {
        MapPlayerModel playerModel = logicController.GetSocketOwner(mapSocketId);
        ownerColor.color = ColorUtils.GetColorByString(playerModel.Color);
    }

    private MapPlayerModel GetOwner(Color ownerColor)
    {
        string parsedColor = ColorUtils.GetStringByColor(ownerColor);
        return logicController.GetSocketOwner(parsedColor);
    } 

    private void UpdateEntity()
    {
        if (selectionModel != null)
        {
            MapPlayerModel owner = GetOwner(ownerColor.color);
            SpriteRenderer spriteRenderer = null;

            if (selectionModel.SelectionType == typeof(EditorCityController))
            {
                string spriteName;
                EditorCityController editorCityController = selectionModel.SelectionObjects.FirstOrDefault().GetComponent<EditorCityController>();

                editorCityController.isCapital = isCapital.isOn;
                editorCityController.name = entityName.text;
                editorCityController.ownerSocketId = owner.MapSocketId;

                spriteName = editorCityController.isCapital ? "Textures/Capital" : "Textures/City";
                spriteRenderer = editorCityController.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = Resources.Load<Sprite>(spriteName);
                spriteRenderer.color = ownerColor.color;
            }
            else if (selectionModel.SelectionType == typeof(EditorTroopController))
            {
                TextMeshProUGUI unitsText;
                EditorTroopController editorTroopController = selectionModel.SelectionObjects.FirstOrDefault().GetComponent<EditorTroopController>();

                editorTroopController.name = entityName.text;
                editorTroopController.ownerSocketId = owner.MapSocketId;

                unitsText = editorTroopController.GetComponent<TextMeshProUGUI>();
                unitsText.text = troopSize.text;
                unitsText.color = ownerColor.color;
            }
            else
            {
                selectionModel = null;
            }
        }
    }
}
