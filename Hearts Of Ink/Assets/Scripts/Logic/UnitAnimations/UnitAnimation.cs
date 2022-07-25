using UnityEngine;

namespace Assets.Scripts.Logic
{
    public abstract class UnitAnimation
    {
        [SerializeField]
        protected const float AnimationSpeed = 0.5f;
        protected float LastFrame { get; set; }

        public UnitAnimation(float gametime)
        {
            LastFrame = gametime;
        }

        public abstract float IterateAnimation(float gametime, TroopController animateTroop);
    }
}
