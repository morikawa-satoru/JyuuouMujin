using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]

    public abstract class MassObject : PixelObject
    {
        private ContactFilter2D filter2D = default;
        private int touchSeachAngle = 10;

        private Rigidbody2D rigidbody;
        private Vector3 prevVelocity;

        private float initGravity = 0;

        public bool isStop { get { return (rigidbody.velocity.x == 0 && rigidbody.velocity.y == 0); } }

        public virtual void Initialize()
        {
            filter2D.useNormalAngle = true;
            filter2D.minNormalAngle = 70;
            filter2D.maxNormalAngle = 100;

            rigidbody = GetComponent<Rigidbody2D>();

            prevVelocity = rigidbody.velocity;
            initGravity = rigidbody.gravityScale;
        }

        protected virtual void AfterUpdate()
        {
            prevVelocity = rigidbody.velocity;
        }

        public virtual void FlipSpeedX()
        {
            Vector3 vel = prevVelocity;
            vel.x = -prevVelocity.x;
            rigidbody.velocity = vel;
        }

        public virtual void FlipSpeedY()
        {
            Vector3 vel = prevVelocity;
            vel.y = -prevVelocity.y;
            rigidbody.velocity = vel;
        }

        public virtual void Hold()
        {
            rigidbody.velocity = new Vector3(0, 0, 0);
        }

        public virtual void HoldX()
        {
            Vector3 vel = rigidbody.velocity;
            vel.x = 0;
            rigidbody.velocity = vel;
        }

        public void AddForce(Vector2 force)
        {
            Vector3 vel = rigidbody.velocity;
            vel.x = vel.x + force.x;
            vel.y = vel.y + force.y;
            rigidbody.velocity = vel;
        }

        public void ReplaceForce(Vector2 force)
        {
            Vector3 vel = rigidbody.velocity;
            vel.x = force.x;
            vel.y = force.y;
            rigidbody.velocity = vel;
        }

        public bool isCeilTouch
        {
            get
            {
                filter2D.minNormalAngle = 270 - touchSeachAngle;
                filter2D.maxNormalAngle = 270 + touchSeachAngle;
                return rigidbody.IsTouching(filter2D);
            }
        }
        public bool isLeftTouch
        {
            get
            {
                filter2D.minNormalAngle = 0 - touchSeachAngle;
                filter2D.maxNormalAngle = 0 + touchSeachAngle;
                return rigidbody.IsTouching(filter2D);
            }
        }
        public bool isGroundTouch
        {
            get
            {
                filter2D.minNormalAngle = 90 - touchSeachAngle;
                filter2D.maxNormalAngle = 90 + touchSeachAngle;
                return rigidbody.IsTouching(filter2D);
            }
        }

        public bool isRightTouch
        {
            get
            {
                filter2D.minNormalAngle = 180 - touchSeachAngle;
                filter2D.maxNormalAngle = 180 + touchSeachAngle;
                return rigidbody.IsTouching(filter2D);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public Vector2 GetVelocity2D()
        {
            return new Vector2(rigidbody.velocity.x, rigidbody.velocity.y);
        }

        public Vector2 GetPrevVelocity2D()
        {
            return prevVelocity;
        }

        public void DisableGravity()
        {
            rigidbody.gravityScale = 0;
        }

        public virtual void ResetGravity()
        {
            rigidbody.gravityScale = initGravity;
        }

        public virtual void ReverseGravity()
        {
            rigidbody.gravityScale = -initGravity;
        }
    }
}