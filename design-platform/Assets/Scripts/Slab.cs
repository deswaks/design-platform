using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slab : MonoBehaviour {
    public Interface interFace { get; private set; }
    public float slabThickness = 0.2f;
    public Material slabMaterial;
    private Slab prefabSlab;

    /// <summary>
    /// Construct walls.
    /// </summary>
    public void InitializeSlab(Interface interFace) {

        this.interFace = interFace;

        gameObject.layer = 14; // Slab layer

        GameObject prefabSlabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SlabPrefab.prefab");
        prefabSlab = (Slab)prefabSlabObject.GetComponent(typeof(Slab));
        gameObject.name = "CLT Slab";

        List<Vector3> slabControlPoints = new List<Vector3> {
            
        };

        //gameObject.AddComponent<MeshCollider>();
        //gameObject.AddComponent<PolyShape>();
        //gameObject.AddComponent<ProBuilderMesh>();

        //PolyShape polyshape = gameObject.GetComponent<PolyShape>();
        //polyshape.SetControlPoints(wallControlPoints);
        //polyshape.extrude = slabThickness; 
        //polyshape.CreateShapeFromPolygon();

        //gameObject.GetComponent<ProBuilderMesh>().Refresh();
        //gameObject.GetComponent<MeshRenderer>().material = prefabWall.wallMaterial;

    }

    /// <summary>
    /// Deletes a wall and removes it from the wall list.
    /// </summary>
    public void DeleteSlab() {
        //if (Building.Instance.walls.Contains(this)) {
        //    Building.Instance.RemoveWall(this);
        //}
        //Destroy(gameObject);
    }
}
