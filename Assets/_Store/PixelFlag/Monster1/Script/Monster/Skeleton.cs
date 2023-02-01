using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Skeleton : Enemy
    {
        [SerializeField]
        private HitObject seachArea;
        [SerializeField]
        private MassObject targetObject;
        [SerializeField]
        private int idleWait = 120;
        [SerializeField]
        private int walkWait = 60;
        [SerializeField]
        private int chargeWait = 60;
        [SerializeField]
        private int fireWait = 60;

        [SerializeField]
        private float attackSeachRange = 64;
        [SerializeField]
        private float walkSpeed = 30f;

        [SerializeField]
        private EnemyFire sword;

        private CycleAnimation walkAnim;

        private State state;
        private State prevState;
        private enum State
        {
            Idle,
            Walk,
            Charge,
            Attack,
            Damage,
        }

        private int count = 0;
        private bool targetFind;

        private void Start()
        {
            base.Initialize();

            sword.Initialize();
            sword.Hide();

            count = 0;
            state = prevState = State.Idle;

            walkAnim = new CycleAnimation(new Sprite[] { sprites[0], sprites[1] }, 10);

            seachArea.OnEnter += OnEnter;
            seachArea.OnExit += OnExit;
        }

        private void OnEnter(Collider2D collider)
        {
            if(collider.tag == "Player")
                targetFind = true;
        }

        private void OnExit(Collider2D collider)
        {
            if (collider.tag == "Player")
                targetFind = false;
        }

        private void FixedUpdate()
        {
            if (state != prevState) count = 0;
            prevState = state;

            switch (state)
            {
                case State.Idle:
                    if (count == 0) render.sprite = sprites[0];

                    LookTarget(targetObject);

                    if (targetFind)
                        state = State.Charge;

                    if (count == idleWait)
                        state = State.Walk;
                    break;

                case State.Walk:
                    render.sprite = walkAnim.UpdateAnim(count);
                    ReplaceForce(new Vector2(walkSpeed * direction, 0));

                    if (count == walkWait)
                    {
                        HoldX();
                        state = State.Idle;
                    }

                    break;

                case State.Charge:
                    if (count == 0)
                        render.sprite = sprites[2];

                    Shake(count, 2);

                    if (count == chargeWait)
                    {
                        PositionReset();
                        state = State.Attack;
                    }
                    break;

                case State.Attack:
                    if (count == 0)
                    {
                        render.sprite = sprites[3];
                        sword.SetFlipX(direction == -1 ? false : true);
                        sword.Show();
                    }

                    if (count == fireWait)
                    {
                        state = State.Idle;
                        sword.Hide();
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
            sword.Hide();
        }

        protected override void Dead()
        {
            Destroy(gameObject);
            Destroy(sword.gameObject);
        }
    }
}