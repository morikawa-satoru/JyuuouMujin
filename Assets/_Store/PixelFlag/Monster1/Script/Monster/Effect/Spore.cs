using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Spore : MassObject
    {
        public float underLimit = -300;

        private void Update()
        {
            if(transform.position.y < underLimit)
            {
                Destroy(gameObject);
            }
        }
    }
}