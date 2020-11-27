using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Geometry {

    /// <summary>
    /// Three dimensional point.
    /// </summary>
    public partial class Point {

        /// <summary>
        /// Constructs a line between the two points.
        /// </summary>
        /// <param name="start">Start point of the line.</param>
        /// <param name="end">End point of the line.</param>
        public Point(float x, float y, float z) {
            this.x = x; this.y = y; this.z = z;
        }

        /// <summary>X coordinate</summary>
        public float x { get; private set; } = 0.0f;
        
        /// <summary>Y coordinate</summary>
        public float y { get; private set; } = 0.0f;

        /// <summary>Z coordinate</summary>
        public float z { get; private set; } = 0.0f;

        /// <summary>This point represented as a three dimensional vector.</summary>
        public Vector3 Vector {
            get { return new Vector3(x, y, z); }
        }

        /// <summary>
        /// Checks whether the point is located on the line.
        /// </summary>
        /// <param name="line">The line to check whether this point lies on.</param>
        /// <returns>True if the point is on the line and false of the point is not.</returns>
        public bool IsOn(Line line) {
            if (Vector3.Distance(line.StartPoint, Vector) < 0.01) return false;
            if (Vector3.Distance(line.EndPoint, Vector) < 0.01) return false;
            return Vector3.Distance(line.StartPoint, Vector)
                + Vector3.Distance(line.EndPoint, Vector)
                - Vector3.Distance(line.StartPoint, Vector) < 0.001;
        }
    }
}