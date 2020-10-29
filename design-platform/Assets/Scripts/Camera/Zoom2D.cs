using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Core {
    public class Zoom2D : MonoBehaviour {
        float scroll;
        float size;
        public float speed = 5f;
        public float minSize = 5f;
        public float maxSize = 150f;
        void Update() {
            scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0) {
                size = GetComponent<Camera>().orthographicSize;

                if (scroll > 0 && size - speed >= minSize) {
                    GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - speed;
                }

                if (scroll < 0 && size + speed <= maxSize) {
                    GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + speed;
                }
            }
        }
    }
}