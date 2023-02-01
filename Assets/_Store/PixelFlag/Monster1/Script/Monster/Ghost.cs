using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Ghost : Enemy
    {
        [SerializeField]
        private MassObject targetObject;
        [SerializeField]
        private Vector2 floatingSpeed;
        [SerializeField]
        private Vector2 floatingScale;

        private Vector2 initPosition;
        private Vector2 floating;
        private CycleAnimation flyAnim;

        private State state;
        private State prevState;
        private enum State
        {
            Idle,
            Damage,
        }

        private int count = 0;

        private void Start()
        {
            base.Initialize();

            count = 0;
            state = prevState = State.Idle;

            initPosition = position;
            flyAnim = new CycleAnimation(new Sprite[] { sprites[0], sprites[1], sprites[2], sprites[1] }, 8);
        }

        private void FixedUpdate()
        {
            if (state != prevState) count = 0;
            prevState = state;

            switch (state)
            {
                case State.Idle:
                    render.sprite = flyAnim.UpdateAnim(count);

                    x = initPosition.x + Mathf.Sin(floating.x)* floatingScale.x;
                    y = initPosition.y + Mathf.Cos(floating.y)* floatingScale.y;
                    floating += floatingSpeed;

                    LookTarget(targetObject);
                    break;

                case State.Damage:
                    if (DamageAnimation(count))
                        state = State.Idle;
                    Hold();
                    break;
            }

            AfterUpdate();
            count++;
        }

        public override void Damage(Vector3 pos, int damage)
        {
            base.Damage(pos, damage);

            state = State.Damage;
            Hold();
        }
    }
}