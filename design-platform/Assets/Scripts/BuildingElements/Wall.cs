using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditorInternal;
using System.Runtime.InteropServices;

public class Wall : MonoBehaviour {
    public Interface interFace { get; private set; }
    public float wallThickness = 0.2f;
    public Material wallMaterial;
    private Wall prefabWall;

    /// <summary>
    /// Construct walls.
    /// </summary>
    public void InitializeWall(Interface interFace) {
        // lav swich wall types (fra Enum - WallType)
        // Fix hjørnesamlinger

        this.interFace = interFace;

        gameObject.layer = 13; // Wall layer

        GameObject prefabWallObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WallPrefab.prefab");
        prefabWall = (Wall)prefabWallObject.GetComponent(typeof(Wall));
        gameObject.name = "CLT Wall";


        List<Vector3> startEndPoints = new List<Vector3>() { interFace.GetStartPoint(),
                                                             interFace.GetEndPoint()};
        Vector3 normal = Vector3.Cross(startEndPoints[1]-startEndPoints[0], Vector3.up).normalized;
        List<Vector3> dubStartEndPoints = startEndPoints.Select(v => new Vector3(v.x, v.y, v.z)).ToList();
        List<Vector3> wallControlPoints = new List<Vector3> {
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
    /// Deletes a wall and removes it from the wall list.
    /// </summary>
    public void DeleteWall() {
        if (Building.Instance.walls.Contains(this)) {
            Building.Instance.RemoveWall(this);
        }
        Destroy(gameObject);
    }
}
