using Microsoft.VisualBasic.ApplicationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Core {
    public class Interface {
        public Face[] attachedFaces = new Face[2];
        public Wall wall;

        public override string ToString() {
            string textString = "Interface attached to:";
            if (attachedFaces[0] != null) textString += " [0] " + attachedFaces[0].ToString();
            if (attachedFaces[1] != null) textString += " and [1] " + attachedFaces[1].ToString();
            return textString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localCoordinates"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localCoordinates"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="localCoordinates"></param>
        /// <returns></returns>
        public Vector3 GetCenterPoint(bool localCoordinates = false) {
            Vector3 SP = GetStartPoint(localCoordinates: localCoordinates);
            Vector3 EP = GetEndPoint(localCoordinates: localCoordinates);
            Vector3 CP = new Vector3((SP.x + EP.x) / 2f, (SP.y + EP.y) / 2f, (SP.z + EP.z) / 2f);
            return CP;
        }

        /// <summary>
        /// Deletes the room
        /// </summary>
        public void Delete() {
            if (Building.Instance.Interfaces.Contains(this)) {
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Orientation GetOrientation() {
            return attachedFaces[0].orientation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public List<Opening> GetCoincidentOpenings2() {
        //    List<Opening> openingsInParentFace = attachedFaces[0].openings;

        //    if (attachedFaces[1] != null) { openingsInParentFace.AddRange(attachedFaces[1].openings); }
        //    openingsInParentFace = openingsInParentFace.Distinct().ToList();

        //    List<Opening> relevantOpeningsInParentFace = new List<Opening>();
        //    foreach (Opening opening in openingsInParentFace) {
        //        if (opening.GetCoincidentInterface() == this) {
        //            relevantOpeningsInParentFace.Add(opening);
        //        }
        //    }
        //    return relevantOpeningsInParentFace;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Opening> GetCoincidentOpenings() {

            List<Opening> openingsInParentFace = attachedFaces[0].openings;

            //Debug.Log("Antal openings i face 0: "+attachedFaces[0].openings.Count);
            //if (attachedFaces[1] != null) Debug.Log("Antal openings i face 1: "+attachedFaces[1].openings.Count);
            //if (attachedFaces[1] != null) { openingsInParentFace.AddRange(attachedFaces[1].openings); }
            //openingsInParentFace = openingsInParentFace.Distinct().ToList();
            List<Opening> relevantOpeningsInParentFace = new List<Opening>();

            foreach (Opening o in openingsInParentFace) {
                if (IsPointCBetweenAB(GetStartPoint(), GetEndPoint(), o.transform.position)) {
                    relevantOpeningsInParentFace.Add(o);
                }
            }
            return relevantOpeningsInParentFace;
        }
        public bool IsPointCBetweenAB(Vector3 A, Vector3 B, Vector3 C) {
            if (Vector3.Distance(A, C) < 0.01) return false;
            if (Vector3.Distance(B, C) < 0.01) return false;
            return (Vector3.Distance(A, C)
                    + Vector3.Distance(B, C)
                    - Vector3.Distance(A, B) < 0.001);
        }
    }
}