using Assets.Scripts.Data;
using Assets.Scripts.DataAccess;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private static MapController _instance;
    private GlobalLogicController globalLogic;
    private MapEditorLogicController editorLogic;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Transform citiesHolder;
    [SerializeField]
    private Transform troopsHolder;
    public static MapController Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            globalLogic = FindObjectOfType<GlobalLogicController>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            editorLogic = FindObjectOfType<MapEditorLogicController>();
        }
    }

    public void UpdateMap(string spritePath)
    {
        CleanMap();
        spriteRenderer.sprite = MapSpriteDAC.LoadMapSprite(spritePath);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(KeyConstants.LeftClick))
        {
            Debug.Log("Map controller - Left mouse button down detected");
            if (globalLogic != null)
            {
                globalLogic.ClickReceivedFromMap(KeyCode.Mouse0);
            }
            else if (editorLogic != null)
            {
                editorLogic.ClickReceivedFromMap(KeyCode.Mouse0);
            }
        }
        else if (Input.GetMouseButtonDown(KeyConstants.RightClick))
        {
            if (globalLogic != null)
            {
                globalLogic.ClickReceivedFromMap(KeyCode.Mouse1);
            }
        }
    }

    public void CleanMap()
    {
        CleanTransform(citiesHolder);
        CleanTransform(troopsHolder);
    }

    private void CleanTransform(Transform cleanTransform)
    {
        List<GameObject> references;

        if (cleanTransform != null)
        {
            references = new List<GameObject>();

            foreach (Transform child in cleanTransform)
            {
                references.Add(child.gameObject);
            }

            foreach (GameObject gameObject in references)
            {
                Destroy(gameObject);
            }

            Debug.Log($"Cleaned transform: {cleanTransform.name}");
        }
        else
        {
            Debug.Log("Received transform empty, this is ok when the map display doesn't want to show cities and/or troops.");
        }
    }
}
