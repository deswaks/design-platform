using System.Collections;
using System.Collections.Generic;
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


    // Construct room of type 0 (Rectangle) or 1 (L-shape)
    public void InitializeRoom(int shape = 0, Building building = null)
    {
        //GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath();
        parentBuilding = building;
        mesh3D = gameObject.AddComponent<ProBuilderMesh>();
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        gameObject.layer = 8;
        gameObject.AddComponent(typeof(MeshCollider));

        List<Vector3> points;
        if (shape == 0){ // Rectangle vertices
            points = new List<Vector3> {new Vector3(0, 0, 0),
                                        new Vector3(0, 0, 3),
                                        new Vector3(3, 0, 3),
                                        new Vector3(3, 0, 0)};}
         else { // L-shape vertices
            points = new List<Vector3> {new Vector3(0, 0, 0),
                                        new Vector3(0, 0, 5),
                                        new Vector3(3, 0, 5),
                                        new Vector3(3, 0, 3),
                                        new Vector3(5, 0, 3),
                                        new Vector3(5, 0, 0)};}

        mesh3D.CreateShapeFromPolygon(points, height, false);
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
        parentBuilding.RemoveRoom(this);
        Destroy(gameObject);
        //Destroy(this);
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
            gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            isHighlighted = true;
        }
        else {
            gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
            isHighlighted = false;
        }
    }

}
