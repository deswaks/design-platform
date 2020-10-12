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
    //public Material defaultRoomMaterial;
    //public Material selectedRoomMaterial;
    public Room selectedRoom;

    public enum ModeType {
        None,
        Move,
        Rotate,
        Edit,
        Delete
    }
    private ModeType currentModeType = ModeType.None;

    public static ModifyMode Instance {
        // Use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new ModifyMode()); }
    }



    /// <summary>
    /// Gameloop tick is run every frame the mode is active
    /// </summary>
    public override void Tick() {

        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject() == false) {
                SelectClickedRoom();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2)) {
            Deselect();
            currentModeType = ModeType.None;
            
        }
    }
    public override void OnModeResume() {
        //camera = Camera.current;
    }
    public override void OnModePause() {
        if (selectedRoom != null) {
            selectedRoom.RemoveEditHandles();
        }
        currentModeType = ModeType.None;
        Deselect();
    }



    /// <summary>
    /// Set mode type
    /// </summary>
    /// <param name="modeType"></param>
    public void SetModeType(ModeType modeType) {
        
        // Change mode
        if (modeType != currentModeType) {
            OnModeTypePause();
            currentModeType = modeType;
            OnModeTypeResume();
        }

        // Run modetype action
        if (selectedRoom != null) {
            TickModeType();
        }
    }
    public void TickModeType() {
        switch (currentModeType) {
            case ModeType.Move:
                break;

            case ModeType.Rotate:
                    selectedRoom.Rotate();
                break;

            case ModeType.Edit:
                break;

            case ModeType.Delete:
                    selectedRoom.Delete();
                break;

            default:
                break;
        }
    }
    public void OnModeTypeResume() {
        //Debug.Log(currentModeType.ToString());

        switch (currentModeType) {
            case ModeType.Move:
                if (selectedRoom != null) {
                    selectedRoom.SetIsInMoveMode(true);
                }
                break;

            case ModeType.Rotate:
                break;

            case ModeType.Edit:
                if (selectedRoom != null) {
                    selectedRoom.SetEditHandles();
                }
                break;

            case ModeType.Delete:
                break;
            case ModeType.None:
                //selectedRoom.RemoveEditHandles();
                selectedRoom.SetIsHighlighted(false);
                selectedRoom.SetIsInMoveMode(false);
                selectedRoom.RemoveEditHandles();
                break;
            default:
                break;
        }
    }
    public void OnModeTypePause() {
        //Debug.Log(currentModeType.ToString());

        switch (currentModeType) {
            case ModeType.Move:
                if (selectedRoom != null) {
                    selectedRoom.SetRoomState(Room.RoomStates.Stationary);
                    selectedRoom.SetIsInMoveMode(false);
                    selectedRoom.SetIsHighlighted(true);
                }
                break;

            case ModeType.Rotate:
                break;

            case ModeType.Edit:
                if (selectedRoom != null) {
                    selectedRoom.RemoveEditHandles();
                    selectedRoom.ResetOrigin();
                }
                break;

            case ModeType.Delete:
                break;

            default:
                break;
        }
    }



    /// <summary>
    /// 
    /// </summary>
    private void SelectClickedRoom() {
        if (GetClickedRoom() != selectedRoom) // Ensures that an already selected room is not deselected and then selected again
        {
            Deselect();
            selectedRoom = GetClickedRoom();
            if (selectedRoom != null) {
                selectedRoom.SetIsHighlighted(true);
            }
        }
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
                //SetModeType(ModeType.None);
                clickedRoom = hitInfo.collider.gameObject.GetComponentInParent<Room>();
                //if (!clickedRoom) {
                //    SetModeType(ModeType.None);
                //}
            }
        }
        return clickedRoom;
    }

    private void Deselect() {
        //SetModeType(ModeType.None);
        if (selectedRoom != null) {
            //selectedRoom.RemoveEditHandles();
            selectedRoom.SetIsHighlighted(false);
            selectedRoom.SetIsInMoveMode(false);
            selectedRoom.RemoveEditHandles();
        }
        selectedRoom = null;
    }
}
