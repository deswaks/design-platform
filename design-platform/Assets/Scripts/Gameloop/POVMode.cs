using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class POVMode : Mode {

    private static POVMode instance;

    public float mouseSensitivity = 50f;

    public static GameObject player;
    public Camera PlanCamera; 
    public Camera POVCamera; 

    bool pressed = false;
    float xRotation = 0f;

    public static POVMode Instance {
        // Use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new POVMode()); }
    }

    public override void Tick() {

        if (Input.GetKey(KeyCode.Escape)) {
            pressed = !pressed;
        }
        if (pressed) {
            Cursor.lockState = CursorLockMode.None;
        }


        if (!pressed) {
            Cursor.lockState = CursorLockMode.Locked;
            UpdatePOVCamera();
        }

    }


    public override void OnModeResume() {
        player = GameObject.Find("First person player");
        PlanCamera = GameObject.Find("Plan Camera").GetComponent<Camera>();
        POVCamera = player.GetComponentInChildren<Camera>(true);



        Debug.Log(player.ToString());
        POVCamera.gameObject.SetActive(true);
        PlanCamera.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }





    public override void OnModePause() {
        POVCamera.gameObject.SetActive(false);
        PlanCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

    }

    public void UpdatePOVCamera() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Look Up/Down 
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        POVCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Se til siden
        player.transform.Rotate(Vector3.up * mouseX);
    }

}
