using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Core {
    public class Interface {

        public Interface() {
            Faces = new Face[2];
        }

        public List<Room> Rooms {
            get { return Faces.Select(f => f.Room).ToList(); }
            set {; }
        }
        public Face[] Faces {
            get { return Faces; }
            set {; }
        }



        public Wall wall;

        /// <summary>
        /// 
        /// </summary>
        public Orientation Orientation {
            get { return Faces[0].orientation;  }
            private set {; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float WallThickness {
            get {
                float[] thicknesses = Faces.Where(f => f != null).Select(f => f.wallThickness).ToArray();
                return thicknesses.Max();
            }
            private set {; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localCoordinates"></param>
        /// <returns></returns>
        public Vector3 GetStartPoint(bool localCoordinates = false) {
            //Debug.Log("Face 0 parameters: "+attachedFaces[0].parameters[this][0] + " __ " + attachedFaces[0].parameters[this][0] ) ;

            float[] parameters = Faces[0].paramerters[this];
            (Vector3 fStartPoint, Vector3 fEndPoint) = Faces[0].Get2DEndPoints(localCoordinates: localCoordinates);
            Vector3 startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[0];

            return startPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localCoordinates"></param>
        /// <returns></returns>
        public Vector3 GetEndPoint(bool localCoordinates = false) {
            float[] parameters = Faces[0].paramerters[this];
            (Vector3 fStartPoint, Vector3 fEndPoint) = Faces[0].Get2DEndPoints(localCoordinates: localCoordinates);
            Vector3 startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[1];

            return startPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localCoordinates"></param>
        /// <returns></returns>
        public List<Vector3> GetSlabControlPoints(bool localCoordinates = false) {
            List<Vector3> cp = Faces[0].GetOGControlPoints(localCoordinates: localCoordinates).ToList();

            return cp;
        }

        /// <summary>
        /// Deletes the interface
        /// </summary>
        public void Delete() {
            if (Building.Instance.Interfaces.Contains(this)) {
                Building.Instance.RemoveInterface(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Opening> GetCoincidentOpenings() {
            List<Opening> openingsInParentFace = Faces[0].openings;
            List<Opening> relevantOpeningsInParentFace = new List<Opening>();
            foreach (Opening opening in openingsInParentFace) {
                if (opening.GetCoincidentInterface() == this) {
                    relevantOpeningsInParentFace.Add(opening);
                }
            }
            return relevantOpeningsInParentFace;
        }
    }
}