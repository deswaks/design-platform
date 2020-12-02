using DesignPlatform.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Core {

    /// <summary>
    /// Represents the shared face between two adjacent building spaces or the shared face
    /// between a face and the greater external space if no adjacent building spaces exist
    /// </summary>
    public class Interface {

        /// <summary>Spaces adjacent to this interface.</summary>
        public List<Space> Spaces {
            get {
                if (Faces != null && Faces.Count() > 0) {
                    return Faces.Select(f => f.Space).ToList();
                }
                else return new List<Space>();
            }
        }

        /// <summary>Faces connected to this interface.</summary>
        public List<Face> Faces { get; private set; }

        /// <summary>The location of this interface given as parameters on the lines of its connected faces.</summary>
        public List<float[]> Parameters {
            get { return Faces.Select(f => f.InterfaceParameters[this]).ToList(); }
        }

        /// <summary>Openings on this interface.</summary>
        public List<Opening> Openings {
            get {
                return Faces.SelectMany(face => face.Openings
                    .Where(o => face.GetInterfaceAtParameter(face.LocationLine.ParameterAtPoint(o.LocationPoint)) == this))
                    .Distinct().ToList();
            }
        }

        /// <summary>Two dimensional line at the base of this interface.</summary>
        public Line LocationLine {
            get { return new Line(StartPoint, EndPoint); }
        }

        /// <summary>The orientation of this interface. It can be either horizontal or vertical.</summary>
        public Orientation Orientation {
            get { return Faces[0].Orientation; }
        }



        /// <summary>
        /// Default constructor uses a single face and attaches this
        /// </summary>
        /// <param name="face">Connected face (a second can be added after construction)</param>
        /// <param name="startParameter">Start location of this interface given as a parameter on the line of the connected face</param>
        /// <param name="endParameter">End location of this interface given as a parameter on the line of the connected face</param>
        public Interface(Face face, float startParameter = 0.0f, float endParameter = 1.0f) {
            Faces = new List<Face> { face };
            face.AddInterface(this, startParameter, endParameter);
        }



        /// <summary>
        /// The start point of the location line of this interface
        /// </summary>
        public Vector3 StartPoint {
            get {
                if (Faces[0].Orientation == Orientation.VERTICAL) {
                    Vector3 faceStartPoint = Faces[0].LocationLine.StartPoint;
                    Vector3 faceEndPoint = Faces[0].LocationLine.EndPoint;
                    return faceStartPoint + (faceEndPoint - faceStartPoint) * Parameters[0][0];
                }
                else {
                    return Spaces[0].GetControlPoints()[0];
                }
            }
        }

        /// <summary>
        /// The end point of the location line of this interface
        /// </summary>
        public Vector3 EndPoint {
            get {
                if (Faces[0].Orientation == Orientation.VERTICAL) {
                    Vector3 faceStartPoint = Faces[0].LocationLine.StartPoint;
                    Vector3 faceEndPoint = Faces[0].LocationLine.EndPoint;
                    return faceStartPoint + (faceEndPoint - faceStartPoint) * Parameters[0][1];
                }
                else {
                    return Spaces[0].GetControlPoints()[0];
                }
            }
        }

        /// <summary>The maximum preferred thickness of all the connected faces</summary>
        public float Thickness {
            get {
                float[] thicknesses = Faces.Where(f => f != null).Select(f => f.Thickness).ToArray();
                return thicknesses.Max();
            }
        }



        /// <summary>
        /// Writes a description of the interface.
        /// </summary>
        /// <returns>The description of this interface.</returns>
        public override string ToString() {
            string textString = "Interface attached to:";
            if (Faces != null && Faces.Count > 0) textString += " " + Faces[0].ToString();
            if (Faces != null && Faces.Count > 1) textString += " " + Faces[1].ToString();
            return textString;
        }

        /// <summary>
        /// Attaches a second face as connected to this interface.
        /// </summary>
        /// <param name="face">Second connected face</param>
        /// <param name="startParameter">Start location of this interface given as a parameter on the line of the connected face</param>
        /// <param name="endParameter">End location of this interface given as a parameter on the line of the connected face</param>
        public void AttachFace(Face face, float startParameter, float endParameter) {
            Faces.Add(face);
            face.AddInterface(this, startParameter, endParameter);
        }

        /// <summary>
        /// Deletes the interface
        /// </summary>
        public void Delete() {
            foreach (Face face in Faces) {
                face.RemoveInterface(this);
            }
        }
    }
}