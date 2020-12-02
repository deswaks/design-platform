using DesignPlatform.Utils;
using DesignPlatform.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {


    /// <summary>
    /// Describes the state of a building space.
    /// </summary>
    public enum MoveState {
        /// <summary> space cannot be moved. </summary>
        STATIONARY,
        /// <summary> space can be moved </summary>
        MOVING
    }

    /// <summary>
    /// Describes the function of a building space.
    /// </summary>
    public enum SpaceFunction {
        /// <summary> space is not placed in the model and is only used for preview purposes </summary>
        PREVIEW = 0,
        /// <summary> The default undefined function </summary>
        DEFAULT = 1,
        /// <summary> Space is a double bedroom </summary>
        DOUBLEROOM = 10,
        /// <summary> Space is a single bedroom </summary>
        SINGLEROOM = 11,
        /// <summary> Space is a living room </summary>
        LIVINGROOM = 12,
        /// <summary> Space is a kitchen </summary>
        KITCHEN = 13,
        /// <summary> Space is a bathroom </summary>
        BATHROOM = 14,
        /// <summary> Space is a corridor </summary>
        CORRIDOR = 15
    }

    /// <summary>
    /// Describes the shape of a building space.
    /// </summary>
    public enum SpaceShape {
        /// <summary> Rectangular building space </summary>
        RECTANGLE,
        /// <summary> L-shaped building space </summary>
        LSHAPE,
        /// <summary> U-shaped building space </summary>
        USHAPE,
        /// <summary> S-shaped building space </summary>
        SSHAPE,
        /// <summary> T-shaped building space </summary>
        TSHAPE
    }

    /// <summary>
    /// The main building component represents the functional building space.
    /// It is defined by a polygonal geometry and a function in the house.
    /// </summary>
    public class Space : MonoBehaviour {

        /// <summary>The control points are the vertices of the underlying polygon
        /// extruded to obtain the three dimensional representation</summary>
        public List<Vector3> ControlPoints { get; private set; }

        /// <summary>The height of the 3D representation of this space (Floor to floor elevation).</summary>
        public float Height { get; private set; } = 3.0f;

        /// <summary>The function of this room in a house, eg. bathroom or bedroom.</summary>
        private SpaceFunction function;

        /// <summary>The function of this room in a house, eg. bathroom or bedroom.</summary>
        public SpaceFunction Function {
            get { return function; }
            set { function = value; UpdateRender3D(); }
        }

        /// <summary>The move state of this room determines whether it can be moved or is loced for movement</summary>
        public MoveState MoveState { get; set; }

        /// <summary>The shape of this space is a classification of the base outline shape of its
        /// extrusion to obtain the three dimensional representation. 
        /// It is utimately determined by its controlpoints but given here as an easy reference.</summary>
        public SpaceShape Shape { get; private set; }

        /// <summary>The material currently assigned to the 3D representation of this space.</summary>
        public Material Material3D { get; private set; }

        /// <summary>The underlying polygon extruded to obtain the three dimensional representation</summary>
        public Polygon2 Polygon {
            get {
                return new Polygon2(GetControlPoints(localCoordinates: true)
                    .Select(p => new Vector2(p.x, p.z)).ToArray());
            }
        }

        /// <summary>The (brutto) floor area of the space including half the walls.</summary>
        public float Area { get {return Polygon.Area;} }

        /// <summary>The volume of the space based on the (brutto) floor area.</summary>
        public float Volume { get { return Area * Height; } }

        /// <summary>The two dimensional faces of the three dimensional solid space.</summary>
        public List<Face> Faces { get; private set; }

        /// <summary>The interfaces connected to all the faces of this space.</summary>
        public List<Interface> Interfaces {
            get { return Faces.SelectMany(f => f.Interfaces).ToList(); }
            private set {; }
        }

        /// <summary>The openings lying on all the faces of this space.</summary>
        public List<Opening> Openings {
            get { return Faces.SelectMany(f => f.Openings).ToList(); }
            private set {; }
        }

        /// <summary>The custom note is a simple text string that can be assigned to the space.</summary>
        public string CustomNote {
            get { return CustomNote; }
            set { if (!string.IsNullOrEmpty(value)) CustomNote = value;}
        }

        /// <summary>The offset of the pointer, used to place the space correctly when moved.</summary>
        private Vector3 MoveModeOffset { get; set; }



        /// <summary>
        /// Initialize the space by setting up constant parameters, game objects, 2D and 3D goemetry.
        /// </summary>
        /// <param name="spaceShape">Shape to build the space in</param>
        /// <param name="spaceFunction">Function of the space. This can be set later.</param>
        public void InitSpace(SpaceShape spaceShape = SpaceShape.RECTANGLE, SpaceFunction spaceFunction = SpaceFunction.DEFAULT) {
            // Set constant values
            gameObject.layer = 8;       // 8 = Rooom layer
            Shape = spaceShape;
            Function = spaceFunction;
            MoveState = MoveState.STATIONARY;
            Material3D = AssetUtil.LoadAsset<Material>("materials", Settings.SpaceMaterialAssets[Function]);
            ControlPoints = Settings.SpaceControlPoints[Shape];

            gameObject.name = Settings.SpaceGameObjectNames[Shape] + " #" + Building.Instance.Spaces.Count().ToString();
            if (Function == SpaceFunction.PREVIEW) gameObject.name = "Preview " + gameObject.name;

            CreateFaces();
            InitRender3D();
            InitRender2D();
        }

        /// <summary>
        /// Updates the 3D representation of this space.
        /// </summary>
        public void UpdateRender3D() {
            if (gameObject.GetComponent(typeof(MeshRenderer)) == null) return;
            // Mesh
            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(ControlPoints);
            polyshape.extrude = Height;
            polyshape.CreateShapeFromPolygon();
            gameObject.GetComponent<ProBuilderMesh>().Refresh();
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
            Material3D = AssetUtil.LoadAsset<Material>("materials", Settings.SpaceMaterialAssets[Function]);
            gameObject.GetComponent<MeshRenderer>().material = Material3D;

            // Collider
            SpaceCollider.GiveCollider(this);
        }

        /// <summary>
        /// Updates the 2D representation of this space.
        /// </summary>
        /// <param name="highlighted">Determines whether the 2d representation should be for a selected room.</param>
        /// <param name="colliding"></param>
        public void UpdateRender2D(bool highlighted = false, bool colliding = false) {
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();

            // Update lines
            lr.positionCount = 0;
            if (UI.Settings.ShowWallLines) {
                // Set controlpoints
                lr.useWorldSpace = false;
                List<Vector3> points = GetControlPoints(localCoordinates: true).Select(p => p + Vector3.up * (Height + 0.001f)).ToList();
                lr.positionCount = points.Count;
                lr.SetPositions(points.ToArray());

                // Style
                lr.materials = Enumerable.Repeat(AssetUtil.LoadAsset<Material>("materials", "plan_space_wall"), lr.positionCount).ToArray();
                float width = 0.2f;
                Color color = Color.black;
                lr.sortingOrder = 0;
                if (highlighted) { lr.sortingOrder = 1; width = 0.2f; color = Color.yellow; }
                if (colliding) { lr.sortingOrder = 1; color = Color.red; }

                lr.startWidth = width; lr.endWidth = width;
                foreach (Material material in lr.materials) {
                    material.color = color;
                }
            }

            // Update text tags
            if (GetComponentInChildren<TMP_Text>()) {
                foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>()) {
                    Destroy(text.gameObject);
                }
            }
            if (UI.Settings.ShowSpaceTags && Function != SpaceFunction.PREVIEW) {
                // Create tag object
                GameObject tagObject = new GameObject("Tag");
                TextMeshPro tag = tagObject.AddComponent<TextMeshPro>();
                // Set position
                Vector3 tagPosition = GetTagLocation(localCoordinates: true);
                tagObject.transform.SetParent(gameObject.transform, worldPositionStays: true);
                tagObject.GetComponent<RectTransform>().anchoredPosition = tagPosition;
                tagObject.transform.localPosition = new Vector3(tagObject.transform.localPosition.x,
                                                           tagObject.transform.localPosition.y,
                                                           tagPosition.z);
                tagObject.transform.rotation = Quaternion.identity;
                tagObject.transform.Rotate(new Vector3(90, 0, 0));
                // Set text
                tag.font = AssetUtil.LoadAsset<TMP_FontAsset>("fonts", "OpenSans-Light_Asset");
                tag.color = Color.black;
                tag.fontSize = 5.0f;
                tag.alignment = TextAlignmentOptions.Center;
                tag.text = Settings.SpaceTypeNames[Function];
                tag.sortingOrder = 300;
            }
        }

        /// <summary>
        /// Writes a description of the space.
        /// </summary>
        /// <returns>The description of this space.</returns>
        public override string ToString() {
            return gameObject.name.ToString() + " " + Settings.SpaceTypeNames[Function];
        }

        /// <summary>
        /// Rotates the space.
        /// </summary>
        /// <param name="clockwise">Determines the rotation direction. Clockwise (true and default) or counter clockwise (false)</param>
        /// <param name="degrees">Degrees to rotate the room. Default to 90 degree increments.</param>
        public void Rotate(bool clockwise = true, float degrees = 90) {
            if (!clockwise) { degrees = -degrees; }

            float[] bounds = Polygon.Bounds;
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

            if (Openings.Count > 0) {
                foreach (Opening opening in Openings) {
                    opening.gameObject.transform.RotateAround(
                                                point: centerPoint,
                                                axis: new Vector3(0, 1, 0),
                                                angle: degrees);
                    opening.AttachClosestFaces();
                }
            }
            
            UpdateRender3D();
            UpdateRender2D();
        }

        /// <summary>
        /// Deletes the space.
        /// </summary>
        public void Delete() {
            if (Faces != null && Faces.Count > 0) {
                foreach (Face face in Faces) {
                    face.Delete();
                }
            }
            if (Building.Instance.Spaces.Contains(this)) {
                Building.Instance.RemoveSpace(this);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Moves the space to the given position.
        /// </summary>
        /// <param name="exactPosition">Point to move the space to.</param>
        public void Move(Vector3 exactPosition) {
            Vector3 gridPosition = Grid.GetNearestGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;      
        }

        /// <summary>
        /// Finds the controlpoints of the space.
        /// The controlpoints are the vertices of the underlying polyshape of the building.
        /// </summary>
        /// <param name="localCoordinates">Determines whether the coordinates should be given locally (true) or world coordinates (false)</param>
        /// <param name="closed">Determines whether the first points should be repeated as the last point in the list.</param>
        /// <returns>List of the controlpoints of the space</returns>
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
        /// Sets the controlpoints (the vertices of the underlying footprint polygon geometry).
        /// </summary>
        /// <param name="newControlPoints">The new controlpoints of the space.</param>
        public void SetControlPoints(List<Vector3> newControlPoints) {
            ControlPoints = newControlPoints;
            UpdateRender3D();
            UpdateRender2D();
        }

        /// <summary>
        /// Gets a list of the midpoints of the edges of the underlying footprint polygon.
        /// </summary>
        /// <param name="localCoordinates">Determines whether the coordinates should be given locally (true) or world coordinates (false)</param>
        /// <returns></returns>
        public List<Vector3> GetPolygonMidpoints(bool localCoordinates = false) {
            List<Vector3> midPoints = new List<Vector3>();
            List<Vector3> circularControlpoints = GetControlPoints(localCoordinates: localCoordinates, closed: true);
            for (int i = 0; i < ControlPoints.Count; i++) {
                midPoints.Add((circularControlpoints[i] + circularControlpoints[i + 1]) / 2);
            }
            return midPoints;
        }

        /// <summary>
        /// Gets a list of normals. They are in same order as controlpoints (clockwise).
        /// </summary>
        /// <param name="localCoordinates">Determines whether the coordinates should be given locally (true) or world coordinates (false)</param>
        /// <returns>List of wall normals.</returns>
        public List<Vector3> GetFaceNormals(bool localCoordinates = false) {
            List<Vector3> wallNormals = new List<Vector3>();
            List<Vector3> circularControlpoints = GetControlPoints(localCoordinates: localCoordinates, closed: true);
            for (int i = 0; i < ControlPoints.Count; i++) {
                wallNormals.Add(Vector3.Cross(circularControlpoints[i + 1] - circularControlpoints[i], Vector3.up).normalized);
            }
            return wallNormals;
        }

        /// <summary>
        /// Extrudes a wall by moving the underlying edge of the space footprint polygon.
        /// </summary>
        /// <param name="wallToExtrude">Index of the wall to extrude</param>
        /// <param name="extrusion">Distance to extrude the wall. Negative distance will extrude the wall into the space.</param>
        public void ExtrudeWall(int wallToExtrude, float extrusion) {
            // Create move vector for extrusion
            Vector3 localExtrusion = GetFaceNormals(localCoordinates: true)[wallToExtrude] * extrusion;
            Vector3 globalExtrusion = GetFaceNormals(localCoordinates: false)[wallToExtrude] * extrusion;

            // Clone points
            List<Vector3> controlPointsClone = GetControlPoints(localCoordinates: true).Select(
                v => new Vector3(v.x, v.y, v.z)).ToList();

            // Extrude the cloned points
            int point1Index = IndexingUtils.WrapIndex(wallToExtrude, ControlPoints.Count);
            int point2Index = IndexingUtils.WrapIndex(wallToExtrude + 1, ControlPoints.Count);
            controlPointsClone[point1Index] += localExtrusion;
            controlPointsClone[point2Index] += localExtrusion;

            // Move openings with extrusion                   
            foreach (Opening opening in Faces[wallToExtrude].Openings) {
                Vector3 openingPoint = Faces[wallToExtrude].LocationLine.ClosestPoint(opening.transform.position);
                opening.transform.position = openingPoint;
                opening.AttachClosestFaces();
            }

            // Compare normals before and after extrusion element-wise 
            List<Vector3> normals = GetFaceNormals(localCoordinates: true);
            List<Vector3> extrudedNormals = PolygonUtils.PolygonNormals(controlPointsClone);
            bool normalsAreIdentical = true;
            for (int i = 0; i < normals.Count; i++) {
                if (normals[i] != extrudedNormals[i]) {
                    normalsAreIdentical = false;
                }
            }

            // If normals did not change: Make extruded points the real control points 
            if (normalsAreIdentical) {
                ControlPoints = controlPointsClone;
            }

            UpdateRender3D();
            UpdateRender2D();

        }

        /// <summary>
        /// Finds the point inside the building where the tag can be located for screen visibility and pdf print.
        /// </summary>
        /// <param name="localCoordinates">Determines whether the coordinates should be given locally (true) or world coordinates (false)</param>
        /// <returns>Location of the tag point inside the building.</returns>
        public Vector3 GetTagLocation(bool localCoordinates = false) {
            Vector3 tagPoint = new Vector3();
            List<Vector3> cp = GetControlPoints(localCoordinates: localCoordinates);

            switch (Shape) {
                case SpaceShape.RECTANGLE:
                    tagPoint = new Vector3(cp[0].x + (cp[2].x - cp[0].x) * 0.5f,
                                           Height + 0.01f,
                                           cp[0].z + (cp[2].z - cp[0].z) * 0.5f);
                    break;
                case SpaceShape.LSHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[3].x - cp[0].x) * 0.65f,
                                           Height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.65f);
                    break;
                case SpaceShape.USHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[7].x - cp[0].x) * 0.5f,
                                           Height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.5f);
                    break;
                case SpaceShape.SSHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[4].x - cp[0].x) * 0.5f,
                                           Height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.5f);
                    break;
                case SpaceShape.TSHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[5].x - cp[0].x) * 0.5f,
                                           Height + 0.01f,
                                           cp[0].z + (cp[5].z - cp[0].z) * 0.5f);
                    break;
                default:
                    break;
            }
            return tagPoint;
        }

        /// <summary>
        /// Defines the room's reaction to collision events among its attached colliders.
        /// </summary>
        /// <param name="isColliding">Determines whether the space should be updated for a collision (true)
        /// or the termination of a collision (false)</param>
        public void OnCollisionEvent(bool isColliding) {
            if (Function == SpaceFunction.PREVIEW || MoveState == MoveState.MOVING) {
                UpdateRender2D( highlighted: Modes.SelectMode.Instance.selection == this,
                                colliding: isColliding);
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
                for (int i = 0; i < ControlPoints.Count; i++) {
                    ControlPoints[i] -= difference;
                }
                UpdateRender3D();
                UpdateRender2D();
            }
        }

        /// <summary>
        /// Initializes the drag movement of the room.
        /// </summary>
        void OnMouseDown() {
            if (MoveState == MoveState.MOVING) {
                MoveModeOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        /// <summary>
        /// Defines the drag movement of the room.
        /// </summary>
        void OnMouseDrag() {
            // Abort if space is locked for movement
            if (MoveState != MoveState.MOVING) return;

            // Find offset position
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + MoveModeOffset;

            // Move openings
            foreach (Opening opening in Openings) {
                Vector3 diff = gameObject.transform.position - opening.gameObject.transform.position;
                opening.gameObject.transform.position = (Grid.GetNearestGridpoint(currentPosition)) - diff;
            }

            // Move space
            transform.position = Grid.GetNearestGridpoint(currentPosition);
        }

        /// <summary>
        /// Ends the drag movement of the room.
        /// </summary>
        private void OnMouseUp() {
            //Reattach the openings
            Building.Instance.Openings.ForEach(opening => opening.AttachClosestFaces());
        }

        /// <summary>
        /// Creates the face objects on each of the surfaces of the 3D representation of this space.
        /// </summary>
        private void CreateFaces() {
            Faces = new List<Face>();
            for (int i = 0; i < ControlPoints.Count + 2; i++) {
                Faces.Add(new Face(this, i));
            }
        }

        /// <summary>
        /// Initializes the 3D representation of this space.
        /// </summary>
        private void InitRender3D() {
            gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent<PolyShape>();
            gameObject.AddComponent<ProBuilderMesh>();
            UpdateRender3D();
        }

        /// <summary>
        /// Initializes the 2D representation of this space.
        /// </summary>
        private void InitRender2D() {
            // Init line render
            LineRenderer lr = gameObject.AddComponent<LineRenderer>();
            lr.loop = true;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            lr.sortingLayerName = "PLAN";

            // Update
            UpdateRender2D();
        }

    }
}