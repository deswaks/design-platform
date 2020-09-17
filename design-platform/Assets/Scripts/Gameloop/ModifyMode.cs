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
    public MainLoop mainLoop;
    public Building building;
    public Camera camera;

    public Material defaultRoomMaterial;
    public Material selectedRoomMaterial;

    // Set at runtime
    public Room selectedRoom;

    public ModifyMode(MainLoop mainLoop, Building building)
    {
        this.mainLoop = mainLoop;
        this.building = building;
    }

    public override void Tick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedRoom.SetIsHighlighted(false);
            deselect();
            selectedRoom = GetClickedRoom();
            selectedRoom.SetIsHighlighted(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            selectedRoom.SetIsHighlighted(false);
            deselect();
        }

    }

    public override void OnModeResume()
    {
        camera = Camera.current;
    }

    public override void OnModePause()
    {
        monoColorAllRooms();
    }

    private Room GetClickedRoom()
    {
        Room clickedRoom = null;
        UnityEngine.RaycastHit hitInfo;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray: ray, hitInfo: out hitInfo))
        {
            // if the hit game object is a room (ie. it is on layer 8)
            if (hitInfo.collider.gameObject.layer == 8)
            {
                clickedRoom = hitInfo.collider.gameObject.GetComponent<Room>();
            }
        }
        return clickedRoom;
    }

    private void deselect()
    {
        selectedRoom = null;
    }

    private void monoColorAllRooms()
    {
        List<Room> allRooms = building.GetAllRooms();

        allRooms.ForEach(go => go.GetComponent<MeshRenderer>().material = defaultRoomMaterial);
    }

    private void highlightSelectedRoom()
    {
        if (selectedRoom != null) {
            selectedRoom.GetComponent<MeshRenderer>().material = selectedRoomMaterial;
        }
    }

    public void MoveHandles()
    {
        //GameObject moveHandle = Instantiate(moveHandlePrefab);
        //Vector3 handlePosition = currentlySelectedObject.GetComponent<Renderer>().bounds.center;
        //handlePosition.y = height + 0.01f;
        //moveHandle.transform.position = handlePosition;
    }
}
