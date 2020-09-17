using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using UnityEngine.EventSystems;

public class ModifyMode : Mode
{
    // References to other objects in scene
    public Main main;
    public Camera camera = Camera.main;

    public Material defaultRoomMaterial;
    public Material selectedRoomMaterial;

    // Set at runtime
    public Room selectedRoom;

    public ModifyMode(Main main)
    {
        this.main = main;
    }

    public override void Tick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                selectClickedRoom();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            deselect();
        }
    }

    public override void OnModeResume()
    {
        //camera = Camera.current;
    }

    public override void OnModePause()
    {
        deselect();
    }
    private void selectClickedRoom()
    {
        deselect();
        selectedRoom = GetClickedRoom();
        if (selectedRoom != null)
        {
            selectedRoom.SetIsHighlighted(true);
        }
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
        if (selectedRoom != null)
        {
            selectedRoom.SetIsHighlighted(false);
        }
        selectedRoom = null;
    }
    public void MoveHandles() //klar til implementering
    {
        //GameObject moveHandle = Instantiate(moveHandlePrefab);
        //Vector3 handlePosition = currentlySelectedObject.GetComponent<Renderer>().bounds.center;
        //handlePosition.y = height + 0.01f;
        //moveHandle.transform.position = handlePosition;
    }
}
