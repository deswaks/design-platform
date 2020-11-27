using DesignPlatform.Database;
using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {

    /// <summary>
    /// Describes the function of an opening.
    /// </summary>
    public enum OpeningFunction {
        /// <summary> Opening is a door </summary>
        DOOR,
        /// <summary> Opening is a window </summary>
        WINDOW
    }

    /// <summary>
    /// Describes the state of an opening.
    /// </summary>
    public enum OpeningState {
        /// <summary> Opening has been placed in the model. </summary>
        PLACED,
        /// <summary> Opening is used for preview and has not been placed in the model. </summary>
        PREVIEW
    }

    /// <summary>
    /// Represents an opening in the walls of a building
    /// This includes external building openings such as windows and
    /// internal building openings such as doors between rooms.
    /// </summary>
    public class Opening : MonoBehaviour {

        /// <summary>The width of the opening hole.</summary>
        public float Width { get; private set; }
        /// <summary>The height of the opening hole.</summary>
        public float Height { get; private set; }
        /// <summary>The thickness of the opening element.</summary>
        public float Thickness { get; private set; }
        /// <summary>The height of the wall below the opening hole.</summary>
        public float SillHeight { get; private set; }
        /// <summary>The function of the opening in the building (eg. door or window).</summary>
        public OpeningFunction Function { get; private set; }
        /// <summary>Describes whether the opening has been placed in the model.</summary>
        public OpeningState State { get; private set; }
        /// <summary>The local controlpoints that are the vertices for the polyline around the opening.</summary>
        public List<Vector3> ControlPoints { get; private set; }
        /// The material of the rendered object.
        public Material Material { get; private set; }

        /// <summary>The space(s) adjacent to this opening.</summary>
        public List<Space> Spaces {
            get {
                if (Faces != null && Faces.Count() > 0) {
                    return Faces.Select(f => f.Space).ToList();
                }
                else return new List<Space>();
            }
        }

        /// <summary>The face(s) coincident with this opening.</summary>
        public List<Face> Faces { get; private set; }

        /// <summary>The parameters where this opening is located on its coincident face(s)</summary>
        public List<float> Parameters {
            get { return Faces.Select(f => f.OpeningParameters[this]).ToList(); }
        }

        /// <summary>The interface coincident with this opening.</summary>
        public Interface Interface {
            get { return Faces[0].GetInterfaceAtParameter(Parameters[0]); }
        }

        /// <summary>The maximum height of the space(s) adjacent to this opening.</summary>
        public float SpaceHeight {
            get {
                if (Faces != null && Faces.Count > 0) return Spaces.Max(s => s.height);
                else return 3.0f;
            }
        }

        /// <summary>
        /// The placement point of the opening on the wall (bottom) placement line.
        /// </summary>
        public Vector3 LocationPoint {
            get { return gameObject.transform.position; }
        }


        public void InitializeOpening(OpeningFunction function = OpeningFunction.WINDOW,
                                      OpeningState state = OpeningState.PREVIEW) {
            Function = function;
            State = state;

            switch (Function) {
                case OpeningFunction.WINDOW:
                    gameObject.layer = 16; // Window layer
                    Width = Settings.WindowWidth;
                    Height = Settings.WindowHeight;
                    SillHeight = Settings.WindowSillHeight;
                    Material = AssetUtil.LoadAsset<Material>("materials", "openingWindow");
                    gameObject.name = "Window";

                    break;
                case OpeningFunction.DOOR:
                    gameObject.layer = 15; // Door layer
                    Width = Settings.DoorWidth;
                    Height = Settings.DoorHeight;
                    SillHeight = Settings.DoorSillHeight;
                    Material = AssetUtil.LoadAsset<Material>("materials", "openingDoor");
                    gameObject.name = "Door";
                    break;
            }
            if (State == OpeningState.PREVIEW) gameObject.name = "Preview " + gameObject.name;
            ControlPoints = new List<Vector3> {
                        new Vector3 (Width/2,SillHeight,0),
                        new Vector3 (Width/2,SillHeight+Height,0),
                        new Vector3 (-Width/2,SillHeight+Height,0),
                        new Vector3 (-Width/2,SillHeight,0)
            };

            if (State != OpeningState.PREVIEW) AttachClosestFaces();

            InitRender2D();
            InitRender3D();
        }
        /// <summary>Creates the plan view render of the opening object</summary>
        private void InitRender3D() {
            gameObject.AddComponent<PolyShape>();
            gameObject.AddComponent<ProBuilderMesh>();

            UpdateRender3D();
        }
        /// <summary>Updates the POV view render of the opening object</summary>
        public void UpdateRender3D() {
            List<Vector3> openingMeshControlPoints = ControlPoints
                .Select(p => p -= Vector3.forward * (Thickness / 2)).ToList();
            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(ControlPoints);
            polyshape.extrude = Thickness;
            polyshape.CreateShapeFromPolygon();
            gameObject.GetComponent<ProBuilderMesh>().Refresh();
            gameObject.GetComponent<MeshRenderer>().material = Material;
        }
        /// <summary>Creates the plan view render of the opening object</summary>
        private void InitRender2D() {
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lr.materials = Enumerable.Repeat(AssetUtil.LoadAsset<Material>("materials", "plan_opening"), lr.positionCount).ToArray();
            lr.sortingOrder = 2;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lr.sortingLayerName = "PLAN";
            UpdateRender2D();
        }
        /// <summary>Updates the plan view render of the opening object</summary>
        /// <param name="width">width of the drawn line in model units.</param>
        public void UpdateRender2D(float width = 0.2f) {
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();

            // Reset line renderer and leave empty if wall visibility is false
            lr.positionCount = 0;
            if (!UI.Settings.ShowOpeningLines) return;

            // Set controlpoints
            lr.useWorldSpace = false;
            List<Vector3> points = new List<Vector3> {
                new Vector3 (  Width / 2, SpaceHeight + 0.001f, 0 ),
                new Vector3 ( -Width / 2, SpaceHeight + 0.001f, 0)
            };
            lr.positionCount = points.Count;
            lr.SetPositions(points.ToArray());

            // Style
            lr.startWidth = width; lr.endWidth = width;
            if (Function == OpeningFunction.DOOR) lr.startColor = lr.endColor = Color.gray;
            else lr.startColor = lr.endColor = new Color(60f / 256f, 124f / 256f, 209f / 256f);
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
        /// Moves the opening to the closest gridpoint to the given position.
        /// </summary>
        /// <param name="exactPosition">The location at which the opening should be moved to.</param>
        /// <param name="useSubgrid">Decides whether the subgrid or main grid should be used.</param>
        public void Move(Vector3 exactPosition, bool useSubgrid = true) {
            Vector3 gridPosition;
            if (useSubgrid) gridPosition = Grid.GetNearestSubGridpoint(exactPosition);
            else gridPosition = Grid.GetNearestGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;
        }

        /// <summary>
        /// Rotates the opening to match the orientation of the given face.
        /// </summary>
        /// <param name="closestFace">Face for which to match rotation.</param>
        public void Rotate(Face closestFace) {
            gameObject.transform.rotation = Quaternion.LookRotation(closestFace.Normal, Vector3.up);
        }

        /// <summary>
        /// Gets a list of controlpoints. The controlpoints are the vertices of the underlying polyshape of the opening.
        /// </summary>
        /// <param name="localCoordinates">Decides whether the controlpoints should be given in local coordinate</param>
        /// <param name="closed">Decides whether the last point should be given in the end of the list as well.</param>
        /// <returns></returns>
        public List<Vector3> GetControlPoints(bool localCoordinates = false, bool closed = false) {
            List<Vector3> returnPoints = ControlPoints;
            if (closed) {
                returnPoints = ControlPoints.Concat(new List<Vector3> { ControlPoints[0] }).ToList();
            }
            if (!localCoordinates) {
                returnPoints = returnPoints.Select(p => gameObject.transform.TransformPoint(p)).ToList();
            }
            return returnPoints;
        }

        /// <summary>
        /// Adds the closest faces to the list of adjacent faces to this opening.
        /// </summary>
        public void AttachClosestFaces() {
            // Remove preexistent attachments
            if (Faces != null && Faces.Count > 0) {
                foreach (Face face in Faces) {
                    face.RemoveOpening(this);
                }
            }
            // Find the two closest faces in the building
            List<Face> closestFaces = new List<Face>();
            foreach (Space space in Building.Instance.Spaces) {
                foreach (Face face in space.Faces.Where(f => f.Orientation == Orientation.VERTICAL)) {
                    if (face.Line.Intersects(LocationPoint)) {
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