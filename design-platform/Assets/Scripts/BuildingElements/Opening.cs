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

    public class Opening : MonoBehaviour {

        public List<Room> Rooms {
            get { return Faces.Select(f => f.Room).ToList(); }
            set {; }
        }
        public List<Face> Faces {
            get { return Faces; }
            set {; }
        }

        public float WindowWidth = 1.6f;
        public float WindowHeight = 1.2f;

        public float DoorWidth = 1f;
        public float DoorHeight = 2.4f;
        public float Doorstep = 0.1f;

        public float OpeningDepth = 0.051f;
        public float SillHeight;
        public float Width;
        public float Height;

        public Material previewMaterial;
        public Material windowMaterial;
        public Material doorMaterial;

        public List<Opening> openings { get; private set; }
        public Face[] attachedFaces = new Face[2];
        private OpeningShape shape;
        private Opening prefabOpening;
        public List<Vector3> controlPoints;

        public enum OpeningStates {
            PLACED,
            PREVIEW
        }
        private OpeningStates openingState;

        public void InitializeOpening(Face[] attachedFaces = null,
                                      OpeningShape openingShape = OpeningShape.WINDOW) {
            shape = openingShape;
            this.attachedFaces = attachedFaces;

            openingState = OpeningStates.PREVIEW;

            GameObject prefabObject = AssetUtil.LoadAsset<GameObject>("prefabs", "OpeningPrefab");

            prefabOpening = (Opening)prefabObject.GetComponent(typeof(Opening));
            Material material = prefabOpening.previewMaterial;

            switch (shape) {
                case OpeningShape.WINDOW:
                    gameObject.layer = 16; // Window layer
                    Width = WindowWidth;
                    Height = WindowHeight;
                    SillHeight = 1.1f;
                    material = prefabOpening.windowMaterial;
                    gameObject.name = "Window";


                    break;
                case OpeningShape.DOOR:
                    gameObject.layer = 15; // Door layer
                    Width = DoorWidth;
                    Height = DoorHeight;
                    SillHeight = Doorstep;
                    material = prefabOpening.doorMaterial;
                    gameObject.name = "Door";
                    break;
            }
            controlPoints = new List<Vector3> {
                        new Vector3 (Width/2,SillHeight,0),
                        new Vector3 (Width/2,SillHeight+Height,0),
                        new Vector3 (-Width/2,SillHeight+Height,0),
                        new Vector3 (-Width/2,SillHeight,0)
                    };

            gameObject.AddComponent<PolyShape>();
            gameObject.AddComponent<ProBuilderMesh>();

            List<Vector3> openingMeshControlPoints = controlPoints.Select(p => p -= Vector3.forward * (OpeningDepth / 2)).ToList();


            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(controlPoints);
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
            UpdateRender2D();
        }
        public void UpdateRender2D(float width = 0.2f) {
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();

            // Reset line renderer and leave empty if wall visibility is false
            lr.positionCount = 0;
            if (!GlobalSettings.ShowOpeningLines) return;

            // Set controlpoints
            lr.useWorldSpace = false;
            float height = 3.001f;
            if (attachedFaces != null) height = attachedFaces[0].Room.height + 0.001f;
            List<Vector3> points = GetControlPoints2D().Select(p =>
                p + Vector3.up * (height)).ToList();
            lr.positionCount = points.Count;
            lr.SetPositions(points.ToArray());

            // Style
            lr.startWidth = width; lr.endWidth = width;
            foreach (Material material in lr.materials) {
                if (shape == OpeningShape.DOOR) material.color = Color.gray;
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
            if (Building.Instance.Openings.Contains(this)) {
                Building.Instance.RemoveOpening(this);
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
            Vector3 faceNormal = closestFace.parentRoom.GetWallNormals()[closestFace.faceIndex];
            gameObject.transform.rotation = Quaternion.LookRotation(faceNormal,Vector3.up);
        }

        public void SetOpeningState(OpeningStates openingState) {
            this.openingState = openingState;
        }


        public Vector3 ClosestPoint(Vector3 mousePos, Face closestFace) {
            (Vector3 vA, Vector3 vB) = closestFace.Get2DEndPoints();
            Vector3 closestPoint = VectorFunctions.LineClosestPoint(vA, vB, mousePos);

            return closestPoint;
        }

        public Interface GetCoincidentInterface() {
            Vector3 openingPoint = gameObject.transform.position;
            float parameterOnFace = attachedFaces[0].GetPointParameter(openingPoint);

            return attachedFaces[0].GetInterfaceAtParameter(parameterOnFace);
        }
        /// <summary>
        /// Gets a list of controlpoints - in local coordinates. The controlpoints are the vertices of the underlying polyshape of the opening.
        /// </summary>
        public List<Vector3> GetControlPoints(bool localCoordinates = false, bool closed = false, bool reverse = true) {
            List<Vector3> returnPoints = controlPoints;
            if (closed) {
                returnPoints = controlPoints.Concat(new List<Vector3> { controlPoints[0] }).ToList();
            }
            if (!localCoordinates) {
                returnPoints = returnPoints.Select(p => gameObject.transform.TransformPoint(p)).ToList();
            }
            if (reverse) {
                returnPoints.Reverse();
            }
            return returnPoints;
        }
        public Face[] SetAttachedFaces(Vector3 openingPos) {
            // Find the two closest faces in the building
            List<Face> facesToAttach = new List<Face>();
            foreach (Room room in Building.Instance.Rooms) {
                foreach (Face face in room.Faces.Where(f => f.orientation == Orientation.VERTICAL)) {
                    if (face.IsPointOnFace(gameObject.transform.position)) {
                        facesToAttach.Add(face);
                    }
                }
            }
            attachedFaces = facesToAttach.ToArray();
            return attachedFaces;
        }

    }
}
