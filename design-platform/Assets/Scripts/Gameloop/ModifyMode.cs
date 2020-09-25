using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using UnityEngine.EventSystems;
using System.Drawing;
using UnityEditor;

public class ModifyMode : Mode {

    private static ModifyMode instance;
    public Material defaultRoomMaterial;
    public Material selectedRoomMaterial;

    // Set at runtime
    public Room selectedRoom;

    public enum ModifyModeTypes {
        None,
        Move,
        Rotate,
        Edit,
        Delete
    }
    private ModifyModeTypes currentModifyModeType = ModifyModeTypes.None;

    public static ModifyMode Instance {
        // Use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new ModifyMode()); }
    }

    public void SetModifyMode(ModifyModeTypes currentMode) {
        currentModifyModeType = currentMode;

        selectedRoom.SetRoomState(Room.RoomStates.Stationary);
        selectedRoom.SetIsInMoveMode(false);

        switch (currentMode) {
            case ModifyModeTypes.Move:
                selectedRoom.SetIsInMoveMode(true);
                break;

            case ModifyModeTypes.Rotate:
                if (selectedRoom != null) {
                    selectedRoom.Rotate();
                }
                break;

            case ModifyModeTypes.Edit:
                if (selectedRoom != null) {
                    selectedRoom.SetEditHandles();
                }


                break;

            case ModifyModeTypes.Delete:
                if (selectedRoom != null) {
                    selectedRoom.Delete();
                }
                break;
        }
    }

    public override void Tick() {

        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject() == false) {
                selectClickedRoom();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) {
            deselect();
        }
    }

    public override void OnModeResume() {
        //camera = Camera.current;
    }

    public override void OnModePause() {
        deselect();
    }
    private void selectClickedRoom() {
        if (GetClickedRoom() != selectedRoom) // Ensures that an already selected room is not deselected and then selected again
        {
            deselect();
            selectedRoom = GetClickedRoom();
            if (selectedRoom != null) {
                selectedRoom.SetIsHighlighted(true);
            }
        }
    }
    private Room GetClickedRoom() {
        Room clickedRoom = null;
        UnityEngine.RaycastHit hitInfo;
        Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray: ray, hitInfo: out hitInfo)) {
            // if the hit game object is a room (ie. it is on layer 8)
            if (hitInfo.collider.gameObject.layer == 8) {
                clickedRoom = hitInfo.collider.gameObject.GetComponent<Room>();
            }
        }
        return clickedRoom;
    }
    private void deselect() {
        if (selectedRoom != null) {
            selectedRoom.SetIsHighlighted(false);
            selectedRoom.SetIsInMoveMode(false);
        }
        selectedRoom = null;
    }

    internal void Move() {
        selectedRoom.SetIsInMoveMode(true);
    }




}
