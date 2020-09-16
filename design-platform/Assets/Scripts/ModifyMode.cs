using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

public class ModifyMode : Mode
{
    // References to other objects in scene
    public Camera cam;
    public RoomHandler roomHandler;
    public Material defaultRoomMaterial;
    public Material selectedRoomMaterial;

    // Set at runtime
    public GameObject selectedObject;

    public override void Tick()
    {
        if (Input.GetMouseButtonDown(0)) {
            deselect();
            selectedObject = GetClickedRoom();
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            deselect();
        }
    }

    public override void OnModeResume()
    {
        monoColorAllRooms();
        hightlightSelectedRoom();
    }

    public override void OnModePause()
    {
        monoColorAllRooms();
    }

    private GameObject GetClickedRoom() {
        GameObject clickedRoom = null;
        UnityEngine.RaycastHit hitInfo;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray: ray, hitInfo: out hitInfo)) {
            // if the hit game object is a room (ie. it is on layer 8)
            if (hitInfo.collider.gameObject.layer == 8) {
                clickedRoom = hitInfo.collider.gameObject;
            }
        }
        return clickedRoom;
    }

    private void deselect()
    {
        selectedObject = null;
    }

    private void monoColorAllRooms()
    {
        List<GameObject> allRooms = roomHandler.GetAllRooms();
        allRooms.ForEach(go => go.GetComponent<MeshRenderer>().material = defaultRoomMaterial);
    }

    private void hightlightSelectedRoom()
    {
        selectedObject.GetComponent<MeshRenderer>().material = selectedRoomMaterial;
    }
}
