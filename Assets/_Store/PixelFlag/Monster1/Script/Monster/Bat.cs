using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Bat : Enemy
    {
        [SerializeField]
        private MassObject targetObject;
        [SerializeField]
        private Vector2 jumpPower = new Vector2(50, -50);
        [SerializeField]
        private int jumpWait = 120;

        private CycleAnimation flyAnim;

        private State state;
        private State prevState;
        private enum State
        {
            Idle,
            Glide,
            Damage,
        }

        private int count = 0;

        private void Start()
        {
            base.Initialize();

            count = 0;
            state = prevState = State.Idle;
            flyAnim = new CycleAnimation(new Sprite[] { sprites[0], sprites[1] }, 6);
        }

        private void FixedUpdate()
        {
            if (state != prevState) count = 0;
            prevState = state;

            switch (state)
            {
                case State.Idle:
                    if (count == 0)
                        render.sprite = sprites[2];

                    LookTarget(targetObject);

                    if (jumpWait < count)
                    {
                        Vector2 force = jumpPower;
                        force.x *= direction;
                        ReplaceForce(force);

                        state = State.Glide;
                    }
                    break;
                case State.Glide:
                    render.sprite = flyAnim.UpdateAnim(count);

                    if (isCeilTouch && 10 < count)
                    {
                        state = State.Idle;
                        HoldX();
                    }
                    break;
                case State.Damage:
                    if(DamageAnimation(count))
                    {
                        ResetGravity();
                        state = State.Glide;
                    }
                    break;
            }

            AfterUpdate();
            count++;
        }

        public override void Damage(Vector3 pos, int damage)
        {
            base.Damage(pos, damage);

            ReverseGravity();
            state = State.Damage;
            HoldX();
        }
    }
}