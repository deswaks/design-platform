using gbXMLSerializer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Core {
    public class PlayerMovement : MonoBehaviour {

        public CharacterController controller;

        public float speed = 6f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;

        public Transform groundChech;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;
        public LayerMask roomMask;
        public LayerMask wallMask;

        Vector3 velocity;
        bool isGrounded;
        bool isRoomed;
        bool isWalled;

        void Start() {
            Physics.IgnoreLayerCollision(12, 15);
            Physics.IgnoreLayerCollision(12, 8);
        }
        // Update is called once per frame
        void Update() {
            //Physics.IgnoreLayerCollision(controller.gameObject.layer,);

            isGrounded = Physics.CheckSphere(groundChech.position, groundDistance, groundMask);
            isRoomed = Physics.CheckSphere(groundChech.position, groundDistance, roomMask);
            isWalled = Physics.CheckSphere(groundChech.position, groundDistance, wallMask);

            if (isGrounded && velocity.y < 0 || isRoomed && velocity.y < 0 || isWalled && velocity.y < 0) {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            move.y = 0;

            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded || Input.GetButtonDown("Jump") && isRoomed || Input.GetButtonDown("Jump") && isWalled) {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }
    }
}