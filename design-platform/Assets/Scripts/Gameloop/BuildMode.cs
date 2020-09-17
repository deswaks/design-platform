﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : Mode
{
    // References to other objects in scene
    public MainLoop mainLoop;
    public Building building;
    public Camera camera = Camera.main;
    private int selectedShape { get; set; } //0 is  rectangle and 1 is L-shape

    // Set at runtime
    public Room previewRoom;

    public BuildMode(MainLoop mainLoop, Building building)
    {
        this.mainLoop = mainLoop;
        this.building = building;
        selectedShape = 0;
}

public override void Tick()
    {
        UpdatePreviewLocation();

        if (Input.GetMouseButtonDown(0)) {
            Build();
        }

        if (Input.GetMouseButtonDown(1)) {
           //Change from rectangle to L-shape and vice versa
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            previewRoom.Rotate(); //remember to tell this to the user when implementing tooltips
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            mainLoop.setMode(mainLoop.modifyMode);
        }
    }

    public override void OnModeResume()
    {
        Debug.Log("Build mode resumed");
        if (previewRoom == null) {
            previewRoom = building.BuildRoom(shape: selectedShape, preview: true);
        }
    }

    public override void OnModePause()
    {
        Debug.Log("Build mode paused");
        previewRoom.Delete();
        previewRoom = null;
    }

    //actually build the thing
    public void Build()
    {
        Room newRoom = building.BuildRoom(shape: selectedShape, templateRoom: previewRoom);
    }
   
    // Moves Preview room with the mouse
    public void UpdatePreviewLocation()
    {
        Plane basePlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        float distance;
        if (basePlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            previewRoom.Move(hitPoint);
        }

        //Nyttig funktion: ElementSelection.GetPerimeterEdges()
    }

    public void SetSelectedShape(int shape = 0)
    {
        selectedShape = shape;
    }
}
