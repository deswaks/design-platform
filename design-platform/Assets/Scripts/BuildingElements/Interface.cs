﻿using Microsoft.VisualBasic.ApplicationServices;
using System.Collections;
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
        /// Deletes the interface
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