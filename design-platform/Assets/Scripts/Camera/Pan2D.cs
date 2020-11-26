using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.UI {
    public class Pan2D : MonoBehaviour {
        public float moveSpeed;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame 
        void Update() {
            if (Input.GetMouseButton(2)) {
                if (EventSystem.current.IsPointerOverGameObject()) return; // Abort if UI object is under cursor
                transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * moveSpeed);
                transform.Translate(transform.up * -Input.GetAxis("Mouse Y") * moveSpeed, Space.World);
            }
        }

    }
}