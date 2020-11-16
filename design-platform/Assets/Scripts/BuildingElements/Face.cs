using System.Collections.Generic;
using DesignPlatform.Geometry;
using UnityEngine;
using System.Linq;

namespace DesignPlatform.Core {
    public class Face {

        public Face() {
            InterfaceWalls = new Dictionary<Interface, Wall>();
            InterfaceSlabs = new Dictionary<Interface, Slab>();
            InterfaceOpenings = new Dictionary<Interface, List<Opening>>();
            InterfaceParameters = new Dictionary<Interface, float[]>();
        }

        public Room Room { get; private set; }
        public int FaceIndex { get; private set; }

        public Dictionary<Interface, float[]> InterfaceParameters { get; private set; }
        public List<Interface> Interfaces {
            get { return InterfaceParameters.Keys.Select(i => (Interface)i).ToList(); }
            private set {; }
        }

        public Dictionary<Interface, Wall> InterfaceWalls { get; private set; }
        public List<Wall> Walls {
            get { return InterfaceWalls.Values.Select(i => (Wall)i).ToList(); }
            private set {; }
        }

        public Dictionary<Interface, Slab> InterfaceSlabs { get; private set; }
        public List<Slab> Slabs {
            get { return InterfaceSlabs.Values.Select(i => (Slab)i).ToList(); }
            private set {; }
        }

        public Dictionary<Interface, List<Opening>> InterfaceOpenings { get; private set; }
        public List<Opening> Openings {
            get {
                if (InterfaceOpenings != null && InterfaceOpenings.Keys.Count > 0) {
                    return InterfaceOpenings.Values.SelectMany(i => i).ToList();
                }
                else return new List<Opening>();
            }
            private set {; }
        }

        public Orientation Orientation { get; private set; }

        public float Thickness = 0.2f;

        public Vector3 Normal {
            get { return Room.GetWallNormals()[FaceIndex]; }
            private set {; }
        }
        public float Length {
            get {
                return (Room.GetControlPoints()[FaceIndex]
                  - Room.GetControlPoints(closed: true)[FaceIndex + 1]).magnitude;
            }
            private set {; }
        }
        public float Height {
            get { return Room.height; }
            private set {; }
        }
        public Vector3 CenterPoint {
            get { return Room.GetWallMidpoints()[FaceIndex]; }
            private set {; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Face(Room parent, int index) {
            Interfaces = new List<Interface>();
            Openings = new List<Opening>();
            InterfaceParameters = new Dictionary<Interface, float[]>();
            Room = parent;
            FaceIndex = index;
            SetOrientation();
        }

        public override string ToString() {
            return Room.ToString() + " Face#" + FaceIndex.ToString();
        }

        /// <summary>
        /// Determs the orientation of the face (horizontal or vertical)
        /// </summary>
        private void SetOrientation() {
            if (FaceIndex < Room.GetControlPoints().Count) {
                Orientation = Orientation.VERTICAL;
            }
            else Orientation = Orientation.HORIZONTAL;
        }

        /// <summary>
        /// Gets the original controlpoints of the face (horizontal and vertical faces)
        /// </summary>
        public List<Vector3> GetControlPoints(bool localCoordinates = false) {
            List<Vector3> roomControlPoints = Room.GetControlPoints(localCoordinates: localCoordinates, closed: true);
            List<Vector3> controlPoints = new List<Vector3>();

            switch (Orientation) {
                // Vertical face
                case Orientation.VERTICAL:
                    controlPoints.Add(roomControlPoints[FaceIndex]);
                    controlPoints.Add(roomControlPoints[FaceIndex + 1]);
                    break;

                // Horizontal face
                case Orientation.HORIZONTAL:
                    controlPoints = roomControlPoints.GetRange(0, roomControlPoints.Count - 1);

                    // Top face
                    if (FaceIndex == roomControlPoints.Count) {
                        controlPoints = controlPoints.Select(p => p + Vector3.up * Height).ToList();
                    }
                    break;
            }

            return controlPoints;
        }

        public (Vector3, Vector3) Get2DEndPoints(bool localCoordinates = false) {
            (Vector3, Vector3) endpoints = (new Vector3(), new Vector3());
            if (Orientation == Orientation.VERTICAL) {
                List<Vector3> cp = Room.GetControlPoints(localCoordinates: localCoordinates, closed: true);
                endpoints.Item1 = cp[FaceIndex];
                endpoints.Item2 = cp[FaceIndex + 1];
            }
            return endpoints;
        }

        /// <summary>
        /// Add an interface to the managed list of interfaces
        /// </summary>
        /// <param name="interFace"></param>
        public void AddInterface(Interface interFace, float startParameter = 0.0f, float endParameter = 1.0f) {
            if (Orientation == Orientation.VERTICAL) {
                InterfaceParameters.Add(interFace, new float[] { startParameter, endParameter });
            }
            else {
                InterfaceParameters.Add(interFace, new float[] { 0.0f, 0.0f });
            }
        }
        /// <summary>
        /// Remove an interface from the managed list of interfaces
        /// </summary>
        /// <param name="interFace"></param>
        public void RemoveInterface(Interface interFace) {
            if (Interfaces.Contains(interFace)) InterfaceParameters.Remove(interFace);
        }

        /// <summary>
        /// Add a wall to the managed list of walls
        /// </summary>
        /// <param name="interFace"></param>
        /// <param name="wall"></param>
        public void AddWall(Interface interFace, Wall wall) {
            InterfaceWalls.Add(interFace, wall);
        }
        /// <summary>
        /// Remove a wall from the managed list of walls
        /// </summary>
        /// <param name="wall"></param>
        public void RemoveWall(Wall wall) {
            if (Walls.Contains(wall)) InterfaceWalls.Remove(wall.Interface);
        }

        /// <summary>
        /// Add a slab to the managed list of slabs
        /// </summary>
        /// <param name="interFace"></param>
        /// <param name="slab"></param>
        public void AddSlab(Interface interFace, Slab slab) {
            InterfaceSlabs.Add(interFace, slab);
        }
        /// <summary>
        /// Remove a slab from the managed list of slabs
        /// </summary>
        /// <param name="slab"></param>
        public void RemoveSlab(Slab slab) {
            if (Slabs.Contains(slab)) InterfaceSlabs.Remove(slab.Interface);
        }

        /// <summary>
        /// Add an opening to the managed list of openings
        /// </summary>
        /// <param name="interFace"></param>
        /// <param name="opening"></param>
        public void AddOpening(Interface interFace, Opening opening) {
            if (!InterfaceOpenings.Keys.Contains(interFace)) {
                InterfaceOpenings.Add(interFace, new List<Opening>());
            }
            else {
                InterfaceOpenings[interFace].Add(opening);
            }
        }
        /// <summary>
        /// Remove an opening from the managed list of openings
        /// </summary>
        /// <param name="opening"></param>
        public void RemoveOpening(Opening opening) {
            if (Openings.Contains(opening)) InterfaceOpenings.Remove(opening.Interface);
        }







        public Interface GetInterfaceAtParameter(float parameterOnFace) {
            foreach (KeyValuePair<Interface, float[]> iface in InterfaceParameters) {
                if (parameterOnFace > iface.Value[0] && parameterOnFace < iface.Value[1]) {
                    return iface.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointOnFace(Vector3 point) {
            Line faceLine = new Line(Get2DEndPoints());
            return faceLine.IsOnLine(point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float GetPointParameter(Vector3 point) {
            Line faceLine = new Line(Get2DEndPoints());
            return faceLine.Parameter(point);
        }
    }
}