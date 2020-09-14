using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

public class RoomBuilder : MonoBehaviour
{
    public Camera cam;
    private Plane basePlane = new Plane(Vector3.up, Vector3.zero);

    public Material defaultMaterial;
    public Material selectionMaterial;

    //public BuildSelector selector;

    private GameObject previewObject; // Object that will be moving around in the scene
    private GameObject currentlySelectedObject; // The currently selected room (that's already placed)
    private List<GameObject> allRoomObjects; 
    
    private ProBuilderMesh m_Mesh;
    private float m_Height = 3f;
    private bool m_FlipNormals = false;

    public GameObject moveHandlePrefab;

    private bool isCurrentlyBuilding = false;

    private Grid grid;

    private void Awake()
    {
        grid = FindObjectOfType<Grid>();
        allRoomObjects = FindObjectsOfType(typeof(GameObject)).Cast<GameObject>().Where(go => go.layer == 8).ToList(); //Finds rooms already placed
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
        previewObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        previewObject.layer = 8;
        previewObject.AddComponent(typeof(MeshCollider));
        
        
        List<Vector3> points = new List<Vector3> {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 3),
            new Vector3(3, 0, 3),
            new Vector3(3, 0, 0)
        };


        //LineRenderer lineRenderer = (LineRenderer)previewObject.AddComponent(typeof(LineRenderer));
        //lineRenderer.widthMultiplier = 0.5f;
        //lineRenderer.positionCount = points.Count();
        //lineRenderer.SetPositions(points.ToArray());
        //lineRenderer.material = defaultMaterial;
        //// A simple 2 color gradient with a fixed alpha of 1.0f.
        //float alpha = 1.0f;
        //Gradient gradient = new Gradient();
        //gradient.SetKeys(
        //    new GradientColorKey[] { new GradientColorKey(Color.black, 0.0f), new GradientColorKey(Color.black, 1.0f) },
        //    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        //);
        //lineRenderer.colorGradient = gradient;


        m_Mesh.CreateShapeFromPolygon(points, m_Height, m_FlipNormals);

        isCurrentlyBuilding = true;
    }
    public void InstantiateLShape()
    {
        // Create a new GameObject
        previewObject = new GameObject();

        // Add a ProBuilderMesh component (ProBuilder mesh data is stored here)
        m_Mesh = previewObject.gameObject.AddComponent<ProBuilderMesh>();
        previewObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        previewObject.layer = 8;
        previewObject.AddComponent(typeof(MeshCollider));

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
    }

    private void BuildIt()//actually build the thing
    {
        GameObject newObject = Instantiate(previewObject);
        allRoomObjects.Add(newObject); // Adds room to list of all instantiated ones

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
                allRoomObjects.ForEach(go => go.GetComponent<MeshRenderer>().material = defaultMaterial);
                currentlySelectedObject = hitInfo.collider.gameObject;
                Debug.Log(currentlySelectedObject.name);
                currentlySelectedObject.GetComponent<MeshRenderer>().material = selectionMaterial;
            }
        }
    }
    
    // Changes position of preview object (Room following mouse while building rooms)
    private void PositionObj(Vector3 _pos)
    {
        var finalPosition = grid.GetNearestPointOnGrid(_pos);// + new Vector3(0, preview.transform.localScale.z / 2, 0);

        //LineRenderer lineRenderer = (LineRenderer)previewObject.GetComponent(typeof(LineRenderer));
        
        //Vector3 translation = previewObject.transform.position - finalPosition;
      
        //lineRenderer.SetPositions(  + translation);

        previewObject.transform.position = finalPosition;
    }


    public bool GetIsBuilding()//just returns the isBuilding bool, so it cant get changed by another script
    {
        return isCurrentlyBuilding;
    }
    
    public void DeleteRoom()
    {
        allRoomObjects.Remove(currentlySelectedObject);
        Destroy(currentlySelectedObject);
        currentlySelectedObject = null;
    }
    public void RotateRoom()
    {
        currentlySelectedObject.transform.RotateAround(grid.GetNearestPointOnGrid(currentlySelectedObject.GetComponent<Renderer>().bounds.center), new Vector3(0, 1, 0), 90f);  //spins like a top, in 90 degree turns

    }
    public void MoveRoom()
    {
        GameObject moveHandle = Instantiate(moveHandlePrefab);
        Vector3 handlePosition = currentlySelectedObject.GetComponent<Renderer>().bounds.center;
        handlePosition.y = m_Height+0.01f;
        moveHandle.transform.position = handlePosition;
    }
}


