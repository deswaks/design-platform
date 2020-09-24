using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;

public class Room : MonoBehaviour
{
    private List<Vector3> controlPoints;
    private RoomShape shape;
    public Material defaultMaterial;
    public Material highlightMaterial;

    public Building parentBuilding;
    public float height = 3.0f;

    private bool isHighlighted { set; get; }
    private ProBuilderMesh mesh3D;
    private Room prefabRoom;
    
    
    private GameObject moveHandle;
    private bool isRoomInMoveMode = false;

    public GameObject moveHandlePrefab;
    private Vector3 moveModeScreenPoint;
    private Vector3 moveModeOffset;

    private Vector3 lastLegalPlacementPoint;

    public enum RoomStates {
        Stationary,
        Preview,
        Moving
    }
    private RoomStates roomState;

    public bool isCurrentlyColliding = false;

    // Construct room of type 0 (Rectangle) or 1 (L-shape)
    public void InitializeRoom(RoomShape buildShape = RoomShape.RECTANGLE, Building building = null) {
        // Set constant values
        parentBuilding = building;
        gameObject.layer = 8; // Rooom layer
        
        shape = buildShape;
        roomState = RoomStates.Preview;

        // Get relevant properties from prefab object
        GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RoomPrefab.prefab");
        
        prefabRoom = (Room)prefabObject.GetComponent(typeof(Room));

        switch (shape) {
            case RoomShape.RECTANGLE:
                controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                   new Vector3(0, 0, 3),
                                                   new Vector3(3, 0, 3),
                                                   new Vector3(3, 0, 0)};
                gameObject.name = "Room(Rectangle)";
                break;
            case RoomShape.LSHAPE:
                controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 0)};
                gameObject.name = "Room(L-Shape)";
                break;
        }

        // Set room visualization geometry
        gameObject.AddComponent<PolyShape>();
        gameObject.AddComponent<ProBuilderMesh>();
        gameObject.GetComponent<MeshRenderer>().material = prefabRoom.defaultMaterial;
        RefreshView();

        // Create and attach collider objects
        gameObject.AddComponent<MeshCollider>();
        RoomCollider.CreateAndAttachCollidersOfRoomShape(gameObject, buildShape);
    }

    public void RefreshView() {
        PolyShape polyshape = gameObject.GetComponent<PolyShape>();
        polyshape.SetControlPoints(controlPoints);
        polyshape.extrude = height;
        polyshape.CreateShapeFromPolygon();
        gameObject.GetComponent<ProBuilderMesh>().Refresh();
    }

    /// <summary>
    /// Rotates the room. Defaults to 90 degree increments
    /// </summary>
    public void Rotate(bool clockwise = true, float degrees = 90)
    {
        if (!clockwise) { degrees = -degrees; }
        gameObject.transform.RotateAround(
            point: parentBuilding.grid.GetNearestGridpoint(gameObject.GetComponent<Renderer>().bounds.center),
            axis: new Vector3(0, 1, 0),
            angle: degrees);
    }

    /// <summary>
    /// Deletes the room
    /// </summary>
    public void Delete()
    {
        if (parentBuilding) { parentBuilding.RemoveRoom(this); }
        Destroy(gameObject);
    }

    /// <summary>
    /// Moves the room to the given position
    /// </summary>
    public void Move(Vector3 exactPosition)
    {
        Vector3 gridPosition = parentBuilding.grid.GetNearestGridpoint(exactPosition);
        gameObject.transform.position = gridPosition;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetIsHighlighted(bool highlighted)
    {
        if (highlighted) {
            gameObject.GetComponent<MeshRenderer>().material = prefabRoom.highlightMaterial;
            isHighlighted = true;
        }
        else {
            gameObject.GetComponent<MeshRenderer>().material = prefabRoom.defaultMaterial;
            isHighlighted = false;
        }
    }

    /// <summary>
    /// Gets a list of controlpoints - in local coordinates. The controlpoints are the vertices of the underlying polyshape of the building.
    /// </summary>
    public List<Vector3> GetControlPoints(bool localCoordinates = false, bool closed = false) {
        List<Vector3> returnPoints = controlPoints;
        if (closed) {
            returnPoints = controlPoints.Concat(new List<Vector3> { controlPoints[0] }).ToList();
        }
        if (!localCoordinates) {
            returnPoints = returnPoints.Select(p => gameObject.transform.TransformPoint(p)).ToList();
        }
        return returnPoints;
    }

    /// <summary>
    /// Gets a list of controlpoints. The controlpoints are the vertices of the underlying polyshape of the building.
    /// </summary>
    public List<Vector3> GetWallMidpoints(bool localCoordinates = false) {
        List<Vector3> midPoints = new List<Vector3>();
        List<Vector3> circularControlpoints = GetControlPoints(localCoordinates: localCoordinates, closed: true);
        for (int i = 0; i < controlPoints.Count; i++) {
            midPoints.Add((circularControlpoints[i] + circularControlpoints[i + 1]) / 2);
        }
        return midPoints;
    }

    /// <summary>
    /// Gets a list of normals. The are in same order as controlpoints (clockwise).
    /// </summary>
    public List<Vector3> GetWallNormals() {
        List<Vector3> wallNormals = new List<Vector3>();
        List<Vector3> circularControlpoints = controlPoints.Concat(new List<Vector3> { controlPoints[0] }).ToList();
        for (int i = 0; i < controlPoints.Count; i++) {
            wallNormals.Add(Vector3.Cross((circularControlpoints[i + 1] - circularControlpoints[i]),Vector3.up).normalized);
        }
        return wallNormals;
    }


    /// <summary>
    /// 
    /// </summary>
    public void SetIsInMoveMode(bool isInMoveMode = false) //klar til implementering
    {
        // Destroys any prior movehandle
        if (moveHandle != null) {
            Destroy(moveHandle);
            moveHandle = null;
        }

        if (isInMoveMode == true){
            roomState = RoomStates.Moving;
            moveHandle = Instantiate(prefabRoom.moveHandlePrefab);
            Vector3 handlePosition = gameObject.GetComponent<Renderer>().bounds.center;
            handlePosition.y = height + 0.01f;
            moveHandle.transform.position = handlePosition;

            moveHandle.transform.SetParent(gameObject.transform, true);
        }
        else {
            roomState = RoomStates.Stationary;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnMouseDown()
    {
        if (roomState == RoomStates.Moving)
        {
            moveModeScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            moveModeOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnMouseDrag()
    {
        if (roomState == RoomStates.Moving)
        {
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + moveModeOffset;
            transform.position = parentBuilding.grid.GetNearestGridpoint(curPosition);
        }
    }

    public void SetIsRoomCurrentlyColliding() {
        Debug.Log("Is colliding");
        if(roomState == RoomStates.Preview || roomState == RoomStates.Moving) { // Only triggers collision events on moving object

            List<bool> collidersColliding = gameObject.GetComponentsInChildren<RoomCollider>().Select(rc => rc.isCurrentlyColliding).ToList(); // list of whether or not each collider is currently colliding

            if (collidersColliding.TrueForAll(b => !b)) { // if there are no collisions in any of room's colliders
                isCurrentlyColliding = false;
                gameObject.GetComponent<MeshRenderer>().material = prefabRoom.defaultMaterial;

            }
            else { // if there is one or more collision(s)
                isCurrentlyColliding = true;
                gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }
    public void SetRoomState(RoomStates roomState) {
        this.roomState = roomState;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool GetIsMoveHandleVisible() { return isRoomInMoveMode; }

    /// <summary>
    /// 
    /// </summary>
    public Room GetPrefabRoom(){ return prefabRoom; }



}
