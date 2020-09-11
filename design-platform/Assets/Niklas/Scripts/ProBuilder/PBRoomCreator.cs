using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;



public class PBRoomCreator : MonoBehaviour
{

    public float m_Height = 1f;
    public bool m_FlipNormals = false;

    ProBuilderMesh m_Mesh;

    void Start()
    {
        // Create a new GameObject
        var go = new GameObject();

        // Add a ProBuilderMesh component (ProBuilder mesh data is stored here)
        m_Mesh = go.gameObject.AddComponent<ProBuilderMesh>();

        List<Vector3> points = new List<Vector3> { 
            new Vector3(0, 0, 0), 
            new Vector3(0, 0, 10),
            new Vector3(5, 0, 10),
            new Vector3(5, 0, 5),
            new Vector3(10, 0, 5),
            new Vector3(10, 0, 0) };

        //InvokeRepeating("Rebuild", 0f, .1f);
        

        points.Sort(new ClockwiseVector3Comparer());
        m_Mesh.CreateShapeFromPolygon(points, m_Height, m_FlipNormals);
    }
    public class ClockwiseVector3Comparer : IComparer<Vector3>
    {
        public int Compare(Vector3 v1, Vector3 v2)
        {
            return Mathf.Atan2(v1.x, v1.z).CompareTo(Mathf.Atan2(v2.x, v2.z));
        }
    }
    //void Rebuild()
    //{
    //    // Create a circle of points with randomized distance from origin.
    //    //Vector3[] points = new Vector3[32];
    //    List<Vector3> points = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(0, 0, 10), new Vector3(10, 0, 0) };

    //    // CreateShapeFromPolygon is an extension method that sets the pb_Object mesh data with vertices and faces
    //    // generated from a polygon path.
    //    m_Mesh.CreateShapeFromPolygon(points, m_Height, m_FlipNormals);

    //}

}
