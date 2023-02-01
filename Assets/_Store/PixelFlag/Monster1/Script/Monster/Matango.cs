using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Matango : Enemy
    {
        [SerializeField]
        private int idleWait = 60;
        [SerializeField]
        private int spawnWait = 240;
        [SerializeField]
        private int chargeWait = 120;
        [SerializeField]
        private int spawnCount = 60;

        [SerializeField]
        private int spawnNum = 4;
        [SerializeField]
        private Vector2 spawnPower = new Vector2(60,100);
        [SerializeField]
        private float walkSpeed = 40;

        [SerializeField]
        private GameObject sporePrefab;

        private int tempDirection;
        private CycleAnimation walkAnim;

        private State state;
        private State prevState;
        private enum State
        {
            Idle,
            Walk,
            Charge,
            Spawn,
            Damage,
        }

        private int count = 0;
        private int touchwait = 0;

        public void Start()
        {
            base.Initialize();

            count = 0;
            state = State.Idle;

            walkAnim = new CycleAnimation(new Sprite[] { sprites[0], sprites[1] , sprites[2] , sprites[3]},8);
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

                    if (idleWait < count)
                        state = State.Walk;

                    break;
                case State.Walk:
                    ReplaceForce(new Vector2(walkSpeed * direction, 0));

                    render.sprite = walkAnim.UpdateAnim(count);

                    if ((isLeftTouch || isRightTouch) && touchwait < 0)
                    {
                        touchwait = 10;
                        FlipSpeedX();
                        direction = -direction;
                    }

                    if (spawnWait < count)
                    {
                        state = State.Charge;
                        direction = -direction;
                    }
                    touchwait--;
                    break;
                case State.Charge:
                    if (count == 1)
                        render.sprite = sprites[4];

                    Shake(count, 2);

                    if (count == chargeWait)
                    {
                        PositionReset();
                        state = State.Spawn;
                    }
                    break;
                case State.Spawn:
                    if (count == 1)
                    {
                        SpawnSpore();
                        tempDirection = direction;
                        render.sprite = sprites[5];
                    }

                    if(count% 4 == 0)
                        direction *= -1;

                    if (count == spawnCount)
                    {
                        PositionReset();
                        direction = tempDirection;
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

        private void SpawnSpore()
        {
            float powerX = spawnPower.x / spawnNum;
            for (int i = 0; i < spawnNum; i++)
            {
                Spore spore = Instantiate(sporePrefab).GetComponent<Spore>();
                spore.Initialize();
                spore.position = position;
                spore.y += 16;
                spore.AddForce(new Vector2(powerX*i- spawnPower.x/2 + powerX/2, spawnPower.y));
            }
        }

        public override void Damage(Vector3 pos, int damage)
        {
            base.Damage(pos, damage);

            state = State.Damage;
            count = 0;
        }
    }
}