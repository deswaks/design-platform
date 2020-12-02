using System.Collections.Generic;
using DesignPlatform.Geometry;
using UnityEngine;
using System.Linq;

namespace DesignPlatform.Core {

    /// <summary>
    /// Represents a two dimensional face on a three dimensional solid building space polygon.
    /// </summary>
    public class Face {

        /// <summary> Proposed thickness of the future wall on the face.</summary>
        public float Thickness = 0.2f;
        


        /// <summary>
        /// Default constructor to create a new face.
        /// </summary>
        /// <param name="parentSpace"></param>
        /// <param name="index"></param>
        public Face(Space parentSpace, int index) {
            OpeningParameters = new Dictionary<Opening, float>();
            InterfaceParameters = new Dictionary<Interface, float[]>();
            Space = parentSpace;
            SpaceIndex = index;
        }



        /// <summary>The space which this face belongs to.</summary>
        public Space Space { get; private set; }
        
        /// <summary>Index of this face in the parent space's list of faces.</summary>
        public int SpaceIndex { get; private set; }
        
        /// <summary>Interfaces connected to this face.</summary>
        public List<Interface> Interfaces {
            get {
                return InterfaceParameters.Keys.Select(i => (Interface)i).ToList();
            }
        }

        /// <summary>The parameters on this face at which the start and end point of
        /// each of the connected interfaces lie.</summary>
        public Dictionary<Interface, float[]> InterfaceParameters { get; private set; }

        /// <summary>The openings located on this face.</summary>
        public List<Opening> Openings {
            get { return OpeningParameters.Keys.Select(i => (Opening)i).ToList(); }
        }
        
        /// <summary>The parameters on this face at which the openings lie.</summary>
        public Dictionary<Opening, float> OpeningParameters { get; private set; }

        /// <summary>THe orientation of this face.</summary>
        public Orientation Orientation {
            get {
                if (SpaceIndex < Space.GetControlPoints().Count) return Orientation.VERTICAL;
                else return Orientation.HORIZONTAL;
            }
        }

        /// <summary>The normal directions of this face represented as a surface.</summary>
        public Vector3 Normal {
            get { return Space.GetFaceNormals()[SpaceIndex]; }
        }

        /// <summary>The height of this face.</summary>
        public float Height {
            get { return Space.Height; }
        }

        /// <summary>The two dimensional line at the base of this face.</summary>
        public Line LocationLine {
            get {
                List<Vector3> cp = Space.GetControlPoints(closed: true);
                return new Line(cp[SpaceIndex], cp[SpaceIndex+1]); }
        }


        /// <summary>
        /// Writes a description of the face.
        /// </summary>
        /// <returns>The description of this face.</returns>
        public override string ToString() {
            return Space.ToString() + " Face #" + SpaceIndex.ToString();
        }

        /// <summary>
        /// Gets the original controlpoints of the face (horizontal and vertical faces)
        /// </summary>
        /// <param name="localCoordinates">Specify whether the coordinates should be given in the local system.</param>
        /// <returns></returns>
        public List<Vector3> GetControlPoints(bool localCoordinates = false) {
            List<Vector3> spaceControlPoints = Space.GetControlPoints(localCoordinates: localCoordinates, closed: true);
            List<Vector3> controlPoints = new List<Vector3>();

            switch (Orientation) {
                // Vertical face
                case Orientation.VERTICAL:
                    controlPoints.Add(spaceControlPoints[SpaceIndex]);
                    controlPoints.Add(spaceControlPoints[SpaceIndex + 1]);
                    break;

                // Horizontal face
                case Orientation.HORIZONTAL:
                    controlPoints = spaceControlPoints.GetRange(0, spaceControlPoints.Count - 1);

                    // Top face
                    if (SpaceIndex == spaceControlPoints.Count) {
                        controlPoints = controlPoints.Select(p => p + Vector3.up * Height).ToList();
                    }
                    break;
            }
            return controlPoints;
        }

        /// <summary>
        /// Add an interface to the managed list of interfaces
        /// </summary>
        /// <param name="interFace">Interface to add.</param>
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
        /// <param name="interFace">Interface to remove.</param>
        public void RemoveInterface(Interface interFace) {
            if (Interfaces.Contains(interFace)) InterfaceParameters.Remove(interFace);
        }

        /// <summary>
        /// Add an opening to the managed list of openings
        /// </summary>
        /// <param name="opening">Opening to add.</param>
        public void AddOpening(Opening opening) {
            if (Openings.Contains(opening)) return;
            float parameter = LocationLine.ParameterAtPoint(opening.LocationPoint);
            OpeningParameters.Add(opening, parameter);
        }

        /// <summary>
        /// Remove an opening from the managed list of openings
        /// </summary>
        /// <param name="opening">Opening to remove.</param>
        public void RemoveOpening(Opening opening) {
            if (Openings.Contains(opening)) OpeningParameters.Remove(opening);
        }

        /// <summary>
        /// Find the interface which connects to this face at a specific parameter.
        /// </summary>
        /// <param name="parameterOnFace">Parameter on this face.</param>
        /// <returns>Interface at the specified parameter.</returns>
        public Interface GetInterfaceAtParameter(float parameterOnFace) {
            foreach (KeyValuePair<Interface, float[]> iface in InterfaceParameters) {
                if (parameterOnFace > iface.Value[0] && parameterOnFace < iface.Value[1]) {
                    return iface.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Delete the face object
        /// </summary>
        public void Delete() {
            if (Openings != null && Openings.Count > 0) {
                foreach (Opening opening in Openings) {
                    if (opening.Faces.Count == 1) opening.Delete();
                    if (opening.Faces.Count > 1) opening.Faces.Remove(this);
                }
            }
            if (Interfaces != null && Interfaces.Count > 0) {
                foreach (Interface interFace in Interfaces) {
                    if (interFace.Faces.Count == 1) interFace.Delete();
                }
            }
        }
    }
}