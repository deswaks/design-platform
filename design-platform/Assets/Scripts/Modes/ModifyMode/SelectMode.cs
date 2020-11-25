using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMode : Mode {

    private static SelectMode instance;
    private Mode currentMode;
    public Room selection;

    public static SelectMode Instance {
        // Use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new SelectMode()); }
    }

    /// <summary>
    /// Gameloop tick is run every frame the mode is active
    /// </summary>
    public override void Tick() {

        if (Input.GetMouseButtonDown(0)) {
            Select();
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2)) {
            Deselect();
            SetMode(null);

        }

        if (currentMode != null) {
            currentMode.Tick();
        }
    }
    public override void OnModeResume() {
        SetMode(null);
    }
    public override void OnModePause() {
        Deselect();
        SetMode(null);
    }

    public void SetMode(Mode mode) {
        if (currentMode != null) {
            currentMode.OnModePause();
        }
        currentMode = mode;
        if (currentMode != null) {
            currentMode.OnModeResume();
        }
    }

<<<<<<< Updated upstream
    private void Select() {
        // Abort if UI object is under cursor
        bool overGO = EventSystem.current.IsPointerOverGameObject();
        if (overGO) {
            return;
        }
=======
            // Check if any number key was pressed
            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                if (selection != null) selection.Type = RoomType.DEFAULT;
            }
            for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha6; i++) {
                if (Input.GetKeyDown((KeyCode)i)) {
                    if (selection != null) {
                        selection.Type = ((RoomType) i - (int)KeyCode.Alpha1 + 10);
                    }
                }
            }
>>>>>>> Stashed changes

        // Set new room selection
        Room clickedRoom = GetClickedRoom();
        if (clickedRoom != null) {
            if (clickedRoom != selection) {
                Deselect();
                selection = clickedRoom;
                selection.SetIsHighlighted(true);
            }
        }
        else {
            Deselect();
            SetMode(null);
        }
    }

    private void Deselect() {

        if (selection != null) {
            selection.SetIsHighlighted(false);
            selection.SetIsInMoveMode(false);
            selection.RemoveEditHandles();
        }
        selection = null;
    }

    private Room GetClickedRoom() {
        Room clickedRoom = null;
        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hitInfo)) {
            // if the hit game object is a room (ie. it is on layer 8)
            if (hitInfo.collider.gameObject.layer == 8) {
                clickedRoom = hitInfo.collider.gameObject.GetComponent<Room>();
            }
            else {
                clickedRoom = hitInfo.collider.gameObject.GetComponentInParent<Room>();
            }
        }
        return clickedRoom;
    }

}
