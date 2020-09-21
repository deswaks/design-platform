using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Room : MonoBehaviour
{
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Building parentBuilding;
    public float height = 3.0f;
    
    private bool isHighlighted { set; get; }
    private ProBuilderMesh mesh3D;
    private Room prefabRoom;

    public enum RoomShapeTypes {
        Rectangular,
        L_Shaped,
        U_Shaped
    }
    private RoomShapeTypes roomShapeType;

    // Construct room of type 0 (Rectangle) or 1 (L-shape)
    public void InitializeRoom(int shape = 0, Building building = null) {
        GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RoomPrefab.prefab");
        prefabRoom = (Room)prefabObject.GetComponent(typeof(Room));
        parentBuilding = building;
        gameObject.layer = 8; // Rooom layer 


        PolyShape poly = gameObject.AddComponent<PolyShape>();

        mesh3D = gameObject.AddComponent<ProBuilderMesh>();
        gameObject.GetComponent<MeshRenderer>().material = prefabRoom.defaultMaterial;


        List<Vector3> points;
        if (shape == 0) { // Rectangle vertices
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

    public List<Vector3> ControlPoints() {
        List<Vector3> controlPoints = gameObject.GetComponent<PolyShape>().controlPoints.Select(p=> gameObject.transform.TransformPoint(p)).ToList();
        return controlPoints;
    }

}