using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Slime : Enemy
    {
        [SerializeField]
        private Vector2 jumpPower = new Vector2(50,200);
        [SerializeField]
        private int jumpWait = 120;

        private State state;
        private State prevState;
        private int count = 0;
        private enum State
        {
            Idle,
            Ready,
            Jump,
            Randing,
            Damage 
        }

        private void Start()
        {
            Initialize();

            count = 0;
            state = State.Idle;
        }

        private void FixedUpdate()
        {
            if (state != prevState) count = 0;
            prevState = state;

            switch (state)
            {
                case State.Idle:
                    if (count == 0)
                        render.sprite = sprites[0];

                    if (jumpWait < count)
                    {
                        state = State.Ready;
                    }
                    break;
                case State.Ready:
                    if (count == 0)
                        render.sprite = sprites[1];

                    if (count == 10)
                    {
                        render.sprite = sprites[2];
                        ReplaceForce( new Vector2(jumpPower.x * direction, jumpPower.y));
                        state = State.Jump;
                    }
                    break;
                case State.Jump:
                    if (isGroundTouch && 0 < count)
                    {
                        state = State.Randing;
                        HoldX();
                    }
                    break;
                case State.Randing:
                    if (count == 0)
                        render.sprite = sprites[1];

                    if (count == 10)
                    {
                        direction *= -1;
                        state = State.Idle;
                    }
                    break;
                case State.Damage:
                    if (DamageAnimation(count))
                        state = State.Idle;
                    break;
            }

            AfterUpdate();
            count++;
        }

        public override void Damage(Vector3 pos, int damage)
        {
            base.Damage(pos, damage);
            state = State.Damage;
        }
    }
}