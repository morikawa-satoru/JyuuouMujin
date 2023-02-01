using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class EnemyFire : MassObject
    {
        private Vector3 initPosition;

        public override void Initialize()
        {
            base.Initialize();
            initPosition = transform.localPosition;
        }

        public void SetFlipX(bool flip)
        {
            transform.Find("BaseSprite").GetComponent<SpriteRenderer>().flipX = flip;

            Vector3 pos = transform.localPosition;
            pos.x = flip ? -initPosition.x : initPosition.x;
            transform.localPosition = pos;
        }
    }
}