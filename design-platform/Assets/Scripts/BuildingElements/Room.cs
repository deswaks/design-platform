using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {

    public enum RoomState {
        STATIONARY,
        PREVIEW,
        MOVING
    }

    public enum RoomType {
        //Til senere implementering
        DELETED = -3,
        HIDDEN = -2,
        SELECTED = -1,
        //------------------
        PREVIEW = 0,
        DEFAULT = 1,
        SINGLEROOM = 10,
        DOUBLEROOM = 11,
        LIVINGROOM = 12,
        KITCHEN = 13,
        BATHROOM = 14
    }

    public enum RoomShape {
        RECTANGLE,
        LSHAPE,
        USHAPE,
        SSHAPE,
        TSHAPE
    }

    public class Room : MonoBehaviour {

        public Building ParentBuilding { get; private set; }
        public RoomShape Shape { get; private set; }
        public RoomType Type { get; private set; }
        public RoomState State { get; set; }

        private List<Vector3> controlPoints;
        public float height = 3.0f;
        public List<Face> Faces { get; private set; }

        private Material currentMaterial;
        public Material highlightMaterial;
        public string customProperty;


        private readonly Dictionary<RoomType, string> RoomMaterialAsset = new Dictionary<RoomType, string> {
            { RoomType.PREVIEW,  "RoomDefault"},
            { RoomType.DEFAULT,  "RoomDefault"},
            { RoomType.SELECTED, "RoomHighlight" },
            { RoomType.SINGLEROOM,  "RoomSingleroom"},
            { RoomType.DOUBLEROOM,  "RoomDoubleroom"},
            { RoomType.LIVINGROOM,  "RoomLivingroom"},
            { RoomType.KITCHEN,  "RoomKitchen"},
            { RoomType.BATHROOM,  "RoomBathroom"},
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildShape"></param>
        /// <param name="building"></param>
        public void InitRoom(RoomShape buildShape = RoomShape.RECTANGLE, Building building = null) {
            // Set constant values
            ParentBuilding = building;
            gameObject.layer = 8;       // 8 = Rooom layer
            Shape = buildShape;
            State = RoomState.PREVIEW;
            Type = RoomType.DEFAULT;
            currentMaterial = AssetUtil.LoadAsset<Material>("materials", RoomMaterialAsset[Type]);
            highlightMaterial = AssetUtil.LoadAsset<Material>("materials", RoomMaterialAsset[RoomType.SELECTED]);

            // Controlpoints
            switch (Shape) {
                case RoomShape.RECTANGLE:
                    controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                   new Vector3(0, 0, 3),
                                                   new Vector3(3, 0, 3),
                                                   new Vector3(3, 0, 0)};
                    gameObject.name = "Room(Rectangle)";
                    break;
                case RoomShape.LSHAPE:
                    controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 0)};
                    gameObject.name = "Room(L-Shape)";
                    break;
                case RoomShape.USHAPE:
                    controlPoints = new List<Vector3> {   new Vector3(0, 0, 0),
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 5),
                                                          new Vector3(8, 0, 5),
                                                          new Vector3(8, 0, 0)};
                    gameObject.name = "Room(U-Shape)"; 
                    break;       
                case RoomShape.SSHAPE:
                    controlPoints = new List<Vector3> {   new Vector3(0, 0, 0),
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(6, 0, 3),
                                                          new Vector3(6, 0, -2),
                                                          new Vector3(3, 0, -2),
                                                          new Vector3(3, 0, 0),
                    };
                    gameObject.name = "Room(S-Shape)"; 
                    break;
                case RoomShape.TSHAPE:
                    controlPoints = new List<Vector3> {   new Vector3(0, 0, 0),
                                                          new Vector3(-2, 0, 0),
                                                          new Vector3(-2, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 0),
                                                          new Vector3(3, 0, 0),
                                                          new Vector3(3, 0, -3),
                                                          new Vector3(0, 0, -3),
                    };
                    gameObject.name = "Room(T-Shape)"; 
                    break;
            }
            if (Type == RoomType.PREVIEW) controlPoints = controlPoints.Select(p => p + Vector3.up * (5f)).ToList();

            InitFaces();
            InitRender2D();
            InitRender3D();
        }

        private void InitFaces() {
            Faces = new List<Face>();
            for (int i = 0; i < controlPoints.Count + 2; i++) {
                Faces.Add(new Face(this, i));
            }
        }
        private void InitRender3D() {
            gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent<PolyShape>();
            gameObject.AddComponent<ProBuilderMesh>();
            SetRoomType(RoomType.DEFAULT);
            UpdateRender3D();
        }

        public void UpdateRender3D() {
            // Mesh
            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(controlPoints);
            polyshape.extrude = height;
            polyshape.CreateShapeFromPolygon();
            gameObject.GetComponent<ProBuilderMesh>().Refresh();
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;

            // Collider
            RoomCollider.GiveCollider(this);
        }
        private void InitRender2D() {
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lr.loop = true;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            UpdateRender2D();
        }
        public void UpdateRender2D(bool highlighted = false, bool colliding = false) {
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();
            lr.positionCount = 0;

            // Set controlpoints
            lr.useWorldSpace = false;
            List<Vector3> points = GetControlPoints(localCoordinates: true).Select(p => p + Vector3.up*(height+0.001f)).ToList();
            lr.positionCount = points.Count;
            lr.SetPositions(points.ToArray());

            // Style
            lr.materials = Enumerable.Repeat(AssetUtil.LoadAsset<Material>("materials", "wall2D"), lr.positionCount).ToArray();
            float width = 0.2f;
            Color color = Color.black;
            lr.sortingOrder = 0;
            if (highlighted) { lr.sortingOrder = 1;  width = 0.3f;  color = Color.yellow; }
            if (colliding) { lr.sortingOrder = 1; color = Color.red; }

            lr.startWidth = width; lr.endWidth = width;
            foreach (Material material in lr.materials) {
                material.color = color;
            }
        }

        /// <summary>
        /// Rotates the room. Defaults to 90 degree increments
        /// </summary>
        public void Rotate(bool clockwise = true, float degrees = 90) {
            if (!clockwise) { degrees = -degrees; }

            List<float> bounds = Bounds(localCoordinates: true);
            float width = bounds[1] - bounds[0];
            float height = bounds[3] - bounds[2];

            Vector3 centerPoint = new Vector3(bounds[0] + width / 2,
                                              0,
                                              bounds[2] + height / 2);
            centerPoint = gameObject.transform.TransformPoint(centerPoint);

            // if width and height are not BOTH divisible or undivisible by double grid
            if (width % (Grid.size * 2) == 0 && height % (Grid.size * 2) != 0
             || width % (Grid.size * 2) != 0 && height % (Grid.size * 2) == 0) {
                centerPoint = Grid.GetNearestGridpoint(centerPoint);
            }

            // Rotation
            gameObject.transform.RotateAround(
                point: centerPoint,
                axis: new Vector3(0, 1, 0),
                angle: degrees);

            UpdateRender3D();
        }

        /// <summary>
        /// Calculates the (brutto) floor area of the room including half the walls.
        /// </summary>
        /// <returns>float area</returns>
        public float GetFloorArea() {
            Vector2[] vertices = GetControlPoints(localCoordinates: true).Select(p => new Vector2(p.x, p.z)).ToArray();
            Polygon2D roomPolygon = new Polygon2D(vertices);
            return roomPolygon.GetArea();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<List<Vector3>> GetSurfaceVertices() {
            List<List<Vector3>> surfacesVertices = new List<List<Vector3>>();
            List<Vector3> vertices;

            // Add Floor surface
            vertices = GetControlPoints().Select(p => new Vector3(p.x, p.y, p.z)).ToList();
            surfacesVertices.Add(vertices);

            // Add wall surfaces
            int j = controlPoints.Count - 1;
            for (int i = 0; i < controlPoints.Count; i++) {
                vertices = new List<Vector3> {
                    controlPoints[i],
                    controlPoints[i] + new Vector3(0, height, 0),
                    controlPoints[j] + new Vector3(0, height, 0),
                    controlPoints[j]
                };
                surfacesVertices.Add(vertices);
                j = i;
            }

            // Add Ceiling vertices
            vertices = GetControlPoints().Select(p => new Vector3(p.x, p.y + height, p.z)).ToList();
            surfacesVertices.Add(vertices);

            return surfacesVertices;
        }

        /// <summary>
        /// Calculates the (brutto) floor area of the room including half the walls.
        /// </summary>
        /// <returns>float area</returns>
        public float GetVolume() {
            return GetFloorArea() * height;
        }

        /// <summary>
        /// Deletes the room
        /// </summary>
        public void Delete() {
            if (Building.Instance.rooms.Contains(this)) {
                ParentBuilding.RemoveRoom(this);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Moves the room to the given position
        /// </summary>
        public void Move(Vector3 exactPosition) {
            Vector3 gridPosition = Grid.GetNearestGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;
        }

        /// <summary>
        /// Gets a list of controlpoints - in local coordinates. The controlpoints are the vertices of the underlying polyshape of the building.
        /// </summary>
        public List<Vector3> GetControlPoints(bool localCoordinates = false, bool closed = false) {
            List<Vector3> returnPoints = controlPoints;
            if (closed) {
                returnPoints = controlPoints.Concat(new List<Vector3> { controlPoints[0] }).ToList();
            }
            if (!localCoordinates) {
                returnPoints = returnPoints.Select(p => gameObject.transform.TransformPoint(p)).ToList();
            }
            return returnPoints;
        }
        public void SetControlPoints(List<Vector3> newControlPoints) {
            controlPoints = newControlPoints;
            UpdateRender3D();
        }

        /// <summary>
        /// Gets a list of controlpoints. The controlpoints are the vertices of the underlying polyshape of the building.
        /// </summary>
        public List<Vector3> GetWallMidpoints(bool localCoordinates = false) {
            List<Vector3> midPoints = new List<Vector3>();
            List<Vector3> circularControlpoints = GetControlPoints(localCoordinates: localCoordinates, closed: true);
            for (int i = 0; i < controlPoints.Count; i++) {
                midPoints.Add((circularControlpoints[i] + circularControlpoints[i + 1]) / 2);
            }
            return midPoints;
        }

        /// <summary>
        /// Gets a list of normals. They are in same order as controlpoints (clockwise). localCoordinates : true (for local coordinates, Sherlock)
        /// </summary>
        public List<Vector3> GetWallNormals(bool localCoordinates = false) {
            List<Vector3> wallNormals = new List<Vector3>();
            List<Vector3> circularControlpoints = GetControlPoints(localCoordinates: localCoordinates, closed: true);
            for (int i = 0; i < controlPoints.Count; i++) {
                wallNormals.Add(Vector3.Cross(circularControlpoints[i + 1] - circularControlpoints[i], Vector3.up).normalized);
            }
            return wallNormals;
        }

        /// <summary>
        /// Takes the index of the wall to extrude and the distance to extrude     
        /// </summary>
        public void ExtrudeWall(int wallToExtrude, float extrusion) {
            // Create move vector for extrusion
            Vector3 localExtrusion = GetWallNormals(localCoordinates: true)[wallToExtrude] * extrusion;

            // Clone points
            List<Vector3> controlPointsClone = GetControlPoints(localCoordinates: true).Select(
                v => new Vector3(v.x, v.y, v.z)).ToList();

            // Extrude the cloned points
            int point1Index = IndexingUtils.WrapIndex(wallToExtrude, controlPoints.Count);
            int point2Index = IndexingUtils.WrapIndex(wallToExtrude + 1, controlPoints.Count);
            controlPointsClone[point1Index] += localExtrusion;
            controlPointsClone[point2Index] += localExtrusion;

            // Compare normals before and after extrusion element-wise 
            List<Vector3> normals = GetWallNormals(localCoordinates: true);
            List<Vector3> extrudedNormals = PolygonUtils.PolygonNormals(controlPointsClone);
            bool normalsAreIdentical = true;
            for (int i = 0; i < normals.Count; i++) {
                if (normals[i] != extrudedNormals[i]) {
                    normalsAreIdentical = false;
                }
            }

            // If normals did not change: Make extruded points the real control points 
            if (normalsAreIdentical) {
                controlPoints = controlPointsClone;
                UpdateRender3D();
                UpdateRender2D();
            }

        }

        /// <summary>
        /// Resets the origin of the gameObject such that it is at the first controlpoint.
        /// </summary>
        public void ResetOrigin() {
            if (GetControlPoints()[0] != gameObject.transform.position) {
                Vector3 originCP = GetControlPoints(localCoordinates: true)[0];
                Vector3 originGO = gameObject.transform.position;
                Vector3 difference = new Vector3(originCP.x - originGO.x,
                                                 0,
                                                 originCP.z - originGO.z);
                gameObject.transform.Translate(difference);
                for (int i = 0; i < controlPoints.Count; i++) {
                    controlPoints[i] -= difference;
                }
                UpdateRender3D();
                UpdateRender2D();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTagLocation(bool localCoordinates = false) {
            Vector3 tagPoint = new Vector3();
            List<Vector3> cp = GetControlPoints(localCoordinates: localCoordinates);

            switch (Shape) {
                case RoomShape.RECTANGLE:
                    tagPoint = new Vector3(cp[0].x + (cp[2].x - cp[0].x) * 0.5f,
                                           height + 0.01f,
                                           cp[0].z + (cp[2].z - cp[0].z) * 0.5f);
                    break;
                case RoomShape.LSHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[3].x - cp[0].x) * 0.65f,
                                           height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.65f);
                    break;
                case RoomShape.USHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[7].x - cp[0].x) * 0.5f,
                                           height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.5f);
                    break;
                case RoomShape.SSHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[4].x - cp[0].x) * 0.5f,
                                           height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.5f);
                    break;
                case RoomShape.TSHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[5].x - cp[0].x) * 0.5f,
                                           height + 0.01f,
                                           cp[0].z + (cp[5].z - cp[0].z) * 0.5f);
                    break;
                default:
                    break;
            }
            return tagPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public List<List<float>> UniqueCoordinates(bool localCoordinates = false) {
            List<List<float>> uniqueCoordinates = new List<List<float>> {
                GetControlPoints(localCoordinates: localCoordinates).Select(p => p[0]).Distinct().OrderBy(n => n).ToList(),
                GetControlPoints(localCoordinates: localCoordinates).Select(p => p[1]).Distinct().OrderBy(n => n).ToList(),
                GetControlPoints(localCoordinates: localCoordinates).Select(p => p[2]).Distinct().OrderBy(n => n).ToList()
            };
            return uniqueCoordinates;
        }

        /// <summary>
        /// 
        /// </summary>
        void OnMouseDown() {
            if (State == RoomState.MOVING) {
                MoveMode.Instance.Offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnMouseDrag() {
            if (State == RoomState.MOVING) {
                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + MoveMode.Instance.Offset;
                transform.position = Grid.GetNearestGridpoint(currentPosition);
            }
        }

        public void SetIsRoomCurrentlyColliding() {

            if (State == RoomState.PREVIEW || State == RoomState.MOVING) { // Only triggers collision events on moving object

                RoomCollider[] colliders = gameObject.GetComponentsInChildren<RoomCollider>();
                List<bool> collidersColliding = colliders.Select(rc => rc.isCurrentlyColliding).ToList();

                bool isSelected = (SelectMode.Instance.selection == this);
                bool isColliding = (collidersColliding.Contains(true));
                //Debug.Log("Selected:" + isSelected.ToString() + "  Colliding:" + isColliding.ToString());
                UpdateRender2D(highlighted: isSelected, colliding: isColliding);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetRoomType(RoomType type) {
            // Change type
            Type = type;
            // Change material
            currentMaterial = AssetUtil.LoadAsset<Material>("materials", RoomMaterialAsset[type]);
            gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetRoomNote(string value) {
            if (!string.IsNullOrEmpty(value)) {
                customProperty = value;
            }
        }

        /// <summary>
        /// Finds the boundaries of the room in the X and Y axis.
        /// returns:  { minX, maxX, minY, maxY }
        /// </summary>
        public List<float> Bounds(bool localCoordinates = false) {
            float minX = 0; float maxX = 0;
            float minZ = 0; float maxZ = 0;
            foreach (Vector3 controlPoint in GetControlPoints(localCoordinates)) {
                if (controlPoint.x < minX) { minX = controlPoint.x; }
                if (controlPoint.x > maxX) { maxX = controlPoint.x; }
                if (controlPoint.z < minZ) { minZ = controlPoint.z; }
                if (controlPoint.z > maxZ) { maxZ = controlPoint.z; }
            }
            return new List<float> { minX, maxX, minZ, maxZ };
        }

    }
}