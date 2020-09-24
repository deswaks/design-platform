using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;

public class Room : MonoBehaviour
{
    public Material defaultMaterial;
    public Material highlightMaterial;

    public Building parentBuilding;
    public float height = 3.0f;
    public GameObject moveHandlePrefab;
    
    private bool isHighlighted { set; get; }
    private ProBuilderMesh mesh3D;
    private Room prefabRoom;

    private GameObject moveHandle;
    private bool isRoomInMoveMode = false;

    private Vector3 moveModeScreenPoint;
    private Vector3 moveModeOffset;

    private Vector3 lastLegalPlacementPoint;

    public enum RoomStates {
        Stationary,
        Preview,
        Moving
    }
    private RoomStates roomState;

    public enum RoomShapeTypes{
        Rectangular,
        L_Shaped,
        U_Shaped
    }
    private RoomShapeTypes roomShapeType;

    public bool isCurrentlyColliding = false;

    // Construct room of type 0 (Rectangle) or 1 (L-shape)
    public void InitializeRoom(int shape = 0, Building building = null)
    {
        GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RoomPrefab.prefab");
        
        prefabRoom = (Room)prefabObject.GetComponent(typeof(Room));

        parentBuilding = building;

        gameObject.layer = 8; // Rooom layer 
        roomState = RoomStates.Preview;

        
        PolyShape poly = gameObject.AddComponent<PolyShape>();
        
        mesh3D = gameObject.AddComponent<ProBuilderMesh>();
        gameObject.GetComponent<MeshRenderer>().material = prefabRoom.defaultMaterial;
        
        
        List<Vector3> points;
        if (shape == 0){ // Rectangle vertices
            points = new List<Vector3> {new Vector3(0, 0, 0),
                                        new Vector3(0, 0, 3),
                                        new Vector3(3, 0, 3),
                                        new Vector3(3, 0, 0)};
            roomShapeType = RoomShapeTypes.Rectangular;
        }
         else { // L-shape vertices
            points = new List<Vector3> {new Vector3(0, 0, 0),
                                        new Vector3(0, 0, 5),
                                        new Vector3(3, 0, 5),
                                        new Vector3(3, 0, 3),
                                        new Vector3(5, 0, 3),
                                        new Vector3(5, 0, 0)};
            roomShapeType = RoomShapeTypes.L_Shaped;
        }

        poly.SetControlPoints(points);
        poly.extrude = height;
        poly.CreateShapeFromPolygon();
        
        gameObject.AddComponent<MeshCollider>();

        RoomCollider.CreateAndAttachCollidersOfRoomShape(gameObject,roomShapeType);
    }

    public void SetRoomState(RoomStates roomState) {
        this.roomState = roomState;
    }

    // Rotates the room. Defaults to 90 degree increments
    public void Rotate(bool clockwise = true, float degrees = 90)
    {
        if (!clockwise) { degrees = -degrees; }
        gameObject.transform.RotateAround(
            point : parentBuilding.grid.GetNearestGridpoint(gameObject.GetComponent<Renderer>().bounds.center),
            axis : new Vector3(0, 1, 0),
            angle : degrees);
    }

    // Deletes the room
    public void Delete()
    {
        if (parentBuilding) { parentBuilding.RemoveRoom(this); }
        Destroy(gameObject);
    }

    // Moves the room to the given position
    public void Move(Vector3 exactPosition)
    {
        Vector3 gridPosition = parentBuilding.grid.GetNearestGridpoint(exactPosition);
        gameObject.transform.position = gridPosition;
    }

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

    public void SetIsInMoveMode(bool isInMoveMode = false) 
    {
        // Destroys any prior movehandle
        if (moveHandle != null)
        {
            Destroy(moveHandle);
            moveHandle = null;
        }

        if (isInMoveMode == true){
            roomState = RoomStates.Moving;
            moveHandle = Instantiate(prefabRoom.moveHandlePrefab);
            Vector3 handlePosition = gameObject.GetComponent<Renderer>().bounds.center;
            handlePosition.y = height + 0.01f;
            moveHandle.transform.position = handlePosition;

            moveHandle.transform.SetParent(gameObject.transform,true);
        }
        else {
            roomState = RoomStates.Stationary;
        }
    }

    void OnMouseDown()
    {
        if (roomState == RoomStates.Moving)
        {
            moveModeScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            moveModeOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void OnMouseDrag()
    {
        if (roomState == RoomStates.Moving)
        {
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + moveModeOffset;
            transform.position = parentBuilding.grid.GetNearestGridpoint(curPosition);
        }
    }

    public void SetIsRoomCurrentlyColliding() {

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

    public Room GetPrefabRoom() { return prefabRoom; }
}
