﻿using System.Collections;
using System.Collections.Generic;
using DesignPlatform.Core;
using UnityEngine;

namespace DesignPlatform.Geometry {

    /// <summary>
    /// Three dimensional line segment.
    /// </summary>
    public class Line {

        /// <summary>Start point of the line.</summary>
        public Vector3 StartPoint;
        /// <summary>End point of the line.</summary>
        public Vector3 EndPoint;



        /// <summary>
        /// Constructs a line between the two points.
        /// </summary>
        /// <param name="start">Start point of the line.</param>
        /// <param name="end">End point of the line.</param>
        public Line(Vector3 start, Vector3 end) {
            StartPoint = start;
            EndPoint = end;
        }



        /// <summary>Length of the line and the distance from start point to end point.</summary>
        public float Length {
            get { return (EndPoint - StartPoint).magnitude; }
        }

        /// <summary>Midpoint of the line and the average of the start point and the end point.</summary>
        public Vector3 Midpoint {
            get { return (EndPoint + StartPoint) / 2; }
        }

        /// <summary>The direction of this line from the startpoint to the endpoint.</summary>
        public Vector3 Direction {
            get { return (StartPoint - EndPoint).normalized; }
        }



        /// <summary>
        /// Checks whether a point is located on the line.
        /// </summary>
        /// <param name="point">The point to check whether lies on the line.</param>
        /// <returns>True if the point is on the line and false of the point does not.</returns>
        public bool Intersects(Vector3 point) {
            if (Vector3.Distance(StartPoint, point) < Settings.nearThreshold) return false;
            if (Vector3.Distance(EndPoint, point) < Settings.nearThreshold) return false;
            return (Vector3.Distance(StartPoint, point)
                + Vector3.Distance(EndPoint, point)
                - Vector3.Distance(StartPoint, EndPoint) < Settings.nearThreshold);
        }

        /// <summary>
        /// Finds the parameter (from 0.0 to 1.0) for the point on the line
        /// </summary>
        /// <param name="point">The point to find the parameter for.</param>
        /// <returns>Parameter of the point on the line.</returns>
        public float ParameterAtPoint(Vector3 point) {
            return (point - StartPoint).magnitude / Length;
        }

        /// <summary>
        /// Finds the point on the line at the given parameter (from 0.0 to 1.0)
        /// </summary>
        /// <param name="parameter">The parameter for which to get the point.</param>
        /// <returns>The point on the line at the given parameter.</returns>
        public Vector3 PointAtParameter(float parameter) {
            return StartPoint + Direction * Length * parameter;
        }

        /// <summary>
        /// Finds the point on this line, which lies closest to the given point.
        /// </summary>
        /// <param name="point">Point to find closest point from.</param>
        /// <returns>The closest point on this line.</returns>
        public Vector3 ClosestPoint(Vector3 point) {
            var v1 = point - StartPoint;
            var v2 = (EndPoint - StartPoint).normalized;

            var d = Vector3.Distance(StartPoint, EndPoint);
            var t = Vector3.Dot(v2, v1);

            if (t <= 0) return StartPoint;
            if (t >= d) return EndPoint;

            var vVector3 = v2 * t;
            var vClosestPoint = StartPoint + vVector3;

            return vClosestPoint;
        }

        public bool IsConnectedTo(Line otherLine) {
            return Vector3.Distance(StartPoint, otherLine.StartPoint) < Settings.nearThreshold
                || Vector3.Distance(StartPoint, otherLine.EndPoint) < Settings.nearThreshold
                || Vector3.Distance(EndPoint, otherLine.StartPoint) < Settings.nearThreshold
                || Vector3.Distance(EndPoint, otherLine.EndPoint) < Settings.nearThreshold;
        }

        public bool IsParallelTo(Line otherLine) {
            float absDot = Mathf.Abs(Vector3.Dot(Direction, otherLine.Direction));
            return (Mathf.Abs(absDot - 1.0f) < Settings.nearThreshold);
        }

    }
}