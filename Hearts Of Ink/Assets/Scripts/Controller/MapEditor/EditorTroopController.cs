using Assets.Scripts.Controller.InGame;
using UnityEngine;

public class EditorTroopController : MonoBehaviour, IObjectAnimator, IObjectSelectable
{
    public EditorPanelController panelController;
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

    public bool IsSelectable(int owner)
    {
        return ownerSocketId == owner;
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
        throw new System.NotImplementedException();
    }

    public void EndSelection()
    {
        //EndAnimation();
    }
}
