using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Crawler : Enemy
    {
        [SerializeField]
        private Vector2 jumpPower = new Vector2(5, 5);
        [SerializeField]
        private int jumpWait = 120;

        private CycleAnimation spinAnim;

        private State state;
        private State prevState;
        private enum State
        {
            Idle,
            Ready,
            Jump,
            Damage,
        }

        private int count = 0;
        private int touchwait = 0;

        private void Start()
        {
            base.Initialize();

            count = 0;
            state = prevState = State.Idle;

            spinAnim = new CycleAnimation(new Sprite[] { sprites[2], sprites[3] }, 6);
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
                        state = State.Ready;
                    break;
                case State.Ready:
                    if (count == 0)
                        render.sprite = sprites[1];

                    if (count == 10)
                    {
                        render.sprite = sprites[2];
                        ReplaceForce(new Vector2(jumpPower.x * direction, jumpPower.y));
                        state = State.Jump;
                    }
                    break;
                case State.Jump:
                    render.sprite = spinAnim.UpdateAnim(count);

                    if ((isLeftTouch || isRightTouch)&& touchwait < 0)
                    {
                        touchwait = 10;
                        FlipSpeedX();
                        direction *= -1;
                    }

                    if (isGroundTouch && 10 < count)
                    {
                        state = State.Idle;
                        HoldX();
                    }
                    touchwait--;
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