using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

<<<<<<< Updated upstream
public class Interface {
    public Face[] attachedFaces = new Face[2];
    public Wall wall;

    public Vector3 GetStartPoint(bool localCoordinates = false) {
        float[] parameters = attachedFaces[0].paramerters[this];
        (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
        Vector3 startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[0];

        return startPoint;
    }

    public Vector3 GetEndPoint(bool localCoordinates = false) {
        float[] parameters = attachedFaces[0].paramerters[this];
        (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
        Vector3 startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[1];
=======
namespace DesignPlatform.Core {
    public class Interface {

        public List<Room> Rooms {
            get {
                if (Faces != null && Faces.Count() > 0) {
                    return Faces.Select(f => f.Room).ToList();
                }
                else return new List<Room>();
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

        public Vector3 Origin {
            get { return Rooms[0].GetControlPoints(localCoordinates: false)[Faces[0].FaceIndex]; }
        }

        public Vector3 StartPoint {
            get {
                if (Orientation == Orientation.VERTICAL) {
                    Vector3 faceStartPoint = Faces[0].Line.StartPoint;
                    Vector3 faceEndPoint = Faces[0].Line.EndPoint;
                    return faceStartPoint + (faceEndPoint - faceStartPoint) * Parameters[0][0];
                }
                else {
                    return Rooms[0].GetControlPoints()[0];
                }
            }
        }

        public Vector3 EndPoint {
            get {
                if (Orientation == Orientation.VERTICAL) {
                    Vector3 faceStartPoint = Faces[0].Line.StartPoint;
                    Vector3 faceEndPoint = Faces[0].Line.EndPoint;
                    return faceStartPoint + (faceEndPoint - faceStartPoint) * Parameters[0][1];
                }
                else {
                    return Rooms[0].GetControlPoints()[0];
                }
            }
        }

        public Vector3 CenterPoint {
            get {
                if (Faces[0].Orientation == Orientation.VERTICAL) {
                    Vector3 faceStartPoint = Faces[0].Line.StartPoint;
                    Vector3 faceEndPoint = Faces[0].Line.EndPoint;
                    return faceStartPoint + (faceEndPoint - faceStartPoint) * (Parameters[0][0] + Parameters[0][1]) / 2;
                }
                else {
                    return Rooms[0].GetControlPoints()[0];
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
        }
>>>>>>> Stashed changes

        return startPoint;
    }

<<<<<<< Updated upstream
    /// <summary>
    /// Deletes the room
    /// </summary>
    public void Delete() {
        if (Building.Instance.interfaces.Contains(this)) {
            Building.Instance.RemoveInterface(this);
=======
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

        public List<Vector3> GetControlPoints(bool localCoordinates = false, bool closed = false) {
            // Get room control points
            List<Vector3> controlPoints = Rooms[0].GetControlPoints(localCoordinates: false, closed: true);
            if (localCoordinates) {
                controlPoints = controlPoints.Select(p => new Vector3(
                    p.x - Origin.x, p.y - Origin.y, p.z - Origin.z)).ToList();
            }

            // Slab
            if (Orientation == Orientation.HORIZONTAL) {
                if (closed) return controlPoints;
                else return controlPoints.GetRange(0, controlPoints.Count - 1);
            }
            //Wall
            if (Orientation == Orientation.VERTICAL) {
                Vector3 start = controlPoints[Faces[0].FaceIndex];
                Vector3 end = controlPoints[Faces[0].FaceIndex+1];
                List<Vector3> wallPoints = new List<Vector3> {
                    new Vector3(start.x, start.y, start.z),
                    new Vector3(start.x, start.y+ Rooms[0].height, start.z),
                    new Vector3(end.x, end.y+ Rooms[0].height, end.z),
                    new Vector3(end.x, end.y, end.z),
                    new Vector3(start.x, start.y, start.z)};
                if (closed) return wallPoints;
                else return wallPoints.GetRange(0, wallPoints.Count - 1);
            }
            return null;
        }

        


        public override string ToString() {
            string textString = "Interface attached to: ";
            if (Faces != null && Faces.Count > 0) textString += Faces[0].ToString();
            if (Faces != null && Faces.Count > 1) textString += ", "+Faces[1].ToString();
            return textString;
>>>>>>> Stashed changes
        }
    }

<<<<<<< Updated upstream
    /// <summary>
    /// 
    /// </summary>
    public float GetWallThickness() {
        float[] thicknesses = attachedFaces.Where(f => f != null).Select(f => f.wallThickness).ToArray();
        return thicknesses.Max();
=======
        /// <summary>
        /// Deletes the interface
        /// </summary>
        public void Delete() {
            foreach (Face face in Faces) {
                face.RemoveInterface(this);
            }
        }

        public List<Vector2> GetLocalNormalOuterVertices(bool closed = false) {
            float Length = (StartPoint - EndPoint).magnitude;
            float Height = Rooms[0].height;
            List<Vector2> localNormalVertices = new List<Vector2> {
                    new Vector2 (Length/2, 0),
                    new Vector2 (Length/2,  Height-0.01f),
                    new Vector2 (-Length/2, Height-0.01f),
                    new Vector2 (-Length/2, 0),
                    new Vector2 (Length/2, 0)
            };
            if (!closed) return localNormalVertices.GetRange(0, localNormalVertices.Count - 1).ToList();
            else return localNormalVertices;
        }

        public List<List<Vector2>> GetLocalNormalHoleVertices(bool closed = false) {
            List<List<Vector2>> allHoleVertices = new List<List<Vector2>>();

            foreach (Opening opening in Openings) {
                float CenterX = Vector3.Distance(StartPoint, opening.CenterPoint);
                List<Vector2> holeVertices = new List<Vector2> {
                    new Vector2 ( CenterX-opening.Width/2, opening.SillHeight),
                    new Vector2 ( CenterX-opening.Width/2, opening.SillHeight+opening.Height),
                    new Vector2 ( CenterX+opening.Width/2, opening.SillHeight+opening.Height),
                    new Vector2 ( CenterX+opening.Width/2, opening.SillHeight),
                    new Vector2 ( CenterX-opening.Width/2, opening.SillHeight)
                };
                if (!closed) allHoleVertices.Add(holeVertices.GetRange(0, holeVertices.Count - 1).ToList());
                else allHoleVertices.Add(holeVertices);
            }
            return allHoleVertices;
        }
>>>>>>> Stashed changes
    }
}
