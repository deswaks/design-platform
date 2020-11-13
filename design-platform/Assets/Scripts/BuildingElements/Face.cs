using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DesignPlatform.Core {
    public class Face {
        public Room parentRoom { get; private set; }
        public int faceIndex { get; private set; }
        public List<Interface> interfaces { get; private set; }
        public List<Opening> openings { get; private set; }
        public Dictionary<Interface, float[]> paramerters { get; private set; }
        public Orientation orientation { get; private set; }

        public float wallThickness = 0.2f;

        /// <summary>
        /// 
        /// </summary>
        public Face(Room parent, int index) {
            interfaces = new List<Interface>();
            openings = new List<Opening>();
            paramerters = new Dictionary<Interface, float[]>();
            parentRoom = parent;
            faceIndex = index;
            SetOrientation();
        }

        public override string ToString() {
            return parentRoom.ToString() + " Face#" + faceIndex.ToString();
        }

        /// <summary>
        /// Determs the orientation of the face (horizontal or vertical)
        /// </summary>
        private void SetOrientation() {
            if (faceIndex < parentRoom.GetControlPoints().Count) {
                orientation = Orientation.VERTICAL;
            }
            else orientation = Orientation.HORIZONTAL;
        }

        /// <summary>
        /// Gets the original controlpoints of the face (horizontal and vertical faces)
        /// </summary>
        public List<Vector3> GetControlPoints(bool localCoordinates = false) {
            List<Vector3> roomControlPoints = parentRoom.GetControlPoints(localCoordinates: localCoordinates, closed: true);
            List<Vector3> controlPoints = new List<Vector3>();

            switch (orientation) {
                // Vertical face
                case Orientation.VERTICAL:
                    controlPoints.Add(roomControlPoints[faceIndex]);
                    controlPoints.Add(roomControlPoints[faceIndex + 1]);
                    break;
                
                // Horizontal face
                case Orientation.HORIZONTAL:
                    controlPoints = roomControlPoints.GetRange(0, roomControlPoints.Count - 1);
                    
                    // Top face
                    if (faceIndex == roomControlPoints.Count) {
                        controlPoints = controlPoints.Select(p => p + Vector3.up * parentRoom.height).ToList();
                    }
                    break;
            }

            return controlPoints;
        }

        public (Vector3, Vector3) Get2DEndPoints(bool localCoordinates = false) {
            (Vector3, Vector3) endpoints = (new Vector3(), new Vector3());
            if (orientation == Orientation.VERTICAL) {
                List<Vector3> cp = parentRoom.GetControlPoints(localCoordinates: localCoordinates, closed: true);
                endpoints.Item1 = cp[faceIndex];
                endpoints.Item2 = cp[faceIndex + 1];
            }
            return endpoints;
        }


        /// <summary>
        /// Add a interface to a face
        /// </summary>
        public void AddInterface(Interface interFace, float startParameter = 0.0f, float endParameter = 1.0f) {
            interfaces.Add(interFace);
            if (orientation == Orientation.VERTICAL) {
                paramerters.Add(interFace, new float[] { startParameter, endParameter });
            }

        }
        public void AddOpening(Opening opening) {
            openings.Add(opening);
        }

        /// <summary>
        /// Remove the interface 
        /// </summary>
        public void RemoveInterface(Interface interFace) {
            if (interfaces.Contains(interFace)) interfaces.Remove(interFace);
            if (paramerters.Keys.Contains(interFace)) paramerters.Remove(interFace);
        }
        public Interface GetInterfaceAtParameter(float parameterOnFace) {
            foreach (KeyValuePair<Interface, float[]> iface in paramerters) {
                if (parameterOnFace > iface.Value[0] && parameterOnFace < iface.Value[1]) {
                    //Debug.Log("Found Interface: " + iface.Key.GetEndPoint().ToString() + iface.Key.GetStartPoint().ToString());
                    return iface.Key;
                }
            }
            return null;
        }

        public bool CollidesWith(Vector3 point) {
            (Vector3 startPoint, Vector3 endPoint) = Get2DEndPoints(localCoordinates: false);
            float dxc = point.x - startPoint.x;
            float dyc = point.y - startPoint.y;
            float dxl = endPoint.x - startPoint.x;
            float dyl = endPoint.y - startPoint.y;
            float cross = dxc * dyl - dyc * dxl;

            // The point is on the line
            if (cross < 0.001f) {

                // Compare x
                if (Mathf.Abs(dxl) >= Mathf.Abs(dyl))
                    return dxl > 0 ?
                      startPoint.x <= point.x && point.x <= endPoint.x :
                      endPoint.x <= point.x && point.x <= startPoint.x;

                // Compare y
                else
                    return dyl > 0 ?
                      startPoint.z <= point.z && point.z <= endPoint.z :
                      endPoint.z <= point.z && point.z <= startPoint.z;
            }
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointOnFace(Vector3 point) {
            List<Vector3> endPoints = GetControlPoints(localCoordinates: false);

            if (Vector3.Distance(endPoints[0], point) < 0.01) return false;
            if (Vector3.Distance(endPoints[1], point) < 0.01) return false;

            return (Vector3.Distance(endPoints[0], point)
                    + Vector3.Distance(endPoints[1], point)
                    - Vector3.Distance(endPoints[0], endPoints[1]) < 0.001);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float GetPointParameter(Vector3 point) {
            (Vector3 faceStart, Vector3 faceEnd) = Get2DEndPoints();
            float parameterOnFace = (point - faceStart).magnitude / (faceEnd - faceStart).magnitude;
            return parameterOnFace;

        }
    }
}