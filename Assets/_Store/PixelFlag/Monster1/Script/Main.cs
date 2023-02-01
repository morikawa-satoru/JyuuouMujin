using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class Main : MonoBehaviour
    {
        public GameObject cameraTarget;
        public Camera mainCamera;

        public GameObject topLeft;
        public GameObject bottomRight;

        private int top;
        private int left;
        private int bottom;
        private int right;

        private void Start()
        {
            float screenScale = Screen.height / mainCamera.orthographicSize / 2;

            int sw = (int)(Screen.width / screenScale);
            int sh = (int)(Screen.height / screenScale);

            top = (int)topLeft.transform.localPosition.y - sh / 2;
            left = (int)topLeft.transform.localPosition.x + sw / 2;
            bottom = (int)bottomRight.transform.localPosition.y + sh / 2;
            right = (int)bottomRight.transform.localPosition.x - sw / 2;
        }

        private void FixedUpdate()
        {
            MoveCamera();
        }

        public void MoveCamera()
        {
            Vector3 tPos = cameraTarget.transform.position;
            Vector3 cPos = mainCamera.transform.position;

            Vector3 newPos = cPos - ((cPos - tPos) / 10);

            if (top < newPos.y) newPos.y = top;
            if (left > newPos.x) newPos.x = left;
            if (bottom > newPos.y) newPos.y = bottom;
            if (right < newPos.x) newPos.x = right;

            mainCamera.transform.position = new Vector3(newPos.x, newPos.y, -100);
        }


    }
}
