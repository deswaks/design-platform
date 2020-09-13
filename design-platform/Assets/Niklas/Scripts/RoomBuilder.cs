using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class RoomBuilder : MonoBehaviour
{
    public Camera cam;
    private Plane basePlane = new Plane(Vector3.up, Vector3.zero);

    public Material material;
    public Material selectionMaterial;

    public BuildSelector selector;

    private GameObject previewObject; // Object that will be moving around in the scene
    private GameObject currentlySelectedObject; // The currently selected room (that's already placed)
    
    private ProBuilderMesh m_Mesh;
    private float m_Height = 3f;
    private bool m_FlipNormals = false;

    private bool isCurrentlyBuilding = false;

    private Grid grid;

    private void Awake()
    {
        grid = FindObjectOfType<Grid>();
    }


    private void Update()
    {
        if (isCurrentlyBuilding)
        {
            if (Input.GetMouseButtonDown(0))// && previewScript.CanBuild())//pressing LMB, and isBuiding = true, and the Preview Script -> canBuild = true
            {
                //InstantiateLShape();//then build the thing
                BuildIt();
            }

            if (Input.GetMouseButtonDown(1))//stop build
            {
                StopBuild();
            }

            if (Input.GetKeyDown(KeyCode.R))//for rotation
            {
                previewObject.transform.Rotate(0f, 90f, 0f);//spins like a top, in 90 degree turns
            }
        }
        DoRay();
        
    }

    public void InstantiateRectangle()
    {
        // Create a new GameObject
        previewObject = new GameObject();

        // Add a ProBuilderMesh component (ProBuilder mesh data is stored here)
        m_Mesh = previewObject.gameObject.AddComponent<ProBuilderMesh>();
        previewObject.GetComponent<MeshRenderer>().material = material;
        previewObject.layer = 8;

        List<Vector3> points = new List<Vector3> {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 3),
            new Vector3(3, 0, 3),
            new Vector3(3, 0, 0)
        };

        m_Mesh.CreateShapeFromPolygon(points, m_Height, m_FlipNormals);

        isCurrentlyBuilding = true;
    }
    public void InstantiateLShape()
    {
        // Create a new GameObject
        previewObject = new GameObject();

        // Add a ProBuilderMesh component (ProBuilder mesh data is stored here)
        m_Mesh = previewObject.gameObject.AddComponent<ProBuilderMesh>();
        previewObject.GetComponent<MeshRenderer>().material = material;
        previewObject.layer = 8;

        List<Vector3> points = new List<Vector3> {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 5),
            new Vector3(3, 0, 5),
            new Vector3(3, 0, 3),
            new Vector3(5, 0, 3),
            new Vector3(5, 0, 0) };

        m_Mesh.CreateShapeFromPolygon(points, m_Height, m_FlipNormals);

        isCurrentlyBuilding = true;
    }
    
    private void StopBuild()
    {
        Destroy(previewObject);//get rid of the preview
        previewObject = null;//not sure if you need this actually
        isCurrentlyBuilding = false;
        selector.TogglePanel();//toggle the button panel back on
    }

    private void BuildIt()//actually build the thing
    {
        Instantiate(previewObject);
        StopBuild();
    }
    
    private void DoRay()//simple ray cast from the main camera. Notice there is no range
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Moves current building with the mouse
        if (isCurrentlyBuilding)
        {
            float distance;
            if (basePlane.Raycast(ray, out distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                PositionObj(hitPoint);
            }
            //// Nyttig funktion: ElementSelection.GetPerimeterEdges()
        }

        // Detects selection on rooms already built
        if (!isCurrentlyBuilding)
        {
            UnityEngine.RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 8) && Input.GetMouseButtonDown(0))
            {
                currentlySelectedObject = hitInfo.collider.gameObject;
                currentlySelectedObject.GetComponent<MeshRenderer>().material = selectionMaterial;
            }
        }
    }
    
    // Changes position of preview object (Room following mouse while building rooms)
    private void PositionObj(Vector3 _pos)
    {
        var finalPosition = grid.GetNearestPointOnGrid(_pos);// + new Vector3(0, preview.transform.localScale.z / 2, 0);
        previewObject.transform.position = finalPosition;
    }


    public bool GetIsBuilding()//just returns the isBuilding bool, so it cant get changed by another script
    {
        return isCurrentlyBuilding;
    }
    

}


