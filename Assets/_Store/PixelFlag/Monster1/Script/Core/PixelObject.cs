using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public abstract class PixelObject : MonoBehaviour
    {
        public Vector3 position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public float x
        {
            get
            {
                return transform.position.x;
            }
            set
            {
                Vector3 pos = transform.position;
                pos.x = value;
                transform.position = pos;
            }
        }

        public float y
        {
            get
            {
                return transform.position.y;
            }
            set
            {
                Vector3 pos = transform.position;
                pos.y = value;
                transform.position = pos;
            }
        }
    }
}