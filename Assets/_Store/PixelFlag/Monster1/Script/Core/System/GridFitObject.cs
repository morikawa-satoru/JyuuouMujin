using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

namespace pixelflag.monster1
{
    [ExecuteAlways]
    public class GridFitObject : MonoBehaviour
    {
        public bool fitToGrid = true;
        public int grid = 16;

        void Update()
        {
            if (fitToGrid)
                GridFitting(grid);
            else
                GridFitting(1);
        }

        private void GridFitting(float grid)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Round(pos.x / grid) * grid;
            pos.y = Mathf.Round(pos.y / grid) * grid;
            pos.z = Mathf.Round(pos.z / grid) * grid;
            transform.position = pos;
        }
    }
}
#endif