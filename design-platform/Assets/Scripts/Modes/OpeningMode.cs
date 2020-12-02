using DesignPlatform.Core;
using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.Modes {

    /// <summary>
    /// The mode wherein new openings may be built.
    /// </summary>
    public class OpeningMode : Mode {

        /// <summary>The opening object used for preview purposes.</summary>
        public Opening PreviewOpening { get; private set; }

        /// <summary>Current selected shape to give new openings.</summary>
        public OpeningFunction SelectedFunction {
            get { return selectedFunction; }
            set { selectedFunction = value; RebuildPreview(selectedFunction); }
        }

        /// <summary>The single instance that exists of this singleton class.</summary>
        public static OpeningMode Instance {
            get { return instance ?? (instance = new OpeningMode()); }
        }

        /// <summary>The single instance that exists of this singleton class.</summary>
        private static OpeningMode instance;

        /// <summary>Current selected shape to give new openings.</summary>
        private OpeningFunction selectedFunction = OpeningFunction.DOOR;



        /// <summary>Default constructor.</summary>
        public OpeningMode() {
            SelectedFunction = OpeningFunction.WINDOW;
        }



        /// <summary>Defines the actions to take at every frame where this mode is active.</summary>
        public override void Tick() {
            //Debug.Log("Opening Mode");
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject() == false) {
                    if (NearbySpaces(MousePositionOnPlane(), Settings.FaceSearchDistance) != null) {
                        Build();
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                if ((int)SelectedFunction == 1) {
                    SelectedFunction = 0;
                }
                else {
                    SelectedFunction = (OpeningFunction)(int)SelectedFunction + 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.D)) {
                SelectedFunction = OpeningFunction.DOOR;
            }

            if (Input.GetKeyDown(KeyCode.W)) {
                SelectedFunction = OpeningFunction.WINDOW;
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Main.Instance.SetMode(SelectMode.Instance);
            }

            if (PreviewOpening) { UpdatePreviewLocation(); }
        }

        /// <summary>Defines the actions to take when changing into this mode.</summary>
        public override void OnModeResume() {
            //List<Interface> interfaces = Building.Instance.Interfaces;
            if (PreviewOpening == null) {
                PreviewOpening = Building.Instance.BuildOpening(function: SelectedFunction,
                                                                preview: true);
            }
        }

        /// <summary>Defines the actions to take when changing out of this mode.</summary>
        public override void OnModePause() {
            if (PreviewOpening != null) PreviewOpening.Delete();
            PreviewOpening = null;
        }

        /// <summary>Build the preview opening into a real opening with gameObject.</summary>        
        public void Build() {
            Building.Instance.BuildOpening(function: SelectedFunction, templateOpening: PreviewOpening);
        }

        /// <summary>Rebuild the preview opening.</summary>
        public void RebuildPreview(OpeningFunction buildShape) {
            if (PreviewOpening != null) PreviewOpening.Delete();
            PreviewOpening = Building.Instance.BuildOpening(function: buildShape,
                                                            preview: true);
        }

        /// <summary>Returns mouse position point on the build plane</summary>
        public Vector3 MousePositionOnPlane() {
            Plane basePlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            Vector3 hitPoint = Vector3.zero;
            if (basePlane.Raycast(ray, out distance)) {
                hitPoint = ray.GetPoint(distance);
            }
            return hitPoint;
        }

        /// <summary>
        /// Finds the spaces that are within a specific distance from a point.
        /// </summary>
        /// <param name="location">Point to search from.</param>
        /// <param name="searchDistance">Distance to search from the point</param>
        /// <returns>a collider array of colliders at the point lcoation.</returns>
        public List<Core.Space> NearbySpaces(Vector3 location, float searchDistance) {
            Collider[] nearbySpaceColliderObjects = Physics.OverlapSphere(location, searchDistance)
                .Where(c => c.gameObject.layer == 8).ToArray();
            if (nearbySpaceColliderObjects.Count() == 0) return null;
            return nearbySpaceColliderObjects.Select(c => c.GetComponentInParent<Core.Space>()).ToList();
        }

        /// <summary>
        /// Moves Preview opening with the mouse
        /// </summary>
        public void UpdatePreviewLocation() {
            Vector3 hitPoint = MousePositionOnPlane();
            if (NearbySpaces(hitPoint, Settings.FaceSearchDistance) == null) {
                PreviewOpening.Move(hitPoint, useSubgrid: false);
            }
            else {
                Face closestFace = ClosestFace(hitPoint);
                Vector3 closestPoint = closestFace.LocationLine.ClosestPoint(hitPoint);
                PreviewOpening.Move(closestPoint);
                PreviewOpening.Rotate(closestFace);
                PreviewOpening.UpdateRender2D();
            }
        }

        /// <summary>
        /// Finds the face closest to the given point.
        /// </summary>
        /// <param name="mousePos">Point to search from.</param>
        /// <returns>Face closest to point.</returns>
        public Face ClosestFace(Vector3 mousePos) {

            // Create list of spaces that might have the closest face
            List<Core.Space> relevantRooms = new List<Core.Space>();
            foreach (Core.Space s in NearbySpaces(mousePos, Settings.FaceSearchDistance)) {
                if (!relevantRooms.Contains(s)) {
                    relevantRooms.Add(s);
                }
            }

            // Find closest face
            Face closestFace = null;
            float closestDistance = float.PositiveInfinity;

            for (int i = 0; i < relevantRooms.Count; i++) {
                List<Face> spaceFaces = relevantRooms[i].Faces.Where(f => f.Orientation == Orientation.VERTICAL).ToList();

                foreach (Face face in spaceFaces) {
                    Vector3 closestPoint = face.LocationLine.ClosestPoint(mousePos);
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

