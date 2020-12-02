using DesignPlatform.Geometry;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DesignPlatform.Utils {

    /// <summary>
    /// Helper functions for vector objects
    /// </summary>
    public static class VectorUtils {

        /// <summary>
        /// Finds the index x(0), y(1) or z(2) of the largest component in the vector.
        /// </summary>
        /// <param name="v">Vector to find largest component in.</param>
        /// <returns>Index of component of the vector.</returns>
        public static int IndexLargestComponent(Vector3 v) {
            int indexLargestComponent = 0;
            for (int i = 0; i < 3; i++) {
                if (Mathf.Abs(v[i]) > Mathf.Abs(v[indexLargestComponent])) {
                    indexLargestComponent = i;
                }
            }
            return indexLargestComponent;
        }

        /// <summary>
        /// Rotates a vector 90 degrees clockwise on the XZ plane
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <returns>Rotated vector</returns>
        public static Vector3 Rotate90ClockwiseXZ(Vector3 vector) {
            return new Vector3(-vector.z, 0, vector.x);
        }

        /// <summary>
        /// Debug function to visualize the vector as a sphere in the unity editor.
        /// </summary>
        /// <param name="vertices">Vertices to visualize</param>
        public static void VisualizeAsSphere(Vector3 point) {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = point;
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            sphere.name = "vertex sphere";
        }

        /// <summary>
        /// Given a list of vertices, their collective plane is found and they are all rotated such that they end up on the XZ plane.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="midpoint"></param>
        /// <param name="rotationVector"></param>
        /// <returns></returns>
        public static List<Vector3> TransformPointsToXZ(List<Vector3> vertices, out Vector3 midpoint, out Vector3 rotationVector) {
            // Normal is calculated assuming all vertices is in the same plane
            Vector3 Normal = Vector3.Cross(vertices[2] - vertices[1], vertices[0] - vertices[1]).normalized;

            // Calculates midpoint from vertices
            Vector3 midpointTemp = RoofUtils.Midpoint(vertices);

            // Finds rotation vector that rotates points from current global position to xz-plane
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, Normal).normalized;
            float rotationAngle = Vector3.Angle(Vector3.up, Normal);
            rotationVector = (-rotationAxis * rotationAngle);

            // Moves vertices to be located around (0,0,0)
            vertices = vertices.Select(v => v - midpointTemp).ToList();

            // Rotates all points around (0,0,0) according to rotation vector
            List<Vector3> transformedPoints = new List<Vector3>();
            foreach (Vector3 v in vertices) {
                transformedPoints.Add(RoofUtils.RotatePointAroundPivot(v, new Vector3(0, 0, 0), rotationVector));
            }
            midpoint = midpointTemp;

            return transformedPoints;
        }

        /// <summary>
        /// Copies the vector
        /// </summary>
        /// <param name="originalVector">The vector to copy.</param>
        /// <returns>The copy of the vector.</returns>
        public static Vector3 Copy(Vector3 originalVector) {
            return new Vector3(originalVector.x, originalVector.y, originalVector.z);
        }

        /// <summary>
        /// Copies the vector
        /// </summary>
        /// <param name="originalVector">The vector to copy.</param>
        /// <returns>The copy of the vector.</returns>
        public static Vector2 Copy(Vector2 originalVector) {
            return new Vector3(originalVector.x, originalVector.y);
        }
    }
}