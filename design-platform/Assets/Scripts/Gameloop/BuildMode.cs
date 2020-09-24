﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildMode : Mode
{
    // References to other objects in scene
    public Main main;
    public Camera camera = Camera.main;
    private RoomShape selectedShape { get; set; } //0 is rectangle and 1 is L-shape

    // Set at runtime
    public Room previewRoom;

    public BuildMode(Main main)
    {
        this.main = main;
        selectedShape = 0;
}

public override void Tick()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                Build();
            }
        }

        if (Input.GetMouseButtonDown(1)) {
           //Change from rectangle to L-shape and vice versa
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            previewRoom.Rotate(); //remember to tell this to the user when implementing tooltips
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            main.setMode(main.modifyMode);
        }

        if (previewRoom) {UpdatePreviewLocation();}
    }

    public override void OnModeResume()
    {
        if (previewRoom == null) {
            previewRoom = main.building.BuildRoom(buildShape: selectedShape, preview: true);
        }
    }

    public override void OnModePause()
    {
        previewRoom.Delete();
        previewRoom = null;
    }

    //actually build the thing
    public void Build()
    {
        Room builtRoom = main.building.BuildRoom(buildShape: selectedShape, templateRoom: previewRoom);
        builtRoom.SetRoomState(Room.RoomStates.Stationary);
    }
   
    // Moves Preview room with the mouse
    public void UpdatePreviewLocation()
    {
        Plane basePlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);  //simple ray cast from the main camera. Notice there is no range

        float distance;
        if (basePlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            previewRoom.Move(hitPoint);
        }
        //Nyttig funktion: ElementSelection.GetPerimeterEdges()
    }

    public void SetSelectedShape(RoomShape shape = RoomShape.RECTANGLE)
    {
        selectedShape = shape;
    }
}
