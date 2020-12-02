using UnityEngine;
using DesignPlatform.Utils;
using System.Linq;
using System.Collections.Generic;

namespace DesignPlatform.Geometry {

    /// <summary>
    /// Two dimensional polygon represented by a closed polyline.
    /// </summary>
    public class Polygon2D {

        /// <summary>The vertices of the underlying polyline</summary>
        public List<Vector2> Vertices { get; private set; }



        /// <summary>
        /// Create a 2D Polygon
        /// </summary>
        /// <param name="vertices">The vertices of the polygon (X,Y)</param>
        public Polygon2D(List<Vector2> vertices) {
            Vertices = vertices;
        }

        /// <summary>
        /// Create a 2D Polygon
        /// </summary>
        /// <param name="vertices">The vertices of the polygon (X,Y)</param>
        public Polygon2D(Vector2[] vertices) {
            Vertices = vertices.ToList();
        }

        /// <summary>
        /// Create a 2D Polygon
        /// </summary>
        /// <param name="vertices">The vertices of the polygon (X,Y)</param>
        public Polygon2D(List<Vector3> vertices) {
            Vertices = vertices.Select(v => new Vector2(v.x, v.z)).ToList();
        }



        /// <summary>
        /// The euclidean maximum and minimum corrdinates in both axes
        /// </summary>
        /// <returns>float[] bounds = {minX, maxX, minY, maxY}</returns>
        public List<float> Bounds {
            get {
                float minX = 0; float maxX = 0;
                float minY = 0; float maxY = 0;
                foreach (Vector2 point in Vertices) {
                    if (point[0] < minX) { minX = point[0]; }
                    if (point[0] > maxX) { maxX = point[0]; }
                    if (point[1] < minY) { minY = point[1]; }
                    if (point[1] > maxY) { maxY = point[1]; }
                }
                return new List<float> { minX, maxX, minY, maxY };
            }
        }

        /// <summary>
        /// The two dimensional area of the polygon.
        /// </summary>
        /// <returns>float area</returns>
        public float Area {
            get {
                float area = 0;

                int j = Vertices.Count - 1; // j starts as the last vertex
                for (int i = 0; i < Vertices.Count; i++) {
                    area += (Vertices[j].x + Vertices[i].x)
                            * (Vertices[j].y - Vertices[i].y);
                    j = i;  // j is the previous vertex to i
                }
                return area / 2;
            }
        }

        /// <summary>
        /// The list of midpoints for all the edges of the polyline of the polygon.
        /// </summary>
        /// <returns>List of edge mid points</returns>
        public Vector2[] EdgeMidpoints {
            get {
                Vector2[] midPoints = new Vector2[Vertices.Count];

                int j = Vertices.Count - 1; // j starts as the last vertex
                for (int i = 0; i < Vertices.Count; i++) {
                    midPoints[j] = (Vertices[j] + Vertices[i]) / 2;
                    j = i;  // j is the previous vertex to i
                }
                return midPoints;
            }
        }

        /// <summary>The center point of the polygon expressed as the average of all its vertices equally weighted.</summary>
        public Vector2 Center {
            get {
                Vector2 midpoint = new Vector2();
                Vertices.ForEach(v => midpoint += v);
                return midpoint /= Vertices.Count;
            }
        }

        /// <summary>
        /// The normal direction for each of the line segments in the polygon
        /// </summary>
        public List<Vector2> Normals {
            get {
                Vector2[] normals = new Vector2[Vertices.Count];

                int j = Vertices.Count - 1; // j starts as the last vertex
                for (int i = 0; i < Vertices.Count; i++) {
                    Vector2 direction = (Vertices[i] - Vertices[j]).normalized;
                    normals[j] =  new Vector2(-direction.y, direction.x);
                    j = i;  // j is the previous vertex to i
                }

                return normals.ToList();
            }
        }

        /// <summary>
        /// Offset the polygon.
        /// Positive offset will increate its size and negative offset will decreate its size
        /// </summary>
        /// <param name="offset">Distance to offset the polygon.</param>
        public void Offset(float offset) {
            List<Vector2> newVertices = Vertices.Select(v => VectorUtils.Copy(v)).ToList();

            int j = Vertices.Count - 1; // j starts as the last vertex
            for (int i = 0; i < Vertices.Count; i++) {
                Vector2 direction = (Vertices[i] - Vertices[j]).normalized;
                Vector2 normal = new Vector2(-direction.y, direction.x);
                newVertices[j] += normal * offset;
                newVertices[i] += normal * offset;
                j = i;  // j is the previous vertex to i
            }

            Vertices = newVertices;
        }


    }
}