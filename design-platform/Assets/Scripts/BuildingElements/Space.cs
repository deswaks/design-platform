using DesignPlatform.Utils;
using DesignPlatform.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using DesignPlatform.Modes;
using DesignPlatform.Database;

namespace DesignPlatform.Core {

    
    /// <summary>
    /// Describes the state of a building space.
    /// </summary>
    public enum SpaceState {
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

        public Building ParentBuilding { get; private set; }
        public SpaceShape Shape { get; private set; }
        public SpaceFunction Type { get; private set; }
        public SpaceState State { get; set; }

        private List<Vector3> controlPoints;
        public float height = 3.0f;
        public List<Face> Faces { get; private set; }
        public List<Interface> Interfaces {
            get { return Faces.SelectMany(f => f.Interfaces).ToList(); }
            private set {; }
        }
        public List<Opening> Openings {
            get { return Faces.SelectMany(f => f.Openings).ToList(); }
            private set {; }
        }

        private Vector3 moveModeOffset;
        private Material currentMaterial;
        public Material highlightMaterial;
        public string customProperty;


        private readonly Dictionary<SpaceFunction, string> SpaceMaterialAsset = new Dictionary<SpaceFunction, string> {
            { SpaceFunction.PREVIEW,  "plan_space_default"},
            { SpaceFunction.DEFAULT,  "plan_space_default"},
            { SpaceFunction.SINGLEROOM,  "plan_space_singleroom"},
            { SpaceFunction.DOUBLEROOM,  "plan_space_doubleroom"},
            { SpaceFunction.LIVINGROOM,  "plan_space_livingroom"},
            { SpaceFunction.KITCHEN,  "plan_space_kitchen"},
            { SpaceFunction.BATHROOM,  "plan_space_bathroom"},
            { SpaceFunction.CORRIDOR,  "plan_space_corridor"}
        };

        private readonly Dictionary<SpaceFunction, string> SpaceTypeName = new Dictionary<SpaceFunction, string> {
            { SpaceFunction.PREVIEW,  "Preview"},
            { SpaceFunction.DEFAULT,  ""},
            { SpaceFunction.SINGLEROOM,  "Single Bed\nRoom"},
            { SpaceFunction.DOUBLEROOM,  "Double Bed\nRoom"},
            { SpaceFunction.LIVINGROOM,  "Living\nRoom"},
            { SpaceFunction.KITCHEN,  "Kitchen"},
            { SpaceFunction.BATHROOM,  "Bathroom"},
            { SpaceFunction.CORRIDOR,  "Corridor"}
        };
        public string TypeName {
            get { return SpaceTypeName[Type]; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildShape"></param>
        /// <param name="building"></param>
        public void InitSpace(SpaceShape buildShape = SpaceShape.RECTANGLE, Building building = null, SpaceFunction type = SpaceFunction.DEFAULT) {
            // Set constant values
            ParentBuilding = building;
            gameObject.layer = 8;       // 8 = Rooom layer
            Shape = buildShape;
            State = SpaceState.STATIONARY;
            currentMaterial = AssetUtil.LoadAsset<Material>("materials", SpaceMaterialAsset[Type]);

            // Controlpoints
            switch (Shape) {
                case SpaceShape.RECTANGLE:
                    controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                   new Vector3(0, 0, 3),
                                                   new Vector3(3, 0, 3),
                                                   new Vector3(3, 0, 0)};
                    gameObject.name = "Space(Rectangle)";
                    break;
                case SpaceShape.LSHAPE:
                    controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 0)};
                    gameObject.name = "Space(L-Shape)";
                    break;
                case SpaceShape.USHAPE:
                    controlPoints = new List<Vector3> {   new Vector3(0, 0, 0),
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 5),
                                                          new Vector3(8, 0, 5),
                                                          new Vector3(8, 0, 0)};
                    gameObject.name = "Space(U-Shape)"; 
                    break;       
                case SpaceShape.SSHAPE:
                    controlPoints = new List<Vector3> {   new Vector3(0, 0, 0),
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(6, 0, 3),
                                                          new Vector3(6, 0, -2),
                                                          new Vector3(3, 0, -2),
                                                          new Vector3(3, 0, 0),
                    };
                    gameObject.name = "Space(S-Shape)"; 
                    break;
                case SpaceShape.TSHAPE:
                    controlPoints = new List<Vector3> {   new Vector3(0, 0, 0),
                                                          new Vector3(-2, 0, 0),
                                                          new Vector3(-2, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 0),
                                                          new Vector3(3, 0, 0),
                                                          new Vector3(3, 0, -3),
                                                          new Vector3(0, 0, -3),
                    };
                    gameObject.name = "Space(T-Shape)"; 
                    break;
            }
            if(Type == SpaceFunction.PREVIEW) gameObject.name = "Preview " + gameObject.name;
            InitFaces();
            InitRender3D();
            SetSpaceType(type);
            InitRender2D();
        }

        public override string ToString() {
            return gameObject.name.ToString() +" "+ TypeName;
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
            SpaceCollider.GiveCollider(this);
        }
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
        public void UpdateRender2D(bool highlighted = false, bool colliding = false) {
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();

            // Update lines
            lr.positionCount = 0;
            if (UI.Settings.ShowWallLines) {
                // Set controlpoints
                lr.useWorldSpace = false;
                List<Vector3> points = GetControlPoints(localCoordinates: true).Select(p => p + Vector3.up * (height + 0.001f)).ToList();
                lr.positionCount = points.Count;
                lr.SetPositions(points.ToArray());

                // Style
                lr.materials = Enumerable.Repeat(AssetUtil.LoadAsset<Material>("materials", "plan_space_wall"), lr.positionCount).ToArray();
                float width = 0.2f;
                Color color = Color.black;
                lr.sortingOrder = 0;
                if (highlighted) { lr.sortingOrder = 1; width = 0.3f; color = Color.yellow; }
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
            if (UI.Settings.ShowSpaceTags && Type != SpaceFunction.PREVIEW) {
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
                tag.color = Color.black;
                tag.fontSize = 5.0f;
                tag.alignment = TextAlignmentOptions.Center;
                tag.text = TypeName;
                tag.sortingOrder = 300;
            }
        }

        /// <summary>
        /// Rotates the space. Defaults to 90 degree increments
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
        /// Calculates the (brutto) floor area of the space including half the walls.
        /// </summary>
        /// <returns>float area</returns>
        public float GetFloorArea() {
            Vector2[] vertices = GetControlPoints(localCoordinates: true).Select(p => new Vector2(p.x, p.z)).ToArray();
            Polygon2D spacePolygon = new Polygon2D(vertices);
            return spacePolygon.GetArea();
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
        /// Calculates the (brutto) floor area of the space including half the walls.
        /// </summary>
        /// <returns>float area</returns>
        public float GetVolume() {
            return GetFloorArea() * height;
        }

        /// <summary>
        /// Deletes the space
        /// </summary>
        public void Delete() {
            if (Faces != null && Faces.Count > 0) {
                foreach (Face face in Faces) {
                    face.Delete();
                }
            }
            if (Building.Instance.Spaces.Contains(this)) {
                ParentBuilding.RemoveSpace(this);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Moves the space to the given position
        /// </summary>
        public void Move(Vector3 exactPosition) {
            Vector3 gridPosition = Grid.GetNearestGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;      
            UpdateRender2D();
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
            Vector3 globalExtrusion = GetWallNormals(localCoordinates: false)[wallToExtrude] * extrusion;

            // Clone points
            List<Vector3> controlPointsClone = GetControlPoints(localCoordinates: true).Select(
                v => new Vector3(v.x, v.y, v.z)).ToList();

            // Extrude the cloned points
            int point1Index = IndexingUtils.WrapIndex(wallToExtrude, controlPoints.Count);
            int point2Index = IndexingUtils.WrapIndex(wallToExtrude + 1, controlPoints.Count);
            controlPointsClone[point1Index] += localExtrusion;
            controlPointsClone[point2Index] += localExtrusion;

            // Move openings with extrusion                   
            foreach (Opening opening in Faces[wallToExtrude].Openings) {
                Vector3 openingPoint = opening.ClosestPoint(opening.transform.position, Faces[wallToExtrude]);
                opening.transform.position = openingPoint;
                opening.AttachClosestFaces();
            }

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
            }

            UpdateRender3D();
            UpdateRender2D();

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
                case SpaceShape.RECTANGLE:
                    tagPoint = new Vector3(cp[0].x + (cp[2].x - cp[0].x) * 0.5f,
                                           height + 0.01f,
                                           cp[0].z + (cp[2].z - cp[0].z) * 0.5f);
                    break;
                case SpaceShape.LSHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[3].x - cp[0].x) * 0.65f,
                                           height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.65f);
                    break;
                case SpaceShape.USHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[7].x - cp[0].x) * 0.5f,
                                           height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.5f);
                    break;
                case SpaceShape.SSHAPE:
                    tagPoint = new Vector3(cp[0].x + (cp[4].x - cp[0].x) * 0.5f,
                                           height + 0.01f,
                                           cp[0].z + (cp[3].z - cp[0].z) * 0.5f);
                    break;
                case SpaceShape.TSHAPE:
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
            if (State == SpaceState.MOVING) {
                moveModeOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnMouseDrag() {
            if (State == SpaceState.MOVING) {
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + moveModeOffset;
                for (int i = 0; i < Faces.Count; i++) {
                    for (int j = 0; j < Faces[i].Openings.Count; j++) {
                        Vector3 diff = gameObject.transform.position - Faces[i].Openings[j].gameObject.transform.position;
                        Faces[i].Openings[j].gameObject.transform.position = (Grid.GetNearestGridpoint(curPosition)) - diff;
                    }
                }
                transform.position = Grid.GetNearestGridpoint(curPosition);
                
                foreach (Opening opening in Openings) {
                    foreach (Face openingsAttachedFace in opening.Faces) {
                        bool faceBelongsToThisSpace = (Faces.Contains(openingsAttachedFace));
                        if (!faceBelongsToThisSpace) openingsAttachedFace.RemoveOpening(opening);
                    }
                }

                foreach (Space space in Building.Instance.Spaces) {
                    foreach (Face face in space.Faces) {
                        foreach (Opening opening in face.Openings) {
                            opening.AttachClosestFaces();
                        }
                    }
                }
            }
        }

        public void SetIsSpaceCurrentlyColliding() {

            if (Type == SpaceFunction.PREVIEW || State == SpaceState.MOVING) { // Only triggers collision events on moving object

                SpaceCollider[] colliders = gameObject.GetComponentsInChildren<SpaceCollider>();
                List<bool> collidersColliding = colliders.Select(rc => rc.isCurrentlyColliding).ToList();

                bool isSelected = (Modes.SelectMode.Instance.selection == this);
                bool isColliding = (collidersColliding.Contains(true));
                //Debug.Log("Selected:" + isSelected.ToString() + "  Colliding:" + isColliding.ToString());
                UpdateRender2D(highlighted: isSelected, colliding: isColliding);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetSpaceType(SpaceFunction type) {
            try {
                // Change type
                Type = type;
                // Change material
                currentMaterial = AssetUtil.LoadAsset<Material>("materials", SpaceMaterialAsset[type]);
                gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
                return;
            }
            catch {
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetSpaceNote(string value) {
            if (!string.IsNullOrEmpty(value)) {
                customProperty = value;
            }
        }

        /// <summary>
        /// Finds the boundaries of the space in the X and Y axis.
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