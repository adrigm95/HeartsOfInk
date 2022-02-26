using Assets.Scripts.DataAccess;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private GlobalLogicController globalLogic;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        globalLogic = FindObjectOfType<GlobalLogicController>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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
