using Assets.Scripts.Controller.InGame;
using Assets.Scripts.Logic;
using UnityEngine;

public class EditorCityController : MonoBehaviour, IObjectAnimator
{
    public CircleRotationAnimation animator;
    public int ownerSocketId;
    public bool isCapital;

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    /// <summary>
    /// Cambia el tipo de ciudad que es (normal/capital)
    /// </summary>
    public void ChangeType()
    {

    }

    /// <summary>
    /// Cambia el owner de la ciudad.
    /// </summary>
    public void ChangeOwner()
    {

    }

    public void Animate()
    {
        animator.IterateAnimation();
    }

    public void EndAnimation()
    {
        animator.gameObject.SetActive(false);
    }
}
