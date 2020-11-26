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

        public List<Space> Spaces {
            get {
                if (Faces != null && Faces.Count() > 0) {
                    return Faces.Select(f => f.Space).ToList();
                }
                else return new List<Space>();
            }
        }
        public List<Face> Faces { get; private set; }

        public List<Opening> Openings {
            get {
                return Faces.SelectMany(f => f.Openings
                    .Where(o => f.GetInterfaceAtParameter(f.Line.Parameter(o.CenterPoint)) == this))
                    .Distinct().ToList();
            }
        }

        public List<Opening> OpeningsVertical {
            get { return Openings.Where(o => o.Interface.Orientation == Orientation.VERTICAL).ToList(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Orientation Orientation {
            get { return Faces[0].Orientation; }
        }

        public float Length { get {return (StartPoint - EndPoint).magnitude;  } }


        public Interface(Face face, float startParameter = 0.0f, float endParameter = 1.0f) {
            Faces = new List<Face> { face };
            face.AddInterface(this, startParameter, endParameter);
        }

        public void AttachFace(Face face, float startParameter, float endParameter) {
            Faces.Add(face);
            face.AddInterface(this, startParameter, endParameter);
        }

        public List<float[]> Parameters {
            get { return Faces.Select(f => f.InterfaceParameters[this]).ToList(); }
        }



        public Vector3 StartPoint {
            get {
                if (Faces[0].Orientation == Orientation.VERTICAL) {
                    Vector3 faceStartPoint = Faces[0].Line.StartPoint;
                    Vector3 faceEndPoint = Faces[0].Line.EndPoint;
                    return faceStartPoint + (faceEndPoint - faceStartPoint) * Parameters[0][0];
                }
                else {
                    return Spaces[0].GetControlPoints()[0];
                }
            }
        }

        public Vector3 EndPoint {
            get {
                if (Faces[0].Orientation == Orientation.VERTICAL) {
                    Vector3 faceStartPoint = Faces[0].Line.StartPoint;
                    Vector3 faceEndPoint = Faces[0].Line.EndPoint;
                    return faceStartPoint + (faceEndPoint - faceStartPoint) * Parameters[0][1];
                }
                else {
                    return Spaces[0].GetControlPoints()[0];
                }
            }
        }

        public Vector3 CenterPoint {
            get {
                if (Faces[0].Orientation == Orientation.VERTICAL) {
                    Vector3 faceStartPoint = Faces[0].Line.StartPoint;
                    Vector3 faceEndPoint = Faces[0].Line.EndPoint;
                    return faceStartPoint + (faceEndPoint - faceStartPoint) * (Parameters[0][0]+ Parameters[0][1])/2;
                }
                else {
                    return Spaces[0].GetControlPoints()[0];
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public float Thickness {
            get {
                float[] thicknesses = Faces.Where(f => f != null).Select(f => f.Thickness).ToArray();
                return thicknesses.Max();
            }
            private set {; }
        }


        public override string ToString() {
            string textString = "Interface attached to:";
            if (Faces != null && Faces.Count > 0) textString += Faces[0].ToString();
            if (Faces != null && Faces.Count > 1) textString += Faces[1].ToString();
            return textString;
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