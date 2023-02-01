using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]

    public class HitObject : MonoBehaviour
    {
        private void Start()
        {
            Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0;

            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (OnEnter != null) OnEnter(collider);
        }

        public delegate void EnterDelegate(Collider2D collider);
        public EnterDelegate OnEnter;

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (OnExit != null) OnExit(collider);
        }

        public delegate void ExitDelegate(Collider2D collider);
        public ExitDelegate OnExit;
    }
}