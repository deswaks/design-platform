﻿using DesignPlatform.Core;
using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.Modes {
    public class OpeningMode : Mode {
        private static OpeningMode instance;
        private Collider[] collidingSpaces;
        private float radius = 4f;
        private OpeningFunction selectedShape = OpeningFunction.DOOR;

        public OpeningFunction SelectedShape {
            get {
                return selectedShape;
            }
            set {
                selectedShape = value;
                RebuildPreview(selectedShape);
            }
        }
        public Opening previewOpening;
        public Vector3 closestPoint;
        public Vector3 hitPoint;


        public static OpeningMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new OpeningMode()); }
        }

        public OpeningMode() {
            SelectedShape = OpeningFunction.WINDOW;
        }

        public override void Tick() {
            //Debug.Log("Opening Mode");
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject() == false) {
                    if (MousePositionCollidingSpaces(hitPoint) != null) {
                        Build();
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                if ((int)SelectedShape == 1) {
                    SelectedShape = 0;
                }
                else {
                    SelectedShape = (OpeningFunction)(int)SelectedShape + 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.D)) {
                SelectedShape = OpeningFunction.DOOR;
            }

            if (Input.GetKeyDown(KeyCode.W)) {
                SelectedShape = OpeningFunction.WINDOW;
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
                previewOpening = Building.Instance.BuildOpening(function: SelectedShape,
                                                                preview: true);
            }
        }

        /// <summary>
        /// Delete the preview opening object
        /// </summary> 
        public override void OnModePause() {
            if (previewOpening != null) previewOpening.Delete();
            previewOpening = null;
        }

        /// <summary>S
        /// Build the opening gameObject
        /// </summary>        
        public void Build() {
            Opening newOpening = Building.Instance.BuildOpening(function: SelectedShape,
                                           templateOpening: previewOpening);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RebuildPreview(OpeningFunction buildShape) {
            if (previewOpening != null) previewOpening.Delete();
            previewOpening = Building.Instance.BuildOpening(function: buildShape,
                                                            preview: true);
        }

        /// <summary>
        /// Returns mouse hit point on plane if no hit zero-vector is returned
        /// </summary>
        public Vector3 HitPointOnPlane() {
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
        /// Returns a collider array of colliding spaces for mouse position
        /// </summary> 
        public Collider[] MousePositionCollidingSpaces(Vector3 hitPoint) {
            collidingSpaces = Physics.OverlapSphere(hitPoint, radius);
            collidingSpaces = collidingSpaces.ToList().Where(c => c.gameObject.layer == 8).ToArray();
            if (collidingSpaces.Count() == 0) { return collidingSpaces = null; }
            return collidingSpaces;
        }

        // Moves Preview opening with the mouse
        public void UpdatePreviewLocation() {
            hitPoint = HitPointOnPlane();
            if (MousePositionCollidingSpaces(hitPoint) == null) {
                previewOpening.Move(hitPoint);
            }
            else {
                Face closestFace = ClosestFace(hitPoint);
                closestPoint = closestFace.Line.ClosestPoint(hitPoint);
                previewOpening.SubMove(closestPoint);
                previewOpening.Rotate(closestFace);
                previewOpening.UpdateRender2D();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        public Face ClosestFace(Vector3 mousePos) {

            // Create list of spaces that might have the closest face
            List<Core.Space> relevantRooms = new List<Core.Space>();
            foreach (Collider c in MousePositionCollidingSpaces(mousePos)) {
                Core.Space r = c.GetComponentInParent<Core.Space>();
                if (!relevantRooms.Contains(r)) {
                    relevantRooms.Add(r);
                }
            }

            // Find closest face
            Face closestFace = null;
            float closestDistance = float.PositiveInfinity;

            for (int i = 0; i < relevantRooms.Count; i++) {
                List<Face> spaceFaces = relevantRooms[i].Faces.Where(f => f.Orientation == Orientation.VERTICAL).ToList();

                foreach (Face face in spaceFaces) {
                    Vector3 closestPoint = face.Line.ClosestPoint(mousePos);
                    float distance = (closestPoint - mousePos).magnitude;
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        closestFace = face;
                    }
                }
            }
            return closestFace;
        }
    }
}

