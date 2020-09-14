using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshGenerator : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float width;
    public float height;

    void Start()
    {
    }

    void Update()
    {
        // Create new mash object
        Mesh mesh = new Mesh();

        // Create vertices list
        Vector3[] vertices = new Vector3[4];

        // Define vertices
        vertices[0] = new Vector3(-width, 1, -height);
        vertices[1] = new Vector3(-width, 1, height);
        vertices[2] = new Vector3(width, 1, height);
        vertices[3] = new Vector3(width, 1, -height);

        // Add vertices and their faces to mesh
        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        // add mesh to meshfilter
        meshFilter.mesh = mesh;
    }
}
