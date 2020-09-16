using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

public class RoomHandler : MonoBehaviour
{
    public Camera cam;


    public Material defaultRoomMaterial;

    private GameObject previewObject; // Object that will be moving around in the scene
    private GameObject currentlySelectedObject; // The currently selected room (that's already placed)
    private List<GameObject> allRoomObjects; 
    
    private ProBuilderMesh m_Mesh;
    private float m_Height = 3f;
    private bool m_FlipNormals = false;

    public GameObject moveHandlePrefab;

    private Grid grid;

    private void Awake()
    {
        grid = FindObjectOfType<Grid>();
        allRoomObjects = FindObjectsOfType(typeof(GameObject)).Cast<GameObject>().Where(go => go.layer == 8).ToList(); //Finds rooms already placed
    }

    //
    public void InstantiateRectangle()
    {
        // Create a new GameObject
        previewObject = new GameObject();

        // Add a ProBuilderMesh component (ProBuilder mesh data is stored here)
        m_Mesh = previewObject.gameObject.AddComponent<ProBuilderMesh>();
        previewObject.GetComponent<MeshRenderer>().material = defaultRoomMaterial;
        previewObject.layer = 8;
        previewObject.AddComponent(typeof(MeshCollider));
        
        
        List<Vector3> points = new List<Vector3> {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 3),
            new Vector3(3, 0, 3),
            new Vector3(3, 0, 0)
        };

        m_Mesh.CreateShapeFromPolygon(points, m_Height, m_FlipNormals);
    }

    //
    public void InstantiateLShape()
    {
        // Create a new GameObject
        previewObject = new GameObject();

        // Add a ProBuilderMesh component (ProBuilder mesh data is stored here)
        m_Mesh = previewObject.gameObject.AddComponent<ProBuilderMesh>();
        previewObject.GetComponent<MeshRenderer>().material = defaultRoomMaterial;
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
    }

    //
    public List<GameObject> GetAllRooms()
    {
        List<GameObject> allRooms = FindObjectsOfType(typeof(GameObject)).Cast<GameObject>().Where(go => go.layer == 8).ToList();
        return allRooms;
    }

    //
    public void DeleteRoom()
    {
        allRoomObjects.Remove(currentlySelectedObject);
        Destroy(currentlySelectedObject);
        currentlySelectedObject = null;
    }

    //
    public void RotateRoom()
    {
        currentlySelectedObject.transform.RotateAround(grid.GetNearestPointOnGrid(currentlySelectedObject.GetComponent<Renderer>().bounds.center), new Vector3(0, 1, 0), 90f);  //spins like a top, in 90 degree turns

    }

    //
    public void MoveRoom()
    {
        GameObject moveHandle = Instantiate(moveHandlePrefab);
        Vector3 handlePosition = currentlySelectedObject.GetComponent<Renderer>().bounds.center;
        handlePosition.y = m_Height+0.01f;
        moveHandle.transform.position = handlePosition;
    }

    // Changes position of preview object (Room following mouse while building rooms)
    public void PositionObj(Vector3 _pos)
    {
        var finalPosition = grid.GetNearestPointOnGrid(_pos);

        previewObject.transform.position = finalPosition;
    }
}


