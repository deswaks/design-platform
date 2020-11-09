using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Core {

    public enum WallJointType {
        None,
        Corner,     // Corner joint
        T_top,      // Element going through (element has midpoint in T-joint)
        T_bottom,   // Element ends onto primary element (endpoint in T-joint)
        X_Primary,  // Element going through (element has midpoint in X-joint)
        X_Secondary, // Element ends onto primary element (endpoint in X-joint)
        Parallel
    }

    public class WallElement {
        public (Vector3 point, WallJointType jointType) startPoint { get; private set; } = (new Vector3(), WallJointType.None);
        public (Vector3 point, WallJointType jointType) endPoint { get; private set; }   = (new Vector3(), WallJointType.None );
        public List<(Vector3 point, WallJointType jointType)> midpoints { get; private set; } = new List<(Vector3 point, WallJointType jointType)>();


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
            midpoints[index] = (midpoints[index].point, jointType);
        }
        public void SetMidpoints(List<Vector3> points, List<WallJointType> jointTypes) {
            if(points.Count != jointTypes.Count) {
                Debug.LogError("Point- and jointtype lists not of equal size!");
                return;
            }

            for(int i = 0; i<points.Count; i++) {
                midpoints.Add((points[i], jointTypes[i]));
            }
        }
        public void SetMidPoints(List<(Vector3 point, WallJointType jointType)> midpoints) {
            this.midpoints = midpoints;
        }
        public void SetMidPoints(List<Vector3> points) {
            points.ForEach(p => midpoints.Add((p, WallJointType.None)));
        } 
        public Vector3 GetDirection() {
            return (startPoint.point - endPoint.point).normalized;
        }
        public List<Vector3> AllPoints() {
            List<Vector3> points = new List<Vector3>();
            points.Add(startPoint.point);
            points.AddRange(midpoints.Select(p => p.point));
            points.Add(endPoint.point);
            return points;
        }
    }

    public partial class Building {

        public void IdentifyWallElementJointTypes() {
            List<WallElement> wallElements = FindWallElements();

            List<Vector3> distinctJointPoints = wallElements.SelectMany(w => new List<Vector3> { w.startPoint.point, w.endPoint.point }).Distinct().ToList();

            foreach (Vector3 point in distinctJointPoints) {
                // Finds wall elements that have an endpoint in the given point
                List<WallElement> jointWallElements = wallElements.Where(w => w.AllPoints().Contains(point)).ToList();

                // Finds vector of all walls joint in the given point
                List<Vector3> wallVectors = jointWallElements.Select(w => w.GetDirection()).ToList();

                string jointType = ":(";

                switch (jointWallElements.Count) {

                    case 2:
                        float dot = Vector3.Dot(wallVectors[0], wallVectors[1]);

                        if (Mathf.Round(dot) == 1 || Mathf.Round(dot) == -1) // Parallel
                        {
                            if (jointWallElements[0].startPoint.point == point) {
                                jointWallElements[0].SetStartPointJointType(WallJointType.Parallel);
                            }                            
                            if (jointWallElements[1].startPoint.point == point) {
                                jointWallElements[1].SetStartPointJointType(WallJointType.Parallel);
                            }
                            if (jointWallElements[0].endPoint.point == point) {
                                jointWallElements[0].SetEndPointJointType(WallJointType.Parallel);
                            }                            
                            if (jointWallElements[1].endPoint.point == point) {
                                jointWallElements[1].SetEndPointJointType(WallJointType.Parallel);
                            }
                        }

                        else if (Mathf.Round(dot) == 0) // Perpendicular
                        {
                            // T-joint:
                            if (jointWallElements[0].startPoint.point == point && jointWallElements[1].midpoints.Select(p=>p.point).Contains(point)) {
                                jointWallElements[0].SetStartPointJointType(WallJointType.Parallel);
                            }



                        }
                        break;

                    case 3:
                        jointType = "T";
                        break;

                    case 4:
                        jointType = "+";
                        break;
                }
            }
        }

        /// <summary>
        /// Identifies the type of joint at all intersections (points) between the interfaces of the building
        /// </summary>
        public void IdentifyInterfaceJointTypes() {
            // Culls interfaces with same start- and endpoint
            List<Interface> culledInterfaces = interfaces.Where(i => i.GetEndPoint() != i.GetStartPoint()).ToList();

            // Finds all joint points (unique points shared by all interfaces)
            List<Vector3> jointPoints = new List<Vector3>();
            culledInterfaces.ForEach(i => {
                jointPoints.Add(i.GetEndPoint());
                jointPoints.Add(i.GetStartPoint());
            });

            jointPoints = jointPoints.Distinct().ToList();

            UnityEngine.Object prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/joint2.prefab", typeof(GameObject)); // MIDLERTIDIG

            foreach (Vector3 point in jointPoints) {
                // Finds interfaces that have an endpoint in the given point
                List<Interface> jointInterfaces = culledInterfaces.Where(i => i.GetEndPoint() == point || i.GetStartPoint() == point).ToList();

                // Finds vector of all interfaces joint in the given point
                List<Vector3> wallVectors = jointInterfaces.Select(interFace => (interFace.GetStartPoint() - interFace.GetEndPoint()).normalized).ToList();

                string jointType = ":(";

                switch (wallVectors.Count) {
                    case 2:
                        float dot = Vector3.Dot(wallVectors[0], wallVectors[1]);
                        if (Mathf.Round(dot) == 1 || Mathf.Round(dot) == -1) // Parallel
                        {
                            jointType = "||";
                        }
                        else if (Mathf.Round(dot) == 0) // Perpendicular
                        {
                            jointType = "L";
                        }
                        break;

                    case 3:
                        jointType = "T";
                        break;

                    case 4:
                        jointType = "+";
                        break;
                }

                GameObject note = (GameObject)GameObject.Instantiate(prefab);//, parent.transform);
                note.transform.position = point;
                note.GetComponent<TMPro.TMP_Text>().text = jointType;
            }
        }

        /// <summary>
        /// Identifies parallel joint walls and combines them (avoiding each wall being split by small, separate interfaces), providing vertex pairs for the resulting full wall elements. 
        /// </summary>
        public List<WallElement> FindWallElements() {
            // Culls interfaces with same start- and endpoint
            List<Interface> culledInterfaces = interfaces.Where(i => i.GetEndPoint() != i.GetStartPoint()).ToList();

            // Identifier ID (Integer) for each wall referring to its wall element
            List<int> wallIDs = Enumerable.Repeat(-1, culledInterfaces.Count).ToList();

            // Finds all joint points (unique points shared by all interfaces)
            List<Vector3> jointPoints = new List<Vector3>();
            culledInterfaces.ForEach(i => {
                jointPoints.Add(i.GetEndPoint());
                jointPoints.Add(i.GetStartPoint());
            });
            jointPoints = jointPoints.Distinct().ToList();

            UnityEngine.Object prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/joint2.prefab", typeof(GameObject)); // MIDLERTIDIG

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
            List<WallElement> wallElements = new List<WallElement>();

            // Loops through list of wall interfaces belong to each wall to find out wall end points and wall midpoints.
            foreach (List<Interface> wallInterfaces in wallGroups.Select(list => list.ToList()).ToList()) {
                WallElement wallElement = new WallElement();

                List<Vector3> currentWallVertices = wallInterfaces.SelectMany(i => new List<Vector3> { i.GetEndPoint(), i.GetStartPoint() }).Distinct().ToList();
                // X-values
                List<float> xs = currentWallVertices.Select(p => p.x).ToList();
                // Z-values
                List<float> zs = currentWallVertices.Select(p => p.z).ToList();
                // Endpoints are (Xmin,0,Zmin);(Xmax,0,Zmax) ////////////////// ONLY TRUE FOR WALLS LYING ALONG X-/Z-AXIS
                wallElement.SetStartPoint(new Vector3(xs.Min(), 0, zs.Min()));
                wallElement.SetEndPoint(  new Vector3(xs.Max(), 0, zs.Max()));
                // Removes endpoints from vertex-list so that only midpoints remain
                currentWallVertices.RemoveAll(p => p == wallElement.startPoint.point || p == wallElement.endPoint.point);
                wallElement.SetMidPoints(currentWallVertices);

                wallElements.Add(wallElement);
            }

            return wallElements;
        }

        public List<Interface> GetParallelConnectedInterfaces(List<Interface> interfacesList, Interface interFace) {
            Vector3 interfaceDirection = (interFace.GetStartPoint() - interFace.GetEndPoint()).normalized;

            // Finds interfaces that have an endpoint in the given point
            List<Interface> jointInterfaces = new List<Interface>();
            jointInterfaces.AddRange(
                interfacesList
                .Where(i => i != interFace && (i.GetEndPoint() == interFace.GetEndPoint() || i.GetStartPoint() == interFace.GetEndPoint()))
                .Where(i => System.Math.Abs(Vector3.Dot((i.GetStartPoint() - i.GetEndPoint()).normalized, interfaceDirection)) == 1)
                .ToList());
            jointInterfaces.AddRange(
                interfacesList
                .Where(i => i != interFace && (i.GetEndPoint() == interFace.GetStartPoint() || i.GetStartPoint() == interFace.GetStartPoint()))
                .Where(i => System.Math.Abs(Vector3.Dot((i.GetStartPoint() - i.GetEndPoint()).normalized, interfaceDirection)) == 1)
                .ToList());

            return jointInterfaces;
        }

    }
}

