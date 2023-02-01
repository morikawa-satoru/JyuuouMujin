using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class SmallRock : MassObject
    {
        [SerializeField]
        private Sprite[] sprites;
        public float underLimit = -300;

        public override void Initialize()
        {
            base.Initialize();

            SpriteRenderer render = transform.Find("BaseSprite").GetComponent<SpriteRenderer>();
            render.sprite = sprites[Random.Range(0,sprites.Length-1)];
        }

        private void Update()
        {
            if (transform.position.y < underLimit)
            {
                Destroy(gameObject);
            }
        }
    }
}