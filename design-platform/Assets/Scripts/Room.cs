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

    public bool isCurrentlyColliding = false;

    public enum RoomShapeTypes
    {
        Rectangular,
        L_Shaped,
        U_Shaped
    }
    private RoomShapeTypes roomShapeType;

    // Construct room of type 0 (Rectangle) or 1 (L-shape)
    public void InitializeRoom(int shape = 0, Building building = null)
    {
        GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RoomPrefab.prefab");
        
        prefabRoom = (Room)prefabObject.GetComponent(typeof(Room));

        parentBuilding = building;

        gameObject.layer = 8; // Rooom layer 

        
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
        
        mesh3D.Refresh();
        
        gameObject.AddComponent<MeshCollider>();

        RoomCollider.CreateAndAttachCollidersOfRoomShape(gameObject,roomShapeType);

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

    public void SetIsInMoveMode(bool isInMoveMode = false) //klar til implementering
    {
        isRoomInMoveMode = isInMoveMode;

        // Destroys any prior movehandle
        if (moveHandle != null)
        {
            Destroy(moveHandle);
            moveHandle = null;
        }

        if (isInMoveMode == true){

            moveHandle = Instantiate(prefabRoom.moveHandlePrefab);
            Vector3 handlePosition = gameObject.GetComponent<Renderer>().bounds.center;
            handlePosition.y = height + 0.01f;
            moveHandle.transform.position = handlePosition;

            moveHandle.transform.SetParent(gameObject.transform,true);
        }
    }

    void OnMouseDown()
    {
        if (isRoomInMoveMode)
        {
            moveModeScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            moveModeOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void OnMouseDrag()
    {
        if (isRoomInMoveMode)
        {
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + moveModeOffset;
            transform.position = parentBuilding.grid.GetNearestGridpoint(curPosition);
        }
    }

    public bool GetIsMoveHandleVisible() { return isRoomInMoveMode; }
    public Room GetPrefabRoom(){ return prefabRoom; }

}






/// /////////////////////////////////////////////////////////////////////////////////////////////// TEST 
public class RoomCollider : MonoBehaviour
{
    // Detects collision with other rooms when placing them
    void OnCollisionEnter(Collision other)
    {
        GameObject parentObject = gameObject.transform.parent.gameObject;
        // Only acts if collision object is other room room object and not a sibling (other colliders in same room)
        if(other.gameObject.GetComponent<Room>() && other.gameObject != parentObject )
        {
            parentObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
            Debug.Log("Collision enter with " + other.gameObject.name);
        }


    }
    // Resets color when collision ends (showing that you are allowed to place the room)
    void OnCollisionExit(Collision other)
    {
        GameObject parentObject = gameObject.transform.parent.gameObject;

        if (other.gameObject.GetComponent<Room>() && other.gameObject != parentObject)
        {
            parentObject.GetComponent<MeshRenderer>().material = parentObject.GetComponent<Room>().GetPrefabRoom().defaultMaterial;
            Debug.Log("Exiting collision with " + other.gameObject.name);
        }        
    }


    public static void CreateAndAttachCollidersOfRoomShape(GameObject roomGameObject, Room.RoomShapeTypes roomShapeType){


        PolyShape roomPolyShape    = roomGameObject.GetComponent<PolyShape>();
        List<Vector3> vertices     = roomPolyShape.controlPoints.ToList();
        float height               = roomPolyShape.extrude;

        // Every collider cube is defined by the controlpoints of the room object. Both the x- and y- position of the collider cube is defined by two indices each.
        // For instance, the location of the collider cube for a rectangular room is defined as follows: 
        //      The x coordinate of the location is defined by the average value from the [1] and [2] controlpoints (or [0] and [3]).
        //      The y coordinate of the location is defined by the average value from the [0] and [1] controlpoints (or [2] and [3]).
        List<List<int>> colliderIndexPairsList = new List<List<int>>();

        switch (roomShapeType)
        {
            case Room.RoomShapeTypes.Rectangular:
                //colliderIndexPairsList.Add(new List<int> { x1, x2, y1, y2 });
                colliderIndexPairsList.Add( new List<int> { 1, 2, 0, 1 } ); 

                break;
            case Room.RoomShapeTypes.L_Shaped:
                colliderIndexPairsList.Add(new List<int> { 1, 2, 0, 1 }); // Collider cube 1
                colliderIndexPairsList.Add(new List<int> { 3, 4, 0, 3 }); // Collider cube 2
                break;
        }

        int counter = 0;
        foreach(List<int> xyPairs in colliderIndexPairsList)
        {
            GameObject colliderObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderObject.name = "RoomCollider_" + counter++.ToString();


            Vector3 positionVector =
                new Vector3(
                    vertices[xyPairs[0]].x + System.Math.Abs(vertices[xyPairs[0]].x - vertices[xyPairs[1]].x) / 2, // x - location
                    height / 2 - 0.005f,                                                                           // y - location
                    vertices[xyPairs[2]].z + System.Math.Abs(vertices[xyPairs[2]].z - vertices[xyPairs[3]].z) / 2  // z - location
                    );
            Vector3 scaleVector =
                new Vector3(
                    System.Math.Abs(vertices[ xyPairs[0] ].x - vertices[ xyPairs[1] ].x), // x - scale
                    height - 0.01f,                                                       // y - scale
                    System.Math.Abs(vertices[ xyPairs[2] ].z - vertices[ xyPairs[3] ].z)  // z - scale
                    );
            
            colliderObject.transform.parent        = roomGameObject.transform;
            colliderObject.transform.localScale    = scaleVector;
            colliderObject.transform.localPosition = positionVector;

            colliderObject.GetComponent<BoxCollider>().isTrigger = false;
            Rigidbody rigidBody = colliderObject.AddComponent<Rigidbody>();
            rigidBody.isKinematic = false;
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;

            colliderObject.AddComponent<RoomCollider>();
        }
    }

}
