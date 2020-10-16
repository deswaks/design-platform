using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;
using UnityEditorInternal;
using System.Runtime.InteropServices;

public class Wall : MonoBehaviour
{
    public Material wallMaterial;
    public Room parentRoom;
    public float wallThickness = 0.2f;
    private Wall prefabWall;
    private List<Vector3> wallControlPoints;

    private List<Vector3> testwallControlPoints;
    private Vector3 testnormal;
    private Room testroom;

    /// <summary>
    /// Construct walls.
    /// </summary>
    public void InitializeWall(List<Vector3> startEndPoints, Vector3 normal, Room room = null) {
        // lav en swich til hhv. bærende og ikke bærende vægge (fra Enum (opret Enum))
        // Vægge er pt baseret på room faces - skal ændres så de inddeles i room interfaces
            // Fjern duplicates
            // Opdel i subsections
        // Fix hjørnesamlinger

        parentRoom = room;
        gameObject.layer = 13; // Wall layer

        GameObject prefabWallObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WallPrefab.prefab");
        
        prefabWall = (Wall)prefabWallObject.GetComponent(typeof(Wall));
        gameObject.name = "CLT Wall";

        List<Vector3> dubStartEndPoints = startEndPoints.Select(v=> new Vector3(v.x,v.y,v.z)).ToList();
        wallControlPoints = new List<Vector3> {
            startEndPoints[0] + normal * wallThickness/2,
            startEndPoints[1] + normal * wallThickness/2,
            dubStartEndPoints[1] - normal * wallThickness/2,
            dubStartEndPoints[0] - normal * wallThickness/2 
        };

        gameObject.AddComponent<MeshCollider>();
        gameObject.AddComponent<PolyShape>();
        gameObject.AddComponent<ProBuilderMesh>();

        PolyShape polyshape = gameObject.GetComponent<PolyShape>();
        polyshape.SetControlPoints(wallControlPoints);
        polyshape.extrude = 3; //Height (get it from room?)
        polyshape.CreateShapeFromPolygon();
        
        gameObject.GetComponent<ProBuilderMesh>().Refresh();
        gameObject.GetComponent<MeshRenderer>().material = prefabWall.wallMaterial;

    }
    /// <summary>
    /// test to test Build and delete walls
    /// </summary>
    public void test() {
       
        if(Building.Instance.GetWalls().Count > 0) {
            Building.Instance.DeleteAllWalls();
        }
        
        foreach (Room r in Building.Instance.GetRooms()){
            for (int i = 0; i < r.GetControlPoints(localCoordinates: false, closed: true).Count - 1; i++) {
                testwallControlPoints = new List<Vector3> {
                    r.GetControlPoints(localCoordinates: false, closed: true)[i],
                    r.GetControlPoints(localCoordinates: false, closed: true)[i+1]
            };
                testnormal = r.GetWallNormals(localCoordinates: false)[i];
                testroom = r;


                prefabWall = Building.Instance.BuildWall(testwallControlPoints, testnormal, testroom);
            }
        }
        
    }
    /// <summary>
    /// Deletes a wall and removes it from the wall list.
    /// </summary>
    public void DeleteWall() {
        if (Building.Instance.GetWalls().Contains(this)) {
            parentRoom.parentBuilding.RemoveWall(this);
        }
        Destroy(gameObject);
    }
}
