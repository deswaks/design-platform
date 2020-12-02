using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.UI {

    /// <summary>
    /// Contains functions to let the user zoom the camera.
    /// </summary>
    public class Zoom2D : MonoBehaviour {

        /// <summary>The speed of the zoom is defined as the addition/subtraction for each increment.</summary>
        public float speed = 5f;

        /// <summary>The current zoom level.</summary>
        float size;

        /// <summary>The minimum allowed zoom level.</summary>
        public float minSize = 5f;

        /// <summary>The maximum allowed zoom level.</summary>
        public float maxSize = 150f;

        /// <summary>
        /// Lets the user zoom the camera in the plan view by scrolling their mouse wheel.
        /// </summary>
        void Update() {
            if (EventSystem.current.IsPointerOverGameObject()) return; // Abort if UI object is under cursor

            float scroll = Input.GetAxis("Mouse ScrollWheel");

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