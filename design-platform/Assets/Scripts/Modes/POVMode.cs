﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class POVMode : Mode {

    private static POVMode instance;

    public float mouseSensitivity = 50f;

    public static GameObject player;
    public Camera PlanCamera;
    public Camera POVCamera;

    float xRotation = 0f;

    public enum ModeType {
        POV,
        MENU
    }
    private ModeType currentModeType = ModeType.POV;

    public static POVMode Instance {
        // Use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new POVMode()); }
    }

    public override void Tick() {

        switch (currentModeType) {
            case ModeType.POV:
                if (Input.GetKeyDown(KeyCode.Escape)) { SetModeType(ModeType.MENU); }
                break;

            case ModeType.MENU:
                if (Input.GetKeyDown(KeyCode.Escape)) { SetModeType(ModeType.POV); }
                break;
        }

        TickModeType();
    }
    public override void OnModeResume() {
        player = GameObject.Find("First person player");
        PlanCamera = GameObject.Find("Plan Camera").GetComponent<Camera>();
        POVCamera = player.GetComponentInChildren<Camera>(true);

        POVCamera.gameObject.SetActive(true);
        PlanCamera.gameObject.SetActive(false);

        currentModeType = ModeType.POV;
        OnModeTypeResume();
    }
    public override void OnModePause() {
        POVCamera.gameObject.SetActive(false);
        PlanCamera.gameObject.SetActive(true);
    }

    /// <summary>
    /// Set mode type
    /// </summary>
    /// <param name="modeType"></param>
    public void SetModeType(ModeType modeType) {
        if (modeType != currentModeType) {
            OnModeTypePause();
            currentModeType = modeType;
            OnModeTypeResume();
        }
    }
    public void TickModeType() {
        switch (currentModeType) {
            case ModeType.POV:
                UpdatePOVCamera();
                break;

            case ModeType.MENU:
                break;
        }
    }
    public void OnModeTypeResume() {
        switch (currentModeType) {
            case ModeType.POV:
                Cursor.lockState = CursorLockMode.Locked;
                break;

            case ModeType.MENU:
                break;
        }
    }
    public void OnModeTypePause() {
        switch (currentModeType) {
            case ModeType.POV:
                Cursor.lockState = CursorLockMode.None;
                break;

            case ModeType.MENU:
                break;
        }
    }


    /// <summary>
    /// 
    /// </summary>
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