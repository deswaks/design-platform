using Microsoft.VisualBasic.ApplicationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Core {
    public class Interface {
        public Face[] attachedFaces = new Face[2];
        public Wall wall;

        public Vector3 GetStartPoint(bool localCoordinates = false) {
            Vector3 startPoint = new Vector3();
            if (attachedFaces[0].orientation == Orientation.VERTICAL) {
                float[] parameters = attachedFaces[0].paramerters[this];
                (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
                startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[0];
            }
            else {
                startPoint = attachedFaces[0].parentRoom.GetControlPoints(localCoordinates: localCoordinates)[0];
            }
            return startPoint;
        }

        public Vector3 GetEndPoint(bool localCoordinates = false) {
            Vector3 endPoint = new Vector3();
            if (attachedFaces[0].orientation == Orientation.VERTICAL) {
                float[] parameters = attachedFaces[0].paramerters[this];
                (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
                endPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[1];
            }
            else {
                List<Vector3> cp = attachedFaces[0].parentRoom.GetControlPoints(localCoordinates: localCoordinates);
                endPoint = cp[cp.Count];
            }
            return endPoint;
        }

        /// <summary>
        /// Deletes the room
        /// </summary>
        public void Delete() {
            if (Building.Instance.interfaces.Contains(this)) {
                Building.Instance.RemoveInterface(this);
                attachedFaces[0].RemoveInterface(this);
                if(attachedFaces[1]!= null) attachedFaces[1].RemoveInterface(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float GetWallThickness() {
            float[] thicknesses = attachedFaces.Where(f => f != null).Select(f => f.wallThickness).ToArray();
            return thicknesses.Max();
        }
        public Orientation GetOrientation() {
            return attachedFaces[0].orientation;
        }
        public List<Opening> GetCoincidentOpenings() {
            List<Opening> openingsInParentFace = attachedFaces[0].openings;
            List<Opening> relevantOpeningsInParentFace = new List<Opening>();
            foreach (Opening opening in openingsInParentFace) {
                if(opening.GetCoincidentInterface() == this) {
                    relevantOpeningsInParentFace.Add(opening);
                }
            }
            return relevantOpeningsInParentFace;
        }
    }
}