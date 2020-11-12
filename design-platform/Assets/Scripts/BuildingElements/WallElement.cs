using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

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
            if(index == -1) {
                Debug.LogError("Trying to set midpoint joint of point not found among midpoints");
                return;
            }
            midpoints[index] = (midpoints[index].point, jointType);
        }
        public void SetMidPoints(List<Vector3> points, List<WallJointType> jointTypes) {
            if(points.Count != jointTypes.Count) {
                Debug.LogError("Point- and jointtype lists not of equal size!");
                return;
            }

            for(int i = 0; i<points.Count; i++) {
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

            midpoints = midpoints.OrderBy(p => Vector3.Distance(p.point, startPoint.point) ).ToList();
        } 

        public List<Vector3> AllPoints() {
            List<Vector3> points = new List<Vector3>();
            points.Add(startPoint.point);
            points.AddRange(midpoints.Select(p => p.point));
            points.Add(endPoint.point);
            return points;
        }
        public double Length() {
            return Vector3.Distance(startPoint.point, endPoint.point);
        }
        public Vector3 GetDirection() {
            return (startPoint.point - endPoint.point).normalized;
        }
    }

    public partial class Building {
        /// <summary>
        /// Finds all wall elements from interfaces and identifies the joints between them. 
        /// </summary>
        /// <returns>List of WallElements with joints set.</returns>
        public List<WallElement> IdentifyWallElementsAndJointTypes() {

            List<WallElement> wallElements = JoinInterfacesToLongestWallElements();
            List<WallElement> newWallElements = new List<WallElement>();


            List<Vector3> allPoints = wallElements.SelectMany(w => w.AllPoints()).ToList();
            List<Vector3> distinctJointPoints = wallElements.SelectMany(w => w.AllPoints()).Distinct().ToList();

            Debug.Log("All points: "+allPoints.Count);


            foreach (Vector3 point in distinctJointPoints) {

                Debug.Log(allPoints.Count(p => p == point));

                // Finds wall elements that have an endpoint in the given point and elements with midpoint(s) in the given point
                // It SHOULD always find only two elements
                List<WallElement> endJointWallElements = wallElements.Where( w => w.startPoint.point == point || w.endPoint.point == point ).ToList();
                List<WallElement> midJointWallElements = wallElements.Where(w => w.midpoints.Select(p=>p.point).Contains(point)).ToList();

                // Finds vector of all walls joint in the given point
                
                // Cross-joint (two wallElement-midpoints) ////////////////// AS OF NOW, PRIMARY ELEMENT IS CHOSEN AS THE LONGEST ELEMENT
                if(midJointWallElements.Count == 2) {
                    Debug.Log("X-joint found!");
                    WallElement primaryElement   = midJointWallElements.OrderBy(e => e.Length()).Last();
                    WallElement secondaryElement = midJointWallElements.OrderBy(e => e.Length()).First();

                    Debug.Log("Longest element: " + primaryElement.Length());
                    Debug.Log("Shortest element: " + secondaryElement.Length());

                    if (primaryElement.midpoints.Select(p=>p.point).Contains(point)) primaryElement.SetMidPointJointType(point, WallJointType.X_Primary);

                    // The secondary element will be cut into two elements after all joints are identified
                    if (secondaryElement.midpoints.Select(p => p.point).Contains(point)) secondaryElement.SetMidPointJointType(point, WallJointType.X_Secondary);
                }
                // T-joint
                else if(midJointWallElements.Count == 1) {
                    midJointWallElements.First().SetMidPointJointType( midJointWallElements.First().midpoints.First(p => p.point == point ).point , WallJointType.T_Primary);
                    if (point == endJointWallElements.First().startPoint.point) endJointWallElements.First().SetStartPointJointType(WallJointType.T_Secondary);
                    if (point == endJointWallElements.First().endPoint.point) endJointWallElements.First().SetEndPointJointType(WallJointType.T_Secondary);
                }
                // Corner joint ///////////////////////////////// AS OF NOW, PRIMARY/SECONDARY ROLE IS ASSIGNED ~RANDOMLY
                else if (endJointWallElements.Count == 2) {
                    if (point == endJointWallElements.First().startPoint.point) endJointWallElements.First().SetStartPointJointType(WallJointType.Corner_Primary);
                    if (point == endJointWallElements.First().endPoint.point)   endJointWallElements.First().SetEndPointJointType(WallJointType.Corner_Primary);                    
                    
                    if (point == endJointWallElements.Last().startPoint.point) endJointWallElements.Last().SetStartPointJointType(WallJointType.Corner_Secondary);
                    if (point == endJointWallElements.Last().endPoint.point)   endJointWallElements.Last().SetEndPointJointType(WallJointType.Corner_Secondary);
                }

            }

            foreach(WallElement wallElement in wallElements) {
                if (wallElement.midpoints.Select(p => p.jointType).Contains(WallJointType.X_Secondary)){

                    WallElement firstElement  = new WallElement();
                    WallElement secondElement = new WallElement();

                    int index = wallElement.midpoints.Select(p => p.jointType).ToList().IndexOf(WallJointType.X_Secondary);
                    
                    firstElement.SetStartPoint(wallElement.startPoint.point,wallElement.startPoint.jointType); 
                    secondElement.SetStartPoint(wallElement.midpoints[index].point, wallElement.midpoints[index].jointType);

                    if (! (wallElement.midpoints.Count == 1)) {
                        secondElement.SetMidPoints(wallElement.midpoints.GetRange(index, wallElement.midpoints.Count - index));
                        firstElement.SetMidPoints(wallElement.midpoints.GetRange(1, index - 1));
                    };

                    firstElement.SetEndPoint(wallElement.midpoints[index].point, wallElement.midpoints[index].jointType);
                    secondElement.SetEndPoint(wallElement.endPoint.point, wallElement.endPoint.jointType);

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
        private List<WallElement> JoinInterfacesToLongestWallElements() {
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

