using DesignPlatform.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {

    /// <summary>
    /// The CLT element represents a structural panel made of CLT (cross laminated timber)
    /// </summary>
    public class CLTElement {
        public (Vector3 point, WallJointType jointType) startPoint { get; private set; } = (new Vector3(), WallJointType.None);
        public (Vector3 point, WallJointType jointType) endPoint { get; private set; } = (new Vector3(), WallJointType.None);
        public List<(Vector3 point, WallJointType jointType)> midpoints { get; private set; } = new List<(Vector3 point, WallJointType jointType)>();

        public List<Interface> interfaces { get; private set; } = new List<Interface>();
        public float Length { get {
                return Vector3.Distance(startPoint.point, endPoint.point);
            }
        }
        
        public float Height { get; private set; } = 3.0f; // SKAL SUGES FRA NOGET
        public float Thickness { get; private set; } = 0.2f; // SKAL SUGES
        public string Quality { get; private set; } = "DVQ"; //Domestic visual (DVQ), Industrial (IVQ), Non visual (NVQ)
        public double Area { 
            get { return Length * Height; } 
            private set { Area = value; } 
        }
        public List<Opening> Openings {
            get { return interfaces.SelectMany(ifc => ifc.Openings).ToList(); }
        }

        public void SetStartPoint(Vector3 point, WallJointType jointType = WallJointType.None) {
            startPoint = (point, jointType);
        }
        public void SetEndPoint(Vector3 point, WallJointType jointType = WallJointType.None) {
            endPoint = (point, jointType);
        }
        public void SetStartPointJointType(WallJointType jointType) {
            startPoint = (startPoint.point, jointType);
        }
        public void SetEndPointJointType(WallJointType jointType) {
            endPoint = (endPoint.point, jointType);
        }
        public void SetMidPointJointType(Vector3 point, WallJointType jointType) {
            int index = midpoints.Select(p => p.point).ToList().IndexOf(point);
            if (index == -1) {
                Debug.LogError("Trying to set midpoint joint of point not found among midpoints");
                return;
            }
            midpoints[index] = (midpoints[index].point, jointType);
        }
        public void SetMidPoints(List<Vector3> points, List<WallJointType> jointTypes) {
            if (points.Count != jointTypes.Count) {
                Debug.LogError("Point- and jointtype lists not of equal size!");
                return;
            }

            for (int i = 0; i < points.Count; i++) {
                midpoints.Add((points[i], jointTypes[i]));
            }
            midpoints = midpoints.OrderBy(p => Vector3.Distance(p.point, startPoint.point)).ToList();
        }
        public void SetMidPoints(List<(Vector3 point, WallJointType jointType)> midpoints) {
            this.midpoints = midpoints;
            midpoints = midpoints.OrderBy(p => Vector3.Distance(p.point, startPoint.point)).ToList();
        }
        public void SetMidPoints(List<Vector3> points) {
            midpoints = new List<(Vector3 point, WallJointType jointType)>();
            points.ForEach(p => midpoints.Add((p, WallJointType.None)));

            midpoints = midpoints.OrderBy(p => Vector3.Distance(p.point, startPoint.point)).ToList();
        }

        public List<Vector3> AllPoints() {
            List<Vector3> points = new List<Vector3>();
            points.Add(startPoint.point);
            points.AddRange(midpoints.Select(p => p.point));
            points.Add(endPoint.point);
            return points;
        }

        public Vector3 GetDirection() {
            return (startPoint.point - endPoint.point).normalized;
        }
        public Vector3 CenterPoint { get{ 
                return startPoint.point + (endPoint.point-startPoint.point)/2; 
            }
        }
        public void SetInterfaces(List<Interface> interfaces) {
            this.interfaces = interfaces;
        }
    }
}

