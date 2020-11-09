using DesignPlatform.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;

namespace DesignPlatform.Core {
    public class OpeningMode : Mode {
        private static OpeningMode instance;
        private Collider[] collidingRooms;
        private float radius = 4f;
        public OpeningShape selectedShape { get; private set; } //0 is window and 1 is door
        public Opening previewOpening;
        public Vector3 closestPoint;
        public Vector3 hitPoint;


        public static OpeningMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new OpeningMode()); }
        }

        public OpeningMode() {
            selectedShape = OpeningShape.WINDOW;
        }

        public override void Tick() {
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject() == false) {
                    Build();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Main.Instance.SetMode(SelectMode.Instance);
            }

            if (previewOpening) { UpdatePreviewLocation(); }
        }

        /// <summary>
        /// Create preview opening 
        /// </summary> 
        public override void OnModeResume() {
                if (previewOpening == null) {
                previewOpening = Building.Instance.BuildOpening(openingShape: selectedShape, 
                                                                preview: true);
            }
        }

        /// <summary>
        /// Delete the preview opening object
        /// </summary> 
        public override void OnModePause() {
            previewOpening.Delete();
            previewOpening = null;
        }

        /// <summary>
        /// Build the opening gameObject
        /// </summary>        
        public void Build() {
            hitPoint = hitPointOnPlane();
            Face[] closestFaces = ClosestFace(hitPoint);
            Building.Instance.BuildOpening(openingShape: selectedShape,
                                           templateOpening: previewOpening,
                                           closestFaces: closestFaces);
        }

        /// <summary>
        /// Returns mouse hit point on plane if no hit zero-vector is returned
        /// </summary>
        public Vector3 hitPointOnPlane() {
            Plane basePlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
            float distance;
            hitPoint = Vector3.zero;
            if (basePlane.Raycast(ray, out distance)) {
                hitPoint = ray.GetPoint(distance);
            }
            return hitPoint;
        }

        /// <summary>
        /// Returns a collider array of colliding rooms for mouse position
        /// </summary> 
        public Collider[] MousePositionCollidingRooms(Vector3 hitPoint) {
            collidingRooms = Physics.OverlapSphere(hitPoint, radius);
            collidingRooms = collidingRooms.ToList().Where(c => c.gameObject.layer == 8).ToArray();
            if (collidingRooms.Count() == 0) { return collidingRooms = null; }
            return collidingRooms;
        }

        // Moves Preview opening with the mouse
        public void UpdatePreviewLocation() {
            hitPoint = hitPointOnPlane();
            if (MousePositionCollidingRooms(hitPoint) == null) {
                previewOpening.Move(hitPoint);
            }
            else {
                Face[] closestFace = ClosestFace(hitPoint);
                closestPoint = ClosestPoint(hitPoint, closestFace[0]);
                //previewOpening.SubMove(closestPoint);
                previewOpening.transform.position = closestPoint;
                previewOpening.Rotate(closestFace[0]);
            }
        }

        public void SetSelectedShape(OpeningShape shape = OpeningShape.WINDOW) {
            selectedShape = shape;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        public Face[] ClosestFace(Vector3 mousePos) {

            // Create list of rooms that might have the closest face
            List<Room> relevantRooms = new List<Room>();
            foreach (Collider c in MousePositionCollidingRooms(mousePos)) {
                Room r = c.GetComponentInParent<Room>();
                if (!relevantRooms.Contains(r)) {
                    relevantRooms.Add(r);
                }
            }

            // Find closest face for each room
            Face[] closestFace = new Face[relevantRooms.Count];
            float[] closestDistance = Enumerable.Repeat(float.PositiveInfinity, relevantRooms.Count).ToArray();
            for (int i = 0; i < relevantRooms.Count; i++) {
                List<Face> roomFaces = relevantRooms[i].Faces.Where(f => f.orientation == Orientation.VERTICAL).ToList();
                foreach (Face face in roomFaces) {
                    (Vector3 vA, Vector3 vB) = face.Get2DEndPoints();
                    Vector3 closestPoint = VectorFunctions.LineClosestPoint(vA, vB, mousePos);
                    float distance = (closestPoint - mousePos).magnitude;
                    if (distance <= closestDistance[i]) {
                        closestDistance[i] = distance;
                        closestFace[i] = face;
                    }
                }
            }

            // Return one or two closest faces
            if (relevantRooms.Count > 1 && closestDistance[0] == closestDistance[1]) {
                return new Face[] { closestFace[0], closestFace[1] };
            }
            else {
                return new Face[] {
                    closestFace[closestDistance.ToList().IndexOf(closestDistance.Min())]  };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="closestFace"></param>
        /// <returns></returns>
        public Vector3 ClosestPoint(Vector3 mousePos, Face closestFace) {
            (Vector3 vA, Vector3 vB) = closestFace.Get2DEndPoints();
            Vector3 closestPoint = VectorFunctions.LineClosestPoint(vA, vB, mousePos);

            return closestPoint;
        }
    }
}

