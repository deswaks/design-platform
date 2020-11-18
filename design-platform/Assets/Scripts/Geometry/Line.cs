using System.Collections;
using System.Collections.Generic;
using DesignPlatform.Core;
using UnityEngine;

namespace DesignPlatform.Geometry {
    public class Line {

        public Vector3 StartPoint {get; set;}
        public Vector3 EndPoint { get; set; }

        public float Length {
            get { return (EndPoint - StartPoint).magnitude; }
        }

        public Vector3 Midpoint {
            get { return (EndPoint + StartPoint)/2; }
        }

        public Line(Vector3 start, Vector3 end) {
            StartPoint = start;
            EndPoint = end;
        }
        public Line(Vector3[] endPoints) {
            StartPoint = endPoints[0];
            EndPoint = endPoints[1];
        }
        public Line(List<Vector3> endPoints) {
            StartPoint = endPoints[0];
            EndPoint = endPoints[1];
        }
        public Line((Vector3,Vector3) endPoints) {
            StartPoint = endPoints.Item1;
            EndPoint = endPoints.Item2;
        }

        public Line(Face face) {
            StartPoint = face.StartPoint;
            EndPoint = face.EndPoint;
        }

        /// <summary>
        /// Checks whether a point is located on the line
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsOnLine(Vector3 point) {
            if (Vector3.Distance(StartPoint, point) < 0.01) return false;
            if (Vector3.Distance(EndPoint, point) < 0.01) return false;
            return (Vector3.Distance(StartPoint, point)
                + Vector3.Distance(EndPoint, point)
                - Vector3.Distance(StartPoint, EndPoint) < 0.001);
        }

        /// <summary>
        /// Finds the parameter (from 0.0 to 1.0) for the point on the line
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float Parameter(Vector3 point) {
            return (point - StartPoint).magnitude / Length;
        }

    }
}