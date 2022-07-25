using System;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class UnitRotationAnimation : UnitAnimation
    {
        private const float scalationFactor = 1.65f;
        private readonly float AnimationPart;

        public UnitRotationAnimation(float gametime) :base(gametime)
        {
            AnimationPart = 360f;
        }

        public override float IterateAnimation(float gametime, TroopController animateTroop)
        {
            float animationImageScale = animateTroop.circleCollider.radius * scalationFactor;

            animateTroop.rotationImage.transform.localScale = new Vector3(animationImageScale, animationImageScale, animationImageScale);

            return AnimationSpeed;
        }
    }
}
