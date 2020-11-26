﻿using DesignPlatform.Database;
using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {

    public enum OpeningShape {
        DOOR,
        WINDOW
    }

    public enum OpeningState {
        PLACED,
        PREVIEW
    }

    public class Opening : MonoBehaviour {

        public List<Room> Rooms {
            get {
                if (Faces != null && Faces.Count() > 0) {
                    return Faces.Select(f => f.Room).ToList();
                }
                else return new List<Room>();
            }
            set {; }
        }
        public List<Face> Faces { get; private set; }

        public List<float> Parameters {
            get { return Faces.Select(f => f.OpeningParameters[this]).ToList() ; }
            private set {; }
        }

        public Interface Interface {
            get { return Faces[0].GetInterfaceAtParameter(Parameters[0]); }
            private set {; }
        }

        private float WindowWidth = 1.6f;
        private float WindowHeight = 1.05f;

        private float DoorWidth = 0.9f;
        private float DoorHeight = 2.1f;
        private float Doorstep = 0.05f;

        public float OpeningDepth = 0.051f;
        public float SillHeight;
        public float Width;
        public float Height;
        public float RoomHeight {
            get { if (Faces != null && Faces.Count > 0) return Faces[0].Room.height;
                else return 3.0f;
            }
        }

        public OpeningShape Shape { get; private set; }
        public OpeningState State { get; private set; }
        public List<Vector3> ControlPoints { get; private set; }

        public Vector3 CenterPoint {
            get { return gameObject.transform.position; }
            private set {; }
        }

        /// <summary>
        /// The placement point of the opening on the wall (bottom) placement line.
        /// </summary>
        public Vector3 PlacementPoint {
            get { return gameObject.transform.position; }
            private set {; }
        }


        public void InitializeOpening(OpeningShape shape = OpeningShape.WINDOW,
                                      OpeningState state = OpeningState.PREVIEW) {
            Shape = shape;
            State = state;

            
            Material material = AssetUtil.LoadAsset<Material>("materials", "openingMaterial"); ;

            switch (Shape) {
                case OpeningShape.WINDOW:
                    gameObject.layer = 16; // Window layer
                    Width = WindowWidth;
                    Height = DoorHeight;// WindowHeight;
                    SillHeight = Doorstep;// 1.1f;
                    material = AssetUtil.LoadAsset<Material>("materials", "openingWindow");
                    gameObject.name = "Window";

                    break;
                case OpeningShape.DOOR:
                    gameObject.layer = 15; // Door layer
                    Width = DoorWidth;
                    Height = DoorHeight;
                    SillHeight = Doorstep;
                    material = AssetUtil.LoadAsset<Material>("materials", "openingDoor");
                    gameObject.name = "Door";
                    break;
            }
            if (State == OpeningState.PREVIEW) gameObject.name = "Preview "+gameObject.name;
            ControlPoints = new List<Vector3> {
                        new Vector3 (Width/2,SillHeight,0),
                        new Vector3 (Width/2,SillHeight+Height,0),
                        new Vector3 (-Width/2,SillHeight+Height,0),
                        new Vector3 (-Width/2,SillHeight,0)
            };

            if (State != OpeningState.PREVIEW) AttachClosestFaces();

            gameObject.AddComponent<PolyShape>();
            gameObject.AddComponent<ProBuilderMesh>();

            List<Vector3> openingMeshControlPoints = ControlPoints
                .Select(p => p -= Vector3.forward * (OpeningDepth / 2)).ToList();

            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(ControlPoints);
            polyshape.extrude = OpeningDepth;
            polyshape.CreateShapeFromPolygon();

            gameObject.GetComponent<ProBuilderMesh>().Refresh();
            gameObject.GetComponent<MeshRenderer>().material = material;

            InitRender2D();
            InitRender3D();
        }
        private void InitRender3D() {
            UpdateRender3D();
        }
        public void UpdateRender3D() {
        }
        private void InitRender2D() {
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lr.materials = Enumerable.Repeat(AssetUtil.LoadAsset<Material>("materials", "plan_opening"), lr.positionCount).ToArray();
            lr.sortingOrder = 2;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lr.sortingLayerName = "PLAN";
            UpdateRender2D();
        }
        public void UpdateRender2D(float width = 0.2f) {
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();

            // Reset line renderer and leave empty if wall visibility is false
            lr.positionCount = 0;
            if (!UI.Settings.ShowOpeningLines) return;

            // Set controlpoints
            lr.useWorldSpace = false;
            List<Vector3> points = GetControlPoints2D().Select(p =>
                p + Vector3.up * (RoomHeight + 0.001f)).ToList();
            lr.positionCount = points.Count;
            lr.SetPositions(points.ToArray());

            // Style
            lr.startWidth = width; lr.endWidth = width;
            foreach (Material material in lr.materials) {
                if (Shape == OpeningShape.DOOR) material.color = Color.gray;
                else material.color = Color.blue;
            }
        }

        public List<Vector3> GetControlPoints2D() {
            List<Vector3> controlPoints2D = new List<Vector3> {
                new Vector3 (  Width / 2, 0, 0 ),
                new Vector3 ( -Width / 2, 0, 0) };
            return controlPoints2D;
        }

        /// <summary>
        /// Deletes the opening
        /// </summary>
        public void Delete() {
            if (Faces != null && Faces.Count() > 0) {
                foreach (Face face in Faces) {
                    face.RemoveOpening(this);
                }
            }
            Destroy(gameObject);
        }
        /// <summary>
        /// Moves the opening to the given position
        /// </summary>
        public void Move(Vector3 exactPosition) {
            Vector3 gridPosition = Grid.GetNearestGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;
        }
        public void SubMove(Vector3 exactPosition) {
            Vector3 gridPosition = Grid.GetNearestSubGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;
        }
        public void Rotate(Face closestFace) {
            gameObject.transform.rotation = Quaternion.LookRotation(closestFace.Normal, Vector3.up);
        }

        public Vector3 ClosestPoint(Vector3 mousePos, Face closestFace) {
            (Vector3 vA, Vector3 vB) = closestFace.Get2DEndPoints();
            Vector3 closestPoint = VectorFunctions.LineClosestPoint(vA, vB, mousePos);

            return closestPoint;
        }

        public Interface GetCoincidentInterface() {
            float parameterOnFace = Faces[0].Line.Parameter(CenterPoint);
            return Faces[0].GetInterfaceAtParameter(parameterOnFace);
        }
        /// <summary>
        /// Gets a list of controlpoints - in local coordinates. The controlpoints are the vertices of the underlying polyshape of the opening.
        /// </summary>
        public List<Vector3> GetControlPoints(bool localCoordinates = false, bool closed = false, bool reverse = true) {
            List<Vector3> returnPoints = ControlPoints;
            if (closed) {
                returnPoints = ControlPoints.Concat(new List<Vector3> { ControlPoints[0] }).ToList();
            }
            if (!localCoordinates) {
                returnPoints = returnPoints.Select(p => gameObject.transform.TransformPoint(p)).ToList();
            }
            if (reverse) {
                returnPoints.Reverse();
            }
            return returnPoints;
        }

        public void AttachClosestFaces() {
            // Remove preexistent attachments
            if (Faces != null && Faces.Count > 0) {
                foreach (Face face in Faces) {
                    face.RemoveOpening(this);
                }
            }
            // Find the two closest faces in the building
            List<Face> closestFaces = new List<Face>();
            foreach (Room room in Building.Instance.Rooms) {
                foreach (Face face in room.Faces.Where(f => f.Orientation == Orientation.VERTICAL)) {
                    if (face.Line.IsOnLine(CenterPoint)) {
                        closestFaces.Add(face);
                    }
                }
            }
            // Add opening to faces
            foreach (Face face in closestFaces) {
                face.AddOpening(this);
            }
            // Add faces to opening
            Faces = closestFaces;
        }

    }
}
