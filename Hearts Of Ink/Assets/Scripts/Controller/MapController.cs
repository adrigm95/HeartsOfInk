using Assets.Scripts.DataAccess;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void UpdateMap(string spritePath)
    {
        spriteRenderer.sprite = MapDAC.LoadMapSprite(spritePath);
    }
}
