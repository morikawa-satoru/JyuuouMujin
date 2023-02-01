using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Gorira : Enemy
    {
        [SerializeField]
        private MassObject targetObject;
        [SerializeField]
        private int jumpWaitCount = 3;
        [SerializeField]
        private int idleWait = 20;
        [SerializeField]
        private float jumpPower = 50;
        [SerializeField]
        private float throwPower = 50;
        [SerializeField]
        private GameObject rockPrefab;
        [SerializeField]
        private Vector2 rockOffset = new Vector2(-2,40);

        private int jumpCount;
        private BigRock rock;

        private State state;
        private State prevState;
        private enum State
        {
            Idle,
            Ready,
            Jump,
            Randing,
            Throw1,
            Throw2,
            Damage,
        }

        private int count = 0;

        private void Start()
        {
            base.Initialize();

            count = 0;
            state = prevState = State.Idle;
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

                    LookTarget(targetObject);

                    if (count == idleWait)
                    {
                        if (jumpWaitCount <= jumpCount)
                        {
                            jumpCount = 0;
                            state = State.Throw1;
                        }
                        else
                        {
                            jumpCount++;
                            state = State.Ready;
                        }
                    }
                    break;

                case State.Ready:
                    if (count == 0)
                        render.sprite = sprites[1];

                    if (count == 20)
                    {
                        render.sprite = sprites[2];
                        ReplaceForce(new Vector2(0, jumpPower));
                        state = State.Jump;
                    }
                    break;

                case State.Jump:
                    if (isGroundTouch && 10 < count)
                    {
                        state = State.Randing;
                    }
                    break;

                case State.Randing:
                    if (count == 0)
                        render.sprite = sprites[1];

                    if (count == 10)
                        state = State.Idle;
                    break;

                case State.Throw1:
                    if (count == 0)
                        render.sprite = sprites[1];

                    if (count == 10)
                    {
                        render.sprite = sprites[3];
                        rock = Instantiate(rockPrefab).GetComponent<BigRock>();
                        rock.Initialize();
                        rock.position = position;
                        rock.y += rockOffset.y;
                        rock.x += rockOffset.x * direction;
                        rock.DisableGravity();
                    }

                    if (count == 30)
                        state = State.Throw2;
                    break;

                case State.Throw2:
                    if (count == 0)
                    {
                        render.sprite = sprites[4];
                        if (rock != null)
                        {
                            rock.ResetGravity();
                            rock.AddForce(new Vector2(direction * throwPower, throwPower));
                        }
                    }

                    if (count  == 10)
                        render.sprite = sprites[1];

                    if (count == 20)
                        state = State.Idle;
                    break;

                case State.Damage:
                    if (rock != null)
                        rock.ResetGravity();

                    jumpCount = 0;

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
            HoldX();
        }
    }
}