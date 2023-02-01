using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class CycleAnimation
    {
        private Sprite[] sprites;
        private int step;
        private int index;

        public CycleAnimation(Sprite[] sprites, int step)
        {
            this.sprites = sprites;
            this.step = step;

            Reset();
        }

        public void Reset()
        {
            index = 0;
        }

        public Sprite UpdateAnim(int count)
        {
            if (count % step == 0)
            {
                index++;
                if (sprites.Length <= index) index = 0;
            }
            return sprites[index];
        }
    }
}