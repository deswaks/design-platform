using DesignPlatform.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {

    public enum WallJointType {
        None,
        Corner_Primary,     // Element is extended to side of other element
        Corner_Secondary,   // Element is shortened
        T_Primary,          // Element going through (element has midpoint in T-joint)
        T_Secondary,        // Element ends onto primary element (endpoint in T-joint)
        X_Primary,          // Element going through (element has midpoint in X-joint)
        X_Secondary,        // Element ends onto primary element (endpoint in X-joint)
        Parallel
    }
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

    public static class CLTElementGenerator {

        /// <summary>
        /// Finds all wall elements from interfaces and identifies the joints between them. 
        /// </summary>
        /// <returns>List of WallElements with joints set.</returns>
        public static List<CLTElement> IdentifyWallElementsAndJointTypes() {
            // Gets full, un-split, wall elements with no joint information
            List<CLTElement> wallElements = JoinInterfacesToLongestWallElements();
            List<CLTElement> newWallElements = new List<CLTElement>();

            List<Vector3> distinctJointPoints = wallElements.SelectMany(w => w.AllPoints()).Distinct().ToList();

            foreach (Vector3 point in distinctJointPoints) {

                // Finds wall elements that have an endpoint in the given point and elements with midpoint(s) in the given point
                // In total, only two elements should always be found. 
                List<CLTElement> endJointWallElements = wallElements.Where(w => w.startPoint.point == point || w.endPoint.point == point).ToList();
                List<CLTElement> midJointWallElements = wallElements.Where(w => w.midpoints.Select(p => p.point).Contains(point)).ToList();

                // Finds vector of all walls joint in the given point

                // Cross-joint (two wallElement-midpoints) ////////////////// AS OF NOW, PRIMARY ELEMENT IS CHOSEN AS THE LONGEST ELEMENT
                if (midJointWallElements.Count == 2) {
                    CLTElement primaryElement = midJointWallElements.OrderBy(e => e.Length).Last();
                    CLTElement secondaryElement = midJointWallElements.OrderBy(e => e.Length).First();
                    ////////////////// AS OF NOW, PRIMARY ELEMENT IS CHOSEN AS THE LONGEST ELEMENT
                    if (primaryElement.midpoints.Select(p => p.point).Contains(point)) primaryElement.SetMidPointJointType(point, WallJointType.X_Primary);

                    // The secondary element is identified now and will be cut into two elements after all joints are identified
                    if (secondaryElement.midpoints.Select(p => p.point).Contains(point)) secondaryElement.SetMidPointJointType(point, WallJointType.X_Secondary);
                }
                // T-joint
                else if (midJointWallElements.Count == 1) {
                    midJointWallElements.First().SetMidPointJointType(midJointWallElements.First().midpoints.First(p => p.point == point).point, WallJointType.T_Primary);
                    if (point == endJointWallElements.First().startPoint.point) endJointWallElements.First().SetStartPointJointType(WallJointType.T_Secondary);
                    if (point == endJointWallElements.First().endPoint.point) endJointWallElements.First().SetEndPointJointType(WallJointType.T_Secondary);
                }
                // Corner joint ///////////////////////////////// AS OF NOW, PRIMARY ROLE IS ASSIGNED TO LONGEST ELEMENT
                else if (endJointWallElements.Count == 2) {
                    CLTElement primaryElement = endJointWallElements.OrderBy(e => e.Length).Last();
                    CLTElement secondaryElement = endJointWallElements.OrderBy(e => e.Length).First();

                    if (point == primaryElement.startPoint.point) primaryElement.SetStartPointJointType(WallJointType.Corner_Primary);
                    if (point == primaryElement.endPoint.point) primaryElement.SetEndPointJointType(WallJointType.Corner_Primary);                    
                    
                    if (point == secondaryElement.startPoint.point) secondaryElement.SetStartPointJointType(WallJointType.Corner_Secondary);
                    if (point == secondaryElement.endPoint.point) secondaryElement.SetEndPointJointType(WallJointType.Corner_Secondary);
                }

            }

            foreach (CLTElement wallElement in wallElements) {
                // If a wall is a secondary part of an X-joint, it will be split into two wall elements
                if (wallElement.midpoints.Select(p => p.jointType).Contains(WallJointType.X_Secondary)) {

                    CLTElement firstElement = new CLTElement();
                    CLTElement secondElement = new CLTElement();

                    // Index of midpoint where X-joint is
                    int index = wallElement.midpoints.Select(p => p.jointType).ToList().IndexOf(WallJointType.X_Secondary);

                    firstElement.SetStartPoint(wallElement.startPoint.point, wallElement.startPoint.jointType);
                    secondElement.SetStartPoint(wallElement.midpoints[index].point, wallElement.midpoints[index].jointType);

                    if (!(wallElement.midpoints.Count == 1)) {
                        secondElement.SetMidPoints(wallElement.midpoints.GetRange(index, wallElement.midpoints.Count - index));
                        firstElement.SetMidPoints(wallElement.midpoints.GetRange(1, index - 1));
                    };

                    firstElement.SetEndPoint(wallElement.midpoints[index].point, wallElement.midpoints[index].jointType);
                    secondElement.SetEndPoint(wallElement.endPoint.point, wallElement.endPoint.jointType);

                    // Adds two new elements to resulting list
                    newWallElements.Add(firstElement);
                    newWallElements.Add(secondElement);
                }
                else {
                    newWallElements.Add(wallElement);
                }
            }
            return newWallElements;
        }


        /// <summary>
        /// Identifies parallel joint walls and combines them (avoiding each wall being split by small, separate interfaces), providing vertex pairs for the resulting full wall elements. 
        /// </summary>
        /// <returns>Returns full, un-split, wall elements with no joint information.</returns>
        private static List<CLTElement> JoinInterfacesToLongestWallElements() {
            // Culls interfaces with same start- and endpoint
            List<Interface> culledInterfaces = Building.Instance.Interfaces.Where(i => i.EndPoint != i.StartPoint).ToList();

            // Identifier ID (Integer) for each wall referring to its wall element
            List<int> wallIDs = Enumerable.Repeat(-1, culledInterfaces.Count).ToList();

            // Finds all joint points (unique points shared by all interfaces)
            List<Vector3> jointPoints = new List<Vector3>();
            culledInterfaces.ForEach(i => {
                jointPoints.Add(i.EndPoint);
                jointPoints.Add(i.StartPoint);
            });
            jointPoints = jointPoints.Distinct().ToList();

            // Loops through all interfaces
            for (int j = 0; j < culledInterfaces.Count; j++) {
                // Finds parallel-joint interfaces of current interface
                List<Interface> parallelJointInterfaces = GetParallelConnectedInterfaces(culledInterfaces, culledInterfaces[j]);

                // Sees if one of parallel interfaces belongs to a wall and saves its ID
                int? currentID = parallelJointInterfaces.Select(i => wallIDs[culledInterfaces.IndexOf(i)]).Where(wg => wg != -1)?.FirstOrDefault();

                // If one interface already had an ID attached, all identified parallel walls gets this ID
                if (currentID != 0) {
                    parallelJointInterfaces.ForEach(i => wallIDs[culledInterfaces.IndexOf(i)] = currentID.Value);
                    wallIDs[j] = currentID.Value;
                }
                else // A new ID is created for the set of parallel walls
                {
                    currentID = wallIDs.Max() + 1;
                    parallelJointInterfaces.ForEach(i => wallIDs[culledInterfaces.IndexOf(i)] = currentID.Value);
                    wallIDs[j] = currentID.Value;
                }
            }

            // Each group consists of interfaces making up an entire wall
            IEnumerable<IGrouping<int, Interface>> wallGroups = culledInterfaces.GroupBy(i => wallIDs[culledInterfaces.IndexOf(i)]);

            // List for resulting wall vertices
            List<CLTElement> wallElements = new List<CLTElement>();

            // Loops through list of wall interfaces belong to each wall to find out wall end points and wall midpoints.
            foreach (List<Interface> wallInterfaces in wallGroups.Select(list => list.ToList()).ToList()) {
                CLTElement wallElement = new CLTElement();

                List<Vector3> currentWallVertices = wallInterfaces.SelectMany(i => new List<Vector3> { i.EndPoint, i.StartPoint }).Distinct().ToList();
                // X-values
                List<float> xs = currentWallVertices.Select(p => p.x).ToList();
                // Z-values
                List<float> zs = currentWallVertices.Select(p => p.z).ToList();
                // Endpoints are (Xmin,0,Zmin);(Xmax,0,Zmax) ////////////////// ONLY TRUE FOR WALLS LYING ALONG X-/Z-AXIS
                wallElement.SetStartPoint(new Vector3(xs.Min(), 0, zs.Min()));
                wallElement.SetEndPoint(new Vector3(xs.Max(), 0, zs.Max()));
                // Removes endpoints from vertex-list so that only midpoints remain
                currentWallVertices.RemoveAll(p => p == wallElement.startPoint.point || p == wallElement.endPoint.point);
                wallElement.SetMidPoints(currentWallVertices);

                wallElement.SetInterfaces(wallInterfaces);

                wallElements.Add(wallElement);

                if (wallElement.Length > 16.5) Debug.Log("Panel surpasses maximum length of 16.5m");
            }

            return wallElements;
        }

        public static List<Interface> GetParallelConnectedInterfaces(List<Interface> interfacesList, Interface interFace) {
            Vector3 interfaceDirection = (interFace.StartPoint - interFace.EndPoint).normalized;

            // Finds interfaces that have an endpoint in the given point
            List<Interface> jointInterfaces = new List<Interface>();
            jointInterfaces.AddRange(
                interfacesList
                .Where(i => i != interFace && (i.EndPoint == interFace.EndPoint || i.StartPoint == interFace.EndPoint))
                .Where(i => System.Math.Abs(Vector3.Dot((i.StartPoint - i.EndPoint).normalized, interfaceDirection)) == 1)
                .ToList());
            jointInterfaces.AddRange(
                interfacesList
                .Where(i => i != interFace && (i.EndPoint == interFace.StartPoint || i.StartPoint == interFace.StartPoint))
                .Where(i => System.Math.Abs(Vector3.Dot((i.StartPoint - i.EndPoint).normalized, interfaceDirection)) == 1)
                .ToList());

            return jointInterfaces;
        }

    }
}

