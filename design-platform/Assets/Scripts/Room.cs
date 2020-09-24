using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Room : MonoBehaviour {
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


    // Construct room of type 0 (Rectangle) or 1 (L-shape)
    public void InitializeRoom(int shape = 0, Building building = null) {
        GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RoomPrefab.prefab");
        prefabRoom = (Room)prefabObject.GetComponent(typeof(Room));
        parentBuilding = building;

        mesh3D = gameObject.AddComponent<ProBuilderMesh>();
        gameObject.GetComponent<MeshRenderer>().material = prefabRoom.defaultMaterial;
        gameObject.layer = 8;
        gameObject.AddComponent(typeof(MeshCollider));

        List<Vector3> points;
        if (shape == 0) { // Rectangle vertices
            points = new List<Vector3> {new Vector3(0, 0, 0),
                                        new Vector3(0, 0, 3),
                                        new Vector3(3, 0, 3),
                                        new Vector3(3, 0, 0)};
        }
        else { // L-shape vertices
            points = new List<Vector3> {new Vector3(0, 0, 0),
                                        new Vector3(0, 0, 5),
                                        new Vector3(3, 0, 5),
                                        new Vector3(3, 0, 3),
                                        new Vector3(5, 0, 3),
                                        new Vector3(5, 0, 0)};
        }

        mesh3D.CreateShapeFromPolygon(points, height, false);



    }

    // Rotates the room. Defaults to 90 degree increments
    public void Rotate(bool clockwise = true, float degrees = 90) {
        if (!clockwise) { degrees = -degrees; }
        gameObject.transform.RotateAround(
            point: parentBuilding.grid.GetNearestGridpoint(gameObject.GetComponent<Renderer>().bounds.center),
            axis: new Vector3(0, 1, 0),
            angle: degrees);
    }

    // Deletes the room
    public void Delete() {
        if (parentBuilding) { parentBuilding.RemoveRoom(this); }
        Destroy(gameObject);
    }

    // Moves the room to the given position
    public void Move(Vector3 exactPosition) {
        Vector3 gridPosition = parentBuilding.grid.GetNearestGridpoint(exactPosition);
        gameObject.transform.position = gridPosition;
    }

    public void SetIsHighlighted(bool highlighted) {
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
        if (moveHandle != null) {
            Destroy(moveHandle);
            moveHandle = null;
        }

        if (isInMoveMode == true) {

            moveHandle = Instantiate(prefabRoom.moveHandlePrefab);
            Vector3 handlePosition = gameObject.GetComponent<Renderer>().bounds.center;
            handlePosition.y = height + 0.01f;
            moveHandle.transform.position = handlePosition;

            moveHandle.transform.SetParent(gameObject.transform, true);
        }
    }


    void OnMouseDown() {
        if (isRoomInMoveMode) {
            moveModeScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            moveModeOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);// new Vector3(Input.mousePosition.x, Input.mousePosition.y, moveModeScreenPoint.z));
        }
    }

    void OnMouseDrag() {
        //Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, moveModeScreenPoint.z);
        if (isRoomInMoveMode) {
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + moveModeOffset;
            transform.position = curPosition;
        }

    }

    public bool GetIsMoveHandleVisible() { return isRoomInMoveMode; }
    public Room GetPrefabRoom() { return prefabRoom; }

    // List of all walls of a given room (not sorted as points in base face)

    public List<Face> getAllWalls() {

        List<Face> allVerticalFaces = new List<Face>();

        for (int i = 2; i < mesh3D.faces.Count; i++) {
            allVerticalFaces.Add(mesh3D.faces[i]);
        }
        return allVerticalFaces;
    }
}
