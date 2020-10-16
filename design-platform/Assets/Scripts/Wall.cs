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


    public void InitializeWall(List<Vector3> startEndPoints, Vector3 normal, Room room = null) {
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
    public void test() {


        for (int i = 0; i < ModifyMode.Instance.selectedRoom.GetControlPoints(localCoordinates: false, closed: true).Count; i++) {
            Debug.Log(ModifyMode.Instance.selectedRoom.GetControlPoints(localCoordinates: false, closed: true)[i]);



            testwallControlPoints = new List<Vector3> {
            ModifyMode.Instance.selectedRoom.GetControlPoints(localCoordinates: false, closed: true)[i],
            ModifyMode.Instance.selectedRoom.GetControlPoints(localCoordinates: false, closed: true)[i+1]
            };
            testnormal = ModifyMode.Instance.selectedRoom.GetWallNormals(localCoordinates: false)[i];
            testroom = ModifyMode.Instance.selectedRoom;


            prefabWall = Building.Instance.BuildWall(testwallControlPoints, testnormal, testroom);
        }
        

    }

}
