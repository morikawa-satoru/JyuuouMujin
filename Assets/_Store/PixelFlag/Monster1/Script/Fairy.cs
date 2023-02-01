using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Fairy : MassObject
    {
        public HitObject hitObject;
        public Tinkle tinkle;
        public Sprite[] sprites;

        private CycleAnimation idol1;
        private CycleAnimation idol2;

        private int fireTouchingCount = 0;
        private int count = 0;
        private SpriteRenderer render;

        public float speed = 10;

        void Start()
        {
            Initialize();

            render = GetComponent<SpriteRenderer>();

            idol1 = new CycleAnimation(new Sprite[] { sprites[0], sprites[1] }, 4);
            idol2 = new CycleAnimation(new Sprite[] { sprites[2], sprites[3] }, 4);

            hitObject.OnEnter += OnEnter;
        }

        private void OnEnter(Collider2D collider)
        {
            if (collider.tag == "Fire")
            {
                fireTouchingCount = 10;
            }

            if (collider.tag == "Monster")
            {
                fireTouchingCount = 10;
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 30), "move : W.A.S.D key");
            GUI.Label(new Rect(10, 40, 200, 30), "shot : space key");
        }

        void FixedUpdate()
        {
            if(0 < fireTouchingCount)
                render.sprite = idol2.UpdateAnim(count);
            else
                render.sprite = idol1.UpdateAnim(count);

            if (Input.GetKey(KeyCode.W))
            {
                AddForce(new Vector2(0, speed));
            }
            if (Input.GetKey(KeyCode.A))
            {
                AddForce(new Vector2(-speed, 0));
            }
            if (Input.GetKey(KeyCode.S))
            {
                AddForce(new Vector2(0, -speed));
            }
            if (Input.GetKey(KeyCode.D))
            {
                AddForce(new Vector2(speed, 0));
            }

            count++;
            fireTouchingCount--;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shot();
            }
        }

        private void Shot()
        {
            Tinkle tin = Instantiate(tinkle);
            tin.Initialize();
            tin.position = position;
            tin.AddForce(GetVelocity2D()*2);
        }
    }
}
