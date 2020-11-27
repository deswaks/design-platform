using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {


    public partial class Building {

        /// <summary>
        /// Builds CLT elements for all vertical interfaces of the whole building.
        /// </summary>
        /// <returns>All walls of the building.</returns>
        public List<Wall> BuildAllWallsAsCLTElements() {
            foreach (Opening opening in Openings) {
                opening.AttachClosestFaces();
            }
            Building.IdentifyWallElementsAndJointTypes().ForEach(clt => BuildWall(clt));
            return Walls;
        }

        /// <summary>
        /// Build a 3D wall representation.
        /// </summary>
        /// <param name="cltElement">CLT element to base the wall upon</param>
        /// <returns>The newly built wall.</returns>
        public Wall BuildWall(CLTElement cltElement) {
            GameObject newWallGameObject = new GameObject("Wall");
            Wall newWall = (Wall)newWallGameObject.AddComponent(typeof(Wall));

            newWall.InitializeWall(cltElement);

            walls.Add(newWall);

            return newWall;
        }

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
        /// Groups all interfaces that are parallel.
        /// </summary>
        /// <param name="interfaces"></param>
        /// <returns></returns>
        public static List<int> GroupParallelJoinedInterfaces(List<Interface> interfaces) {

            // Identifier ID (Integer) for each wall referring to its wall element
            List<int> wallIDs = Enumerable.Repeat(-1, interfaces.Count).ToList();

            // Finds all joint points (unique points shared by all interfaces)
            List<Vector3> jointPoints = new List<Vector3>();
            interfaces.ForEach(i => {
                jointPoints.Add(i.EndPoint);
                jointPoints.Add(i.StartPoint);
            });
            jointPoints = jointPoints.Distinct().ToList();

            // Loops through all interfaces
            for (int j = 0; j < interfaces.Count; j++) {
                // Finds parallel-joint interfaces of current interface
                List<Interface> parallelJointInterfaces = GetParallelConnectedInterfaces(interfaces, interfaces[j]);

                // Sees if one of parallel interfaces belongs to a wall and saves its ID
                int? currentID = parallelJointInterfaces.Select(i => wallIDs[interfaces.IndexOf(i)]).Where(wg => wg != -1)?.FirstOrDefault();

                // If one interface already had an ID attached, all identified parallel walls gets this ID
                if (currentID != 0) {
                    parallelJointInterfaces.ForEach(i => wallIDs[interfaces.IndexOf(i)] = currentID.Value);
                    wallIDs[j] = currentID.Value;
                }
                else // A new ID is created for the set of parallel walls
                {
                    currentID = wallIDs.Max() + 1;
                    parallelJointInterfaces.ForEach(i => wallIDs[interfaces.IndexOf(i)] = currentID.Value);
                    wallIDs[j] = currentID.Value;
                }
            }

            return wallIDs;
        }

        /// <summary>
        /// Identifies parallel joint walls and combines them (avoiding each wall being split by small, separate interfaces), providing vertex pairs for the resulting full wall elements. 
        /// </summary>
        /// <returns>Returns full, un-split, wall elements with no joint information.</returns>
        private static List<CLTElement> JoinInterfacesToLongestWallElements() {
            // Culls interfaces with same start- and endpoint
            List<Interface> culledInterfaces = Building.Instance.Interfaces.Where(i => i.EndPoint != i.StartPoint).ToList();

            List<int> wallIDs = GroupParallelJoinedInterfaces(culledInterfaces);

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

        /// <summary>
        /// Finds all the interfaces that are parallel
        /// </summary>
        /// <param name="interfacesList"></param>
        /// <param name="interFace"></param>
        /// <returns></returns>
        private static List<Interface> GetParallelConnectedInterfaces(List<Interface> interfacesList, Interface interFace) {
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