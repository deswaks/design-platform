﻿using System.Collections.Generic;
using DesignPlatform.Geometry;
using UnityEngine;
using System.Linq;

namespace DesignPlatform.Core {

    /// <summary>
    /// Represents a two dimensional face on a three dimensional solid building space polygon.
    /// </summary>
    public class Face {

        public Face(Space parentSpace, int index) {
            OpeningParameters = new Dictionary<Opening, float>();
            InterfaceParameters = new Dictionary<Interface, float[]>();
            Space = parentSpace;
            FaceIndex = index;
            SetOrientation();
        }

        public Space Space { get; private set; }
        public int FaceIndex { get; private set; }

        public Dictionary<Interface, float[]> InterfaceParameters { get; private set; }
        public List<Interface> Interfaces {
            get {
                return InterfaceParameters.Keys.Select(i => (Interface)i).ToList();
            }
        }

        public Dictionary<Opening, float> OpeningParameters { get; private set; }
        public List<Opening> Openings {
            get { return OpeningParameters.Keys.Select(i => (Opening)i).ToList(); }
        }


        public Orientation Orientation { get; private set; }

        public float Thickness = 0.2f;

        public Vector3 Normal {
            get { return Space.GetWallNormals()[FaceIndex]; }
            private set {; }
        }
        public float Length {
            get {
                return (Space.GetControlPoints()[FaceIndex]
                  - Space.GetControlPoints(closed: true)[FaceIndex + 1]).magnitude;
            }
            private set {; }
        }
        public float Height {
            get { return Space.height; }
            private set {; }
        }

        public Vector3 StartPoint {
            get { return Space.GetControlPoints()[FaceIndex]; }
            private set {; }
        }

        public Vector3 EndPoint {
            get { return Space.GetControlPoints(closed: true)[FaceIndex + 1]; }
            private set {; }
        }

        public Line Line {
            get { return new Line(this); }
            private set {; }
        }

        public Vector3 CenterPoint {
            get { return Space.GetWallMidpoints()[FaceIndex]; }
            private set {; }
        }

        public override string ToString() {
            return Space.ToString() + " Face#" + FaceIndex.ToString();
        }

        /// <summary>
        /// Determs the orientation of the face (horizontal or vertical)
        /// </summary>
        private void SetOrientation() {
            if (FaceIndex < Space.GetControlPoints().Count) {
                Orientation = Orientation.VERTICAL;
            }
            else Orientation = Orientation.HORIZONTAL;
        }

        /// <summary>
        /// Gets the original controlpoints of the face (horizontal and vertical faces)
        /// </summary>
        public List<Vector3> GetControlPoints(bool localCoordinates = false) {
            List<Vector3> spaceControlPoints = Space.GetControlPoints(localCoordinates: localCoordinates, closed: true);
            List<Vector3> controlPoints = new List<Vector3>();

            switch (Orientation) {
                // Vertical face
                case Orientation.VERTICAL:
                    controlPoints.Add(spaceControlPoints[FaceIndex]);
                    controlPoints.Add(spaceControlPoints[FaceIndex + 1]);
                    break;

                // Horizontal face
                case Orientation.HORIZONTAL:
                    controlPoints = spaceControlPoints.GetRange(0, spaceControlPoints.Count - 1);

                    // Top face
                    if (FaceIndex == spaceControlPoints.Count) {
                        controlPoints = controlPoints.Select(p => p + Vector3.up * Height).ToList();
                    }
                    break;
            }

            return controlPoints;
        }

        public (Vector3, Vector3) Get2DEndPoints(bool localCoordinates = false) {
            (Vector3, Vector3) endpoints = (new Vector3(), new Vector3());
            if (Orientation == Orientation.VERTICAL) {
                List<Vector3> cp = Space.GetControlPoints(localCoordinates: localCoordinates, closed: true);
                endpoints.Item1 = cp[FaceIndex];
                endpoints.Item2 = cp[FaceIndex + 1];
            }
            return endpoints;
        }

        /// <summary>
        /// Add an interface to the managed list of interfaces
        /// </summary>
        /// <param name="interFace"></param>
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
        /// <param name="interFace"></param>
        public void RemoveInterface(Interface interFace) {
            if (Interfaces.Contains(interFace)) InterfaceParameters.Remove(interFace);
        }

        /// <summary>
        /// Add an opening to the managed list of openings
        /// </summary>
        /// <param name="interFace"></param>
        /// <param name="opening"></param>
        public void AddOpening(Opening opening) {
            if (Openings.Contains(opening)) return;
            float parameter = Line.Parameter(opening.CenterPoint);
            OpeningParameters.Add(opening, parameter);
        }

        /// <summary>
        /// Remove an opening from the managed list of openings
        /// </summary>
        /// <param name="opening"></param>
        public void RemoveOpening(Opening opening) {
            if (Openings.Contains(opening)) OpeningParameters.Remove(opening);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterOnFace"></param>
        /// <returns></returns>
        public Interface GetInterfaceAtParameter(float parameterOnFace) {
            foreach (KeyValuePair<Interface, float[]> iface in InterfaceParameters) {
                if (parameterOnFace > iface.Value[0] && parameterOnFace < iface.Value[1]) {
                    return iface.Key;
                }
            }
            return null;
        }
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