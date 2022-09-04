using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRotationAnimation : MonoBehaviour
{
    private float LastFrame { get; set; }
    private const float scalationFactor = 1.65f;
    private readonly float AnimationPart = 360f;
    public CircleCollider2D circleCollider;
    public const float AnimationSpeed = 0.5f;

    public void IterateAnimation()
    {
        float animationImageScale;
        this.gameObject.SetActive(true);

        animationImageScale = circleCollider.radius * scalationFactor;
        transform.localScale = new Vector3(animationImageScale, animationImageScale, animationImageScale);
        transform.RotateAround(transform.position, new Vector3(0, 0, 1), 1);
    }
}
