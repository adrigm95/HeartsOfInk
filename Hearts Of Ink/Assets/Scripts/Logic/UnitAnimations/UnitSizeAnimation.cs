using UnityEngine;

namespace Assets.Scripts.Logic
{
    public class UnitSizeAnimation : UnitAnimation
    {
        private const float maxTextSize = 1.20f;
        private const float minTextSize = 0.85f;

        private float textSize;
        private bool isInCrescendo;

        public UnitSizeAnimation(float gametime) : base(gametime)
        {
            textSize = 1;
            isInCrescendo = true;
        }

        public override float IterateAnimation(float gametime, TroopController animateTroop)
        {
            float sizeChange = (gametime - LastFrame) * AnimationSpeed;

            LastFrame = gametime;

            if (isInCrescendo)
            {
                textSize += sizeChange;
                
                if (textSize > maxTextSize)
                {
                    textSize = maxTextSize;
                    isInCrescendo = false;
                }
            }
            else
            {
                textSize -= sizeChange;

                if (textSize < minTextSize)
                {
                    textSize = minTextSize;
                    isInCrescendo = true;
                }
            }

            return textSize;
        }
    }
}
