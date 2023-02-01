using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Tinkle : MassObject
    {
        public int life = 120;
        private int count = 0;

        private void FixedUpdate()
        {
            count++;
            if (life < count)
            {
                 Destroy(gameObject);
            }
        }
    }
}