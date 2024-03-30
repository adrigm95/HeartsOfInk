using Rawgen.Math.Logic.Utils;
using UnityEngine;

/// <summary>
/// Partiendo de la base de que el origen es [0,0] permite animar un contenedor del canvas hacia un punto y volver.
/// </summary>
public class UIAnimator : MonoBehaviour
{
    enum AnimationAction { NONE, TO_TARGET, TO_ORIGIN}

    private const float ArrivalDistance = 0.01f;
    public Vector2 origin;
    private AnimationAction currentAnimation;
    public Vector2 currentPosition;
    public Vector2 target;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        currentAnimation = AnimationAction.NONE;
        origin = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAnimation != AnimationAction.NONE)
        {
            MoveContainer();
        }
    }

    public void MoveToOrigin()
    {
        currentAnimation = AnimationAction.TO_ORIGIN;
    }

    public void MoveToTarget()
    {
        currentAnimation = AnimationAction.TO_TARGET;
    }

    private void MoveContainer()
    {
        Vector2 direction;
        Vector2 movement;
        float newX;
        float newY;
        float targetX;
        float targeyY;

        switch (currentAnimation)
        {
            case AnimationAction.TO_ORIGIN:
                direction = origin - currentPosition;
                break;
            case AnimationAction.TO_TARGET:
                direction = target - currentPosition;
                break;
            default:
                throw new System.Exception("Animation type unknown");
        }

        movement = direction * speed;
        Camera.main.transform.Translate(movement.x, movement.y, 0);
        currentPosition = Camera.main.transform.position;

        newX = Camera.main.transform.position.x;
        newY = Camera.main.transform.position.y;
        targetX = currentAnimation == AnimationAction.TO_ORIGIN ? origin.x : target.x;
        targeyY = currentAnimation == AnimationAction.TO_ORIGIN ? origin.y : target.y;

        if (ArrivalDistance > MathUtils.ExperimentalDistance(newX, newY, targetX, targeyY))
        {
            currentAnimation = AnimationAction.NONE;
        }
    }
}
