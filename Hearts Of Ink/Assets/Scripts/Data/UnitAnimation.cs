namespace Assets.Scripts.Data
{
    public class UnitAnimation
    {
        private const float maxTextSize = 1.20f;
        private const float minTextSize = 0.85f;
        private const float animationSpeed = 0.5f;

        private float textSize;
        private bool isInCrescendo;
        private float lastFrame;

        public UnitAnimation(float gametime)
        {
            textSize = 1;
            isInCrescendo = true;
            this.lastFrame = gametime;
        }

        public float IterateAnimation(float gametime)
        {
            float sizeChange = (gametime - lastFrame) * animationSpeed;

            lastFrame = gametime;

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
