using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorEntitiesController : MonoBehaviour
{
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
        
    }

    public void SetValues(SelectionModel selection)
    {
        if (selection.SelectionObjects.Count > 0)
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

    public void SetOwner(byte mapSocketId)
    {
        MapPlayerModel playerModel = logicController.GetSocketOwner(mapSocketId);
        ownerColor.color = ColorUtils.GetColorByString(playerModel.Color);
    }
}
