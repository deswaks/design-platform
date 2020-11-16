using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

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
            //Debug.Log("Opening Mode");
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject() == false) {
                    if (MousePositionCollidingRooms(hitPoint) != null) {
                        Build();
                    }
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
            //List<Interface> interfaces = Building.Instance.Interfaces;
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
            //hitPoint = hitPointOnPlane();
            //Face[] closestFaces = ClosestFace(hitPoint);

            previewOpening.SetAttachedFaces(previewOpening.transform.position);

            Building.Instance.BuildOpening(openingShape: selectedShape,
                                           templateOpening: previewOpening,
                                           attachedFaces: previewOpening.Faces);
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
                Face closestFace = ClosestFace(hitPoint);
                closestPoint = ClosestPoint(hitPoint, closestFace);
                previewOpening.SubMove(closestPoint);
                previewOpening.Rotate(closestFace);
                previewOpening.UpdateRender2D();
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
        public Face ClosestFace(Vector3 mousePos) {

            // Create list of rooms that might have the closest face
            List<Room> relevantRooms = new List<Room>();
            foreach (Collider c in MousePositionCollidingRooms(mousePos)) {
                Room r = c.GetComponentInParent<Room>();
                if (!relevantRooms.Contains(r)) {
                    relevantRooms.Add(r);
                }
            }

            // Find closest face
            Face closestFace = null;
            float closestDistance = float.PositiveInfinity;

            for (int i = 0; i < relevantRooms.Count; i++) {
                List<Face> roomFaces = relevantRooms[i].Faces.Where(f => f.Orientation == Orientation.VERTICAL).ToList();

                foreach (Face face in roomFaces) {

                    (Vector3 vA, Vector3 vB) = face.Get2DEndPoints();
                    Vector3 closestPoint = VectorFunctions.LineClosestPoint(vA, vB, mousePos);

                    float distance = (closestPoint - mousePos).magnitude;
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        closestFace = face;
                    }
                }
            }
            return closestFace;
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

