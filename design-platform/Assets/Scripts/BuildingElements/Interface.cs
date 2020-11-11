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
            float[] parameters = attachedFaces[0].paramerters[this];
            (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
            Vector3 startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[0];

            return startPoint;
        }

        public Vector3 GetEndPoint(bool localCoordinates = false) {
            float[] parameters = attachedFaces[0].paramerters[this];
            (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
            Vector3 startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[1];

            return startPoint;
        }

        public List<Vector3> GetSlabControlPoints(bool localCoordinates = false) {
            List<Vector3> cp = attachedFaces[0].GetOGControlPoints(localCoordinates: localCoordinates).ToList();

            return cp;
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
                Debug.Log(GetEndPoint().ToString() + GetStartPoint().ToString());
                Debug.Log("Check: " + (opening.GetCoincidentInterface() == this).ToString());
                if(opening.GetCoincidentInterface() == this) {
                    relevantOpeningsInParentFace.Add(opening);
                }
            }
            return relevantOpeningsInParentFace;
        }
    }
}