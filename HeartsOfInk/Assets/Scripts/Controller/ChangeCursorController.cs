using UnityEngine;

public class ChangeCursorController : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    [SerializeField] Texture2D cursorTextureDefault;
    [SerializeField] Vector2 cursorHotspot;
    public TroopController troopController;
    private void OnMouseEnter()
    {
        if (!troopController.IsAllyTroop() )
        {
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(cursorTextureDefault, cursorHotspot, CursorMode.Auto);
    }
}
