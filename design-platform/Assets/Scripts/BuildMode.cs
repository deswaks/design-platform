//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BuildMode : Mode
//{
//    // References to other objects in scene
//    public Camera cam;
//    public RoomHandler roomhandler;
//    public int selectedShape = 0; //0 is rectangle and 1 is L-shape

//    // Set at runtime
//    public GameObject previewObject;

//    // Constants
//    private Plane basePlane = new Plane(Vector3.up, Vector3.zero);

//    public override void Tick()
//    {
//        if (Input.GetMouseButtonDown(0))// && previewScript.CanBuild())//pressing LMB, and isBuiding = true, and the Preview Script -> canBuild = true
//        {
//            //InstantiateLShape();//then build the thing
//            roomhandler.BuildIt();
//        }

//        if (Input.GetMouseButtonDown(1))//stop build
//        {
//            roomhandler.StopBuild();
//        }

//        if (Input.GetKeyDown(KeyCode.R))//for rotation
//        {
//            previewObject.transform.Rotate(0f, 90f, 0f);//spins like a top, in 90 degree turns
//        }
//    }

//    public override void OnModeResume()
//    {
//        GameObject newObject = Instantiate(previewObject);
//    }

//    public override void OnModePause()
//    {
//        Destroy(previewObject);//get rid of the preview
//        previewObject = null;//not sure if you need this actually
//    }

//    //actually build the thing
//    public void BuildIt()
//    {
//        GameObject newObject = Instantiate(previewObject);
//        allRoomObjects.Add(newObject); // Adds room to list of all instantiated ones
//        StopBuild();
//    }

//    //simple ray cast from the main camera. Notice there is no range
//    public void UpdatePreview()
//    {
//        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

//        // Moves current building with the mouse

//        float distance;
//        if (basePlane.Raycast(ray, out distance))
//        {
//            Vector3 hitPoint = ray.GetPoint(distance);
//            roomhandler.PositionObj(hitPoint);
//        }
//        //// Nyttig funktion: ElementSelection.GetPerimeterEdges()
//    }
//}
