﻿using Microsoft.VisualBasic.ApplicationServices;
using DesignPlatform.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPlatform.Core {
    public class Interface {

        public Interface(Face face) {
            Faces = new List<Face>();
            Faces.Add(face);
        }
        public Interface(List<Face> faces) {
            Faces = faces;
        }

        public List<Room> Rooms {
            get {
                if (Faces != null && Faces.Count() > 0) {
                    return Faces.Select(f => f.Room).ToList();
                }
                else return new List<Room>();
            }
            set {; }
        }
        public List<Face> Faces {get; set;}
        public Wall Wall {
            get { return Faces[0].InterfaceWalls[this]; }
            private set {; } }
        public Slab Slab {
            get { return Faces[0].InterfaceSlabs[this]; }
            private set {; }
        }
        public List<Opening> Openings {
            get { return Faces.SelectMany(f => f.InterfaceOpenings[this]).ToList().Distinct().ToList(); }
            private set {; }
        }

        public List<Opening> OpeningsVertical {
            get {
                List<Opening> faceOpenings = Faces.SelectMany(f => f.Openings).Distinct().ToList();
                Line interfaceLine = new Line(GetStartPoint(), GetEndPoint());
                return faceOpenings.Where(o => interfaceLine.IsOnLine(o.CenterPoint)).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Orientation Orientation {
            get { return Faces[0].Orientation;  }
            private set {; }
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
            if (Faces[0] != null) textString += " [0] " + Faces[0].ToString();
            if (Faces[1] != null) textString += " and [1] " + Faces[1].ToString();
            return textString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localCoordinates"></param>
        /// <returns></returns>
        public Vector3 GetStartPoint(bool localCoordinates = false) {
            Vector3 startPoint = new Vector3();
            if (Faces[0].Orientation == Orientation.VERTICAL) {
                float[] parameters = Faces[0].InterfaceParameters[this];
                (Vector3 fStartPoint, Vector3 fEndPoint) = Faces[0].Get2DEndPoints(localCoordinates: localCoordinates);
                startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[0];
            }
            else {
                startPoint = Rooms[0].GetControlPoints(localCoordinates: localCoordinates)[0];
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
            if (Faces[0].Orientation == Orientation.VERTICAL) {
                float[] parameters = Faces[0].InterfaceParameters[this];
                (Vector3 fStartPoint, Vector3 fEndPoint) = Faces[0].Get2DEndPoints(localCoordinates: localCoordinates);
                endPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[1];
            }
            else {
                List<Vector3> cp = Rooms[0].GetControlPoints(localCoordinates: localCoordinates);
                endPoint = cp[cp.Count-1];
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
            Building.Instance.RemoveInterface(this);
            foreach (Face face in Faces) {
                face.RemoveInterface(this);
            }
        }
    }
}