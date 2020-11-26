using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Utils {

    /// <summary>
    /// Helper functions for polygon objects
    /// </summary>
    public static class PolygonUtils {

        /// <summary>
        /// Takes a list of vector3 control points from a given polygon
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns> Edge normals </returns>
        public static List<Vector3> PolygonNormals(List<Vector3> vertices) {
            List<Vector3> edgeNormals = new List<Vector3>();
            vertices = vertices.Concat(new List<Vector3> { vertices[0] }).ToList();
            for (int i = 0; i < vertices.Count - 1; i++) {
                edgeNormals.Add(Vector3.Cross(vertices[i + 1] - vertices[i], Vector3.up).normalized);
            }
            return edgeNormals;
        }
    }
}