using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SebastianRoom : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    Mesh mesh;
    List<Vector3> vertices = new List<Vector3> { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

    public void Begin(Vector3 newPoint)
    {
        vertices[0] = newPoint;
        //Debug.Log(vertices.Count.ToString());
    }
    public void UpdateRoom(Vector3 newPoint)
    {
        vertices[1] = new Vector3(vertices[0][0], newPoint[1], newPoint[2]);
        vertices[2] = newPoint;
        vertices[3] = new Vector3(newPoint[0], newPoint[1], vertices[0][2]);

        //Debug.Log(vertices[0].ToString() +","+ vertices[1].ToString() + "," + vertices[2].ToString() + "," + vertices[3].ToString());
        Render();
    }

    public void Render()
    {
        // Add vertices and their faces to mesh
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        //Debug.Log(mesh.triangles.Count().ToString());
        // add mesh to meshfilter
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}