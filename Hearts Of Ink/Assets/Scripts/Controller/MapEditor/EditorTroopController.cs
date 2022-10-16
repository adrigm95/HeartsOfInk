using Assets.Scripts.Controller.InGame;
using UnityEngine;

public class EditorTroopController : MonoBehaviour, IObjectAnimator, IObjectSelectable
{
    public MapEditorLogicController editorLogicController;
    public CircleRotationAnimation animator;
    public int ownerSocketId;

    // Start is called before the first frame update
    public void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    private void OnMouseDown()
    {
        editorLogicController.ClickReceivedFromTroop(this);
        Debug.Log("On left click: " + this);
    }

    public bool IsSelectable(int owner)
    {
        return true;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Animate()
    {
        animator.IterateAnimation();
    }

    public void EndAnimation()
    {
        Debug.Log("EndAnimation: " + this.name);
        animator.gameObject.SetActive(false);
    }

    public void EndSelection()
    {
        EndAnimation();
    }
}
