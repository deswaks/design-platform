using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController controller;

    public float speed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundChech;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask roomMask;

    Vector3 velocity;
    bool isGrounded;
    bool isRoomed;

    // Update is called once per frame
    void Update() {
        isGrounded = Physics.CheckSphere(groundChech.position, groundDistance, groundMask);
        isRoomed = Physics.CheckSphere(groundChech.position, groundDistance, roomMask);

        if (isGrounded && velocity.y < 0 || isRoomed && velocity.y < 0) {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        move.y = 0;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded || Input.GetButtonDown("Jump") && isRoomed) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
