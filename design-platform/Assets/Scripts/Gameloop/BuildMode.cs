using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : Mode
{
    // References to other objects in scene
    public MainLoop mainLoop;
    public Building building;
    public Camera camera;
    private int selectedShape { get; set; } //0 is rectangle and 1 is L-shape

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
        if (Input.GetMouseButtonDown(0)) {
            Build();
        }

        if (Input.GetMouseButtonDown(1)) {
           //Change from rectangle to L-shape and vice versa
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            previewRoom.Rotate();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            mainLoop.setMode(mainLoop.modifyMode);
        }

        UpdatePreviewLocation();
    }

    public override void OnModeResume()
    {
        camera = Camera.current;
        if (previewRoom == null) {
            previewRoom = new Room(selectedShape);
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
        Room newRoom = new Room(selectedShape);
        newRoom.transform.position = previewRoom.transform.position;
        newRoom.transform.rotation = previewRoom.transform.rotation;
        mainLoop.building.AddRoom(newRoom);
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
}
