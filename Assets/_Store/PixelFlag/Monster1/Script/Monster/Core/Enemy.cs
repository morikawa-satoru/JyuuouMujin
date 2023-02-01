using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public abstract class Enemy : MassObject
    {
        public int life = 1;

        public Sprite damage;
        private Sprite empty;

        public Sprite[] sprites;

        private int damageWait = 50;
        private Vector2 damageForce = new Vector2(-3, 3);
        private bool damageLock = false;

        protected SpriteRenderer render;
        private HitObject hitObject;

        protected int direction = -1;

        public override void Initialize()
        {
            base.Initialize();
            render = transform.Find("BaseSprite").GetComponent<SpriteRenderer>();
            hitObject = transform.Find("Hit").GetComponent<HitObject>();
            hitObject.OnEnter += OnEnter;
        }

        private void OnEnter(Collider2D collider)
        {
            if (collider.tag == "PlayerFire")
            {
                Damage(collider.transform.position, 1);
            }
        }

        protected override void AfterUpdate()
        {
            base.AfterUpdate();
            render.flipX = direction > 0 ? true : false;
        }

        protected bool DamageAnimation(int count)
        {
            Blink(count);

            if (damageWait < count)
            {
                HoldX();

                if (life <= 0)
                {
                    Dead();
                    return false;
                }
                damageLock = false;
                return true;
            }
            return false;
        }

        public virtual void Damage(Vector3 pos, int damage)
        {
            if (damageLock == true) return;

            damageLock = true;
            life -= damage;

            Vector2 force = damageForce;
            force.x *= position.x < pos.x ? 1 : -1;
            ReplaceForce(force);
        }

        protected virtual void Dead()
        {
            Destroy(gameObject);
        }

        protected void Shake(int count, int size)
        {
            int xx = count % (size * 2)-size;
            render.transform.localPosition = new Vector3(xx, 0, 0);
        }

        protected void PositionReset()
        {
            render.transform.localPosition = new Vector3(0, 0, 0);
        }

        protected void Blink(int count)
        {
            render.sprite = count % 4 < 1 ? damage : empty;
        }

        protected void LookTarget(MassObject target)
        {
            if (x < target.x)
                direction = 1;
            else
                direction = -1;
        }
    }
}