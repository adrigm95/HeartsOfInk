using Assets.Scripts.DataAccess;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private static MapController _instance;
    private GlobalLogicController globalLogic;
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
        }
    }

    public void UpdateMap(string spritePath)
    {
        CleanMap();
        spriteRenderer.sprite = MapDAC.LoadMapSprite(spritePath);
    }

    void OnMouseOver()
    {
        const int LeftClick = 0;
        const int RightClick = 1;

        if (Input.GetMouseButtonDown(LeftClick))
        {
            // TODO: Multiselect
        }
        else if (Input.GetMouseButtonDown(RightClick))
        {
            globalLogic.ClickReceivedFromMap();
        }
    }

    public void CleanMap()
    {
        CleanTransform(citiesHolder);
        CleanTransform(troopsHolder);
    }

    private void CleanTransform(Transform cleanTransform)
    {
        List<GameObject> references = new List<GameObject>();

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
}
