using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2D
{
    private Vector2[] Vertices { get; set; }

    /// <summary>
    /// Create a 2D Polygon
    /// </summary>
    /// <param name="vertices">The vertices of the polygon (X,Y)</param>
    public Polygon2D(Vector2[] vertices) {
        Vertices = vertices;
    }

    /// <summary>
    /// Calculate the area of the polygon.
    /// </summary>
    /// <returns>float area</returns>
    public float Area() {
        float area = 0;

        int j = Vertices.Length - 1; // j starts as the last vertex
        for (int i = 0; i < Vertices.Length; i++) {
            area += (Vertices[j].x + Vertices[i].x)
                    * (Vertices[j].y - Vertices[i].y);
            j = i;  // j is the previous vertex to i
        }
        return area / 2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Vector2[] midPoints</returns>
    public Vector2[] Midpoints() {
        Vector2[] midPoints = new Vector2[Vertices.Length];

        int j = Vertices.Length - 1; // j starts as the last vertex
        for (int i = 0; i < Vertices.Length; i++) {
            midPoints[j] = (Vertices[j] + Vertices[i])/2;
            j = i;  // j is the previous vertex to i
        }
        return midPoints;
    }

    /// <summary>
    /// Gets a list of normals. They are in same order as controlpoints (clockwise). localCoordinates : true (for local coordinates, Sherlock)
    /// </summary>
    public Vector2[] GetWallNormals(bool localCoordinates = false) {
        Vector2[] normals = new Vector2[Vertices.Length];

        int j = Vertices.Length - 1; // j starts as the last vertex
        for (int i = 0; i < Vertices.Length; i++) {
            normals[j] = Vector2.Perpendicular(Vertices[j] - Vertices[i]);
            j = i;  // j is the previous vertex to i
        }
        return normals;
    }

    /// <summary>
    /// Finds the euclidean maximum and minimum corrdinates in both axes
    /// </summary>
    /// <returns>float[] bounds = {minX, maxX, minY, maxY}</returns>
    public float[] Bounds() {
        float minX = 0; float maxX = 0;
        float minY = 0; float maxY = 0;
        foreach (Vector2 point in Vertices) {
            if (point[0] < minX) { minX = point[0]; }
            if (point[0] > maxX) { maxX = point[0]; }
            if (point[1] < minY) { minY = point[1]; }
            if (point[1] > maxY) { maxY = point[1]; }
        }
        return new float[] { minX, maxX, minY, maxY };
    }
}
