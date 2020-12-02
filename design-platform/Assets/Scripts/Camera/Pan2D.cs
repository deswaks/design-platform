using DesignPlatform.Geometry;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.UI {

    /// <summary>
    /// Contains functions to let the user pan the camera.
    /// </summary>
    public class Pan2D : MonoBehaviour {
        public float moveSpeed;

        /// <summary>
        /// Lets the user pan the camera around the plan view by holding their middle mouse button and dragging.
        /// </summary>
        void Update() {
            if (Input.GetMouseButton(2)) {
                if (EventSystem.current.IsPointerOverGameObject()) return; // Abort if UI object is under cursor
                transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * moveSpeed);
                transform.Translate(transform.up * -Input.GetAxis("Mouse Y") * moveSpeed, Space.World);
            }
        }

    }
}