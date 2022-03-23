using Assets.Scripts.DataAccess;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private static MapController _instance;
    private GlobalLogicController globalLogic;
    private SpriteRenderer spriteRenderer;
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
        spriteRenderer.sprite = MapDAC.LoadMapSprite(spritePath);
    }

    void OnMouseDown()
    {
        globalLogic.ClickReceivedFromMap();
    }
}
