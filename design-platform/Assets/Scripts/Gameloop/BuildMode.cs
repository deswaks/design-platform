using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : Mode
{
    // References to other objects in scene
    public MainLoop mainLoop;
    public Camera cam;
    public Building building;
    public int selectedShape = 0; //0 is rectangle and 1 is L-shape

    // Set at runtime
    public Room previewRoom;

    // Constants
    private Plane basePlane = new Plane(Vector3.up, Vector3.zero);

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
    }

    public override void OnModeResume()
    {
        if (previewRoom == null) {
            previewRoom = new Room();
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
        building.AddRoom(previewRoom);
    }

   
    // Moves Preview room with the mouse
    public void UpdatePreview()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);  //simple ray cast from the main camera. Notice there is no range

        float distance;
        if (basePlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            previewRoom.Move(hitPoint);
        }
        //Nyttig funktion: ElementSelection.GetPerimeterEdges()
    }
}
