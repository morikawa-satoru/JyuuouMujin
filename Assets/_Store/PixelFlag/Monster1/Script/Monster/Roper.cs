using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Roper : Enemy
    {
        [SerializeField]
        private MassObject targetObject;
        [SerializeField]
        private int idleWait = 120;
        [SerializeField]
        private int chargeWait = 60;
        [SerializeField]
        private int fireWait = 60;

        [SerializeField]
        private EnemyFire rope;

        private CycleAnimation idleAnim;

        private State state;
        private State prevState;
        private enum State
        {
            Idle,
            Charge,
            Fire,
            Damage,
        }

        private int count = 0;

        private void Start()
        {
            base.Initialize();

            count = 0;
            state = prevState = State.Idle;
            idleAnim = new CycleAnimation(new Sprite[] { sprites[0], sprites[1], sprites[2], sprites[1] }, 10);

            rope.Initialize();
            rope.Hide();
        }

        private void FixedUpdate()
        {
            if (state != prevState) count = 0;
            prevState = state;

            switch (state)
            {
                case State.Idle:
                    render.sprite = idleAnim.UpdateAnim(count);

                    if (count == idleWait)
                       state = State.Charge;

                    LookTarget(targetObject);
                    break;

                case State.Charge:
                    if (count == 0)
                        render.sprite = sprites[3];

                    Shake(count, 2);

                    if (count == chargeWait)
                    {
                        PositionReset();
                        state = State.Fire;
                    }
                    break;

                case State.Fire:
                    if (count == 0)
                    {
                        render.sprite = sprites[4];
                        rope.SetFlipX(direction == -1 ? false: true);
                        rope.Show();
                    }

                    if (count == fireWait)
                    {
                        state = State.Idle;
                        rope.Hide();
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
            rope.Hide();
        }

        protected override void Dead()
        {
            Destroy(gameObject);
            Destroy(rope.gameObject);
        }
    }
}