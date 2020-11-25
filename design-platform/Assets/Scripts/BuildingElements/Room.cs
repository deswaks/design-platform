using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Room : MonoBehaviour {
    private List<Vector3> controlPoints;

    public List<Face> faces { get; private set; }

<<<<<<< Updated upstream
    private RoomShape shape;
    private Material currentMaterial;
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material singleRoomMaterial;
    public Material doubleRoomMaterial;
    public Material livingroomMaterial;
    public Material kitchenMaterial;
    public Material bathroomMaterial;

    public Building parentBuilding;
    public float height = 3.0f;

    public string customProperty;
    //private Dictionary<string, string> customProperties = new Dictionary<string, string>();

    private bool isHighlighted { set; get; }
    private Room prefabRoom;    

    private GameObject moveHandle;
    private bool isRoomInMoveMode = false;

    private GameObject editHandle;
    public GameObject editHandlePrefab;
    public List<GameObject> activeEditHandles;
=======
    public enum RoomType {
        //Til senere implementering
        NULL = -10,
        DELETED = -3,
        HIDDEN = -2,
        SELECTED = -1,
        //------------------
        PREVIEW = 0,
        DEFAULT = 1,
        DOUBLEROOM = 10,
        SINGLEROOM = 11,
        LIVINGROOM = 12,
        KITCHEN = 13,
        BATHROOM = 14
        //HALLWAY = 15
    }
>>>>>>> Stashed changes

    public GameObject moveHandlePrefab;
    private Vector3 moveModeOffset;

    public RoomType roomType { get; private set; }

    public enum RoomStates {
        Stationary,
        Preview,
        Moving
    }
    private RoomStates roomState;

    public bool isCurrentlyColliding = false;

<<<<<<< Updated upstream
    // Construct room of type 0 (Rectangle) or 1 (L-shape)
    public void InitializeRoom(RoomShape buildShape = RoomShape.RECTANGLE, Building building = null) {
        // Set constant values
        parentBuilding = building;
        gameObject.layer = 8; // Rooom layer
=======
        public Building ParentBuilding { get; private set; }
        public RoomShape Shape { get; private set; }

        private RoomType _type;
        public RoomType Type {
            get { return _type; }
            set { _type = value; UpdateRender2D(); UpdateRender3D(); }
        }
        public RoomState State { get; set; }
>>>>>>> Stashed changes

        shape = buildShape;
        roomState = RoomStates.Preview;

<<<<<<< Updated upstream
        // Get relevant properties from prefab object
        GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RoomPrefab.prefab");

        prefabRoom = (Room)prefabObject.GetComponent(typeof(Room));

        switch (shape) {
            case RoomShape.RECTANGLE:
                controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                   new Vector3(0, 0, 3),
                                                   new Vector3(3, 0, 3),
                                                   new Vector3(3, 0, 0)};
                gameObject.name = "Room(Rectangle)";
                break;
            case RoomShape.LSHAPE:
                controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
=======
        private Vector3 moveModeOffset;

        public string Note {
            get { return Note; }
            set { if (!string.IsNullOrEmpty(value)) Note = value; }
        }

        public bool IsSelected {
            get { return (SelectMode.Instance.selection == this); }
        }

        public bool IsColliding {
            get {
                RoomCollider[] colliders = gameObject.GetComponentsInChildren<RoomCollider>();
                return (colliders.Select(rc => rc.isCurrentlyColliding).Contains(true));
            }
        }


        private readonly Dictionary<RoomType, string> RoomMaterialAsset = new Dictionary<RoomType, string> {
            { RoomType.PREVIEW,  "plan_room_default"},
            { RoomType.DEFAULT,  "plan_room_default"},
            { RoomType.SELECTED, "plan_room_highlight" },
            { RoomType.SINGLEROOM,  "plan_room_singleroom"},
            { RoomType.DOUBLEROOM,  "plan_room_doubleroom"},
            { RoomType.LIVINGROOM,  "plan_room_livingroom"},
            { RoomType.KITCHEN,  "plan_room_kitchen"},
            { RoomType.BATHROOM,  "plan_room_bathroom"},
            //{ RoomType.BATHROOM,  "plan_room_hallway"},
        };

        private readonly Dictionary<RoomType, string> RoomTypeName = new Dictionary<RoomType, string> {
            { RoomType.PREVIEW,  "Preview"},
            { RoomType.DEFAULT,  ""},
            { RoomType.SELECTED, "Selected\nSpace" },
            { RoomType.SINGLEROOM,  "Single Bed\nRoom"},
            { RoomType.DOUBLEROOM,  "Double Bed\nRoom"},
            { RoomType.LIVINGROOM,  "Living\nRoom"},
            { RoomType.KITCHEN,  "Kitchen"},
            { RoomType.BATHROOM,  "Bathroom"},
            //{ RoomType.BATHROOM,  "Hallway"},
        };
        public string TypeName {
            get { return RoomTypeName[Type]; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="building"></param>
        public void InitRoom(RoomShape shape = RoomShape.RECTANGLE,
                             RoomType type = RoomType.DEFAULT) {
            
            // Set constant values
            ParentBuilding = Building.Instance;
            gameObject.layer = 8;       // 8 = Rooom layer
            Shape = shape;
            _type = type;
            State = RoomState.STATIONARY;

            // Controlpoints
            switch (Shape) {
                case RoomShape.RECTANGLE:
                    controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                   new Vector3(0, 0, 3),
                                                   new Vector3(3, 0, 3),
                                                   new Vector3(3, 0, 0)};
                    gameObject.name = "Rectangular space";
                    break;
                case RoomShape.LSHAPE:
                    controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
>>>>>>> Stashed changes
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 0)};
<<<<<<< Updated upstream
                gameObject.name = "Room(L-Shape)";
                break;
        }
        faces = new List<Face>();
        for (int i = 0; i < controlPoints.Count+2; i++) {
            faces.Add(new Face(this, i));
=======
                    gameObject.name = "L-Shaped space";
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
                    gameObject.name = "U-Shaped space";
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
                    gameObject.name = "S-Shaped space";
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
                    gameObject.name = "T-Shaped space";
                    break;
            }
            if (Type == RoomType.PREVIEW) gameObject.name = "Preview " + gameObject.name;
            InitFaces();
            InitRender3D();
            InitRender2D();
        }

        public override string ToString() {
            return (gameObject.name.ToString() + " " + TypeName).Trim();
>>>>>>> Stashed changes
        }

        // Create and attach collider objects
        gameObject.AddComponent<MeshCollider>();

<<<<<<< Updated upstream
=======
        public void UpdateRender3D() {
            // Mesh
            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(controlPoints);
            polyshape.extrude = height;
            polyshape.CreateShapeFromPolygon();
            gameObject.GetComponent<ProBuilderMesh>().Refresh();
            gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;

            // Set material
            gameObject.GetComponent<MeshRenderer>().material
                = AssetUtil.LoadAsset<Material>("materials", RoomMaterialAsset[Type]); ;

            // Collider
            RoomCollider.GiveCollider(this);
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
            if (GlobalSettings.ShowWallLines) {
                // Set controlpoints
                lr.useWorldSpace = false;
                List<Vector3> points = GetControlPoints(localCoordinates: true).Select(p => p + Vector3.up * (height + 0.001f)).ToList();
                lr.positionCount = points.Count;
                lr.SetPositions(points.ToArray());

                // Style
                lr.materials = Enumerable.Repeat(AssetUtil.LoadAsset<Material>("materials", "plan_room_wall"), lr.positionCount).ToArray();
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
>>>>>>> Stashed changes

        // Set room visualization geometry
        gameObject.AddComponent<PolyShape>();
        gameObject.AddComponent<ProBuilderMesh>();
        SetRoomType(RoomType.DEFAULT);
        RefreshView();
    }

    public void SetControlPoints(List<Vector3> newControlPoints) {
        controlPoints = newControlPoints;
        RefreshView();
    }

<<<<<<< Updated upstream
    public void RefreshView() {
        PolyShape polyshape = gameObject.GetComponent<PolyShape>();
        polyshape.SetControlPoints(controlPoints);
        polyshape.extrude = height;
        polyshape.CreateShapeFromPolygon();
        gameObject.GetComponent<ProBuilderMesh>().Refresh();
        gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
        RoomCollider.GiveCollider(this);
    }
=======
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
>>>>>>> Stashed changes

    public RoomShape GetRoomShape() {
        return shape;
    }

    /// <summary>
    /// Rotates the room. Defaults to 90 degree increments
    /// </summary>
    public void Rotate(bool clockwise = true, float degrees = 90) {
        if (!clockwise) { degrees = -degrees; }
        
        List<float> bounds = Bounds();
        float width = bounds[1] - bounds[0];
        float height = bounds[3] - bounds[2];

        Vector3 centerPoint = new Vector3(bounds[0] + width / 2,
                                          0,
                                          bounds[2] + height / 2);
        centerPoint = gameObject.transform.TransformPoint(centerPoint);

        // if width and height are not BOTH divisible or undivisible by double grid
        if (width % (Grid.size*2) == 0 && height % (Grid.size*2) != 0
         || width % (Grid.size*2) != 0 && height % (Grid.size*2) == 0) {
            centerPoint = Grid.GetNearestGridpoint(centerPoint);
        }

        // Rotation
        gameObject.transform.RotateAround(
            point: centerPoint,
            axis: new Vector3(0, 1, 0),
            angle: degrees);

        RefreshView();
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
            vertices = new List<Vector3>();
            vertices.Add(controlPoints[i]);
            vertices.Add(controlPoints[i] + new Vector3(0, height, 0));
            vertices.Add(controlPoints[j] + new Vector3(0, height, 0));
            vertices.Add(controlPoints[j]);

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
            parentBuilding.RemoveRoom(this); 
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
    /// 
    /// </summary>
    public void SetIsHighlighted(bool highlighted) {
        if (highlighted) {
            gameObject.GetComponent<MeshRenderer>().material = prefabRoom.highlightMaterial;
            isHighlighted = true;
        }
        else {
            gameObject.GetComponent<MeshRenderer>().material = this.currentMaterial;
            isHighlighted = false;
        }
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

<<<<<<< Updated upstream
    /// <summary>
    /// Gets a list of normals. They are in same order as controlpoints (clockwise). localCoordinates : true (for local coordinates, Sherlock)
    /// </summary>
    public List<Vector3> GetWallNormals(bool localCoordinates = false) {
        List<Vector3> wallNormals = new List<Vector3>();
        List<Vector3> circularControlpoints = GetControlPoints(localCoordinates: localCoordinates, closed: true);
        for (int i = 0; i < controlPoints.Count; i++) {
            wallNormals.Add(Vector3.Cross((circularControlpoints[i + 1] - circularControlpoints[i]), Vector3.up).normalized);
=======
        /// <summary>
        /// Moves the room to the given position
        /// </summary>
        public void Move(Vector3 exactPosition) {
            Vector3 gridPosition = Grid.GetNearestGridpoint(exactPosition);
            gameObject.transform.position = gridPosition;
            UpdateRender2D();
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

        // If normals did not change: Make extruded points the real control points 
        if (normalsAreIdentical) {
            controlPoints = controlPointsClone;
            RefreshView();
=======
        public void SetControlPoints(List<Vector3> newControlPoints) {
            controlPoints = newControlPoints;
            UpdateRender3D();
            UpdateRender2D();
>>>>>>> Stashed changes
        }

    }

    /// <summary>
    /// Resets the origin of the gameObject such that it is at the first controlpoint.
    /// </summary>
    public void ResetOrigin() {
        if (GetControlPoints()[0] != gameObject.transform.position) {
            Vector3 oCP = GetControlPoints(localCoordinates: true)[0];
            Vector3 oGO = gameObject.transform.position;
            Vector3 difference = new Vector3(oCP.x - oGO.x, 0, oCP.z - oGO.z);
            
            gameObject.transform.Translate(difference);
            for (int i = 0; i < controlPoints.Count; i++) {
                controlPoints[i] -= difference;
            }
            RefreshView();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetEditHandles() {

        for (int i = 0; i < controlPoints.Count; i++) {
            editHandle = Instantiate(prefabRoom.editHandlePrefab);
            editHandle.transform.SetParent(gameObject.transform, true);
            editHandle.GetComponent<EditHandle>().InitializeHandle(i);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void RemoveEditHandles() {
        EditHandle[] editHandles = GetComponentsInChildren<EditHandle>();
        if (editHandles != null) {
            foreach (EditHandle editHandle in editHandles) {
                Destroy(editHandle.gameObject);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetTagLocation(bool localCoordinates = false) {
        Vector3 tagPoint = new Vector3();
        List<Vector3> cp = GetControlPoints(localCoordinates: localCoordinates);

        switch (shape) {
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
        List<List<float>> uniqueCoordinates = new List<List<float>>();
        uniqueCoordinates.Add(GetControlPoints(localCoordinates: localCoordinates).Select(p => p[0]).Distinct().OrderBy(n => n).ToList());
        uniqueCoordinates.Add(GetControlPoints(localCoordinates: localCoordinates).Select(p => p[1]).Distinct().OrderBy(n => n).ToList());
        uniqueCoordinates.Add(GetControlPoints(localCoordinates: localCoordinates).Select(p => p[2]).Distinct().OrderBy(n => n).ToList());
        return uniqueCoordinates;
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetIsInMoveMode(bool setMoveMode = false)
    {
        // Destroys any prior movehandle
        if (moveHandle != null) {
            Destroy(moveHandle);
            moveHandle = null;
        }

        if (setMoveMode == true) {
            roomState = RoomStates.Moving;

            // Create and attach move handle
            moveHandle = Instantiate(prefabRoom.moveHandlePrefab);
            moveHandle.transform.position = GetTagLocation(localCoordinates: true);
            moveHandle.transform.SetParent(gameObject.transform, false);
        }
        else {
            roomState = RoomStates.Stationary;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnMouseDown() {
        if (roomState == RoomStates.Moving) {
            //Vector3 moveModeScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            moveModeOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnMouseDrag() {
        if (roomState == RoomStates.Moving) {
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + moveModeOffset;
            transform.position = Grid.GetNearestGridpoint(curPosition);
        }
    }   

    public void SetIsRoomCurrentlyColliding() {
        if (roomState == RoomStates.Preview || roomState == RoomStates.Moving) { // Only triggers collision events on moving object

            List<bool> collidersColliding = gameObject.GetComponentsInChildren<RoomCollider>().Select(rc => rc.isCurrentlyColliding).ToList(); // list of whether or not each collider is currently colliding

            if (collidersColliding.TrueForAll(b => !b)) { // if there are no collisions in any of room's colliders
                isCurrentlyColliding = false;
                //Material meshRenderMaterial = gameObject.GetComponent<MeshRenderer>().material;
                if (isHighlighted) gameObject.GetComponent<MeshRenderer>().material = prefabRoom.highlightMaterial;
                else gameObject.GetComponent<MeshRenderer>().material = currentMaterial;

            }
            else { // if there is one or more collision(s)
                //Debug.Log("Is colliding");

<<<<<<< Updated upstream
                isCurrentlyColliding = true;
                gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
=======
        /// <summary>
        /// 
        /// </summary>
        void OnMouseDrag() {
            if (State == RoomState.MOVING) {
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
                        bool faceBelongsToThisRoom = (Faces.Contains(openingsAttachedFace));
                        if (!faceBelongsToThisRoom) openingsAttachedFace.RemoveOpening(opening);
                    }
                }

                foreach (Room room in Building.Instance.Rooms) {
                    foreach (Face face in room.Faces) {
                        foreach (Opening opening in face.Openings) {
                            opening.AttachClosestFaces();
                        }
                    }
                }
>>>>>>> Stashed changes
            }
        }
    }
    public void SetRoomState(RoomStates roomState) {
        this.roomState = roomState;
    }

<<<<<<< Updated upstream
    /// <summary>
    /// 
    /// </summary>
    public bool GetIsMoveHandleVisible() { return isRoomInMoveMode; }

    /// <summary>
    /// 
    /// </summary>
    public Room GetPrefabRoom() { return prefabRoom; }


    public void SetRoomType(RoomType type) {
        roomType = type;

        switch (type) {
            case RoomType.PREVIEW:
                isHighlighted = true;
                currentMaterial = prefabRoom.highlightMaterial;
                gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
                break;
            case RoomType.DEFAULT:
                currentMaterial = prefabRoom.defaultMaterial;
                gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
                break;
            case RoomType.SINGLEROOM:
                currentMaterial = prefabRoom.singleRoomMaterial;
                gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
                break;
            case RoomType.DOUBLEROOM:
                currentMaterial = prefabRoom.doubleRoomMaterial;
                gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
                break;
            case RoomType.LIVINGROOM:
                currentMaterial = prefabRoom.livingroomMaterial;
                gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
                break;
            case RoomType.KITCHEN:
                currentMaterial = prefabRoom.kitchenMaterial;
                gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
                break;
            case RoomType.BATHROOM:
                currentMaterial = prefabRoom.bathroomMaterial;
                gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
                break;
        }
    }
    public void SetRoomNote(string value) {
        if (!string.IsNullOrEmpty(value)) {
            customProperty = value;
=======
        public void SetIsRoomCurrentlyColliding() {
            if (Type == RoomType.PREVIEW || State == RoomState.MOVING) { 
                UpdateRender2D(highlighted: IsSelected, colliding: IsColliding);
            }
>>>>>>> Stashed changes
        }
    }

    /// <summary>
    /// Finds the boundaries of the room in the X and Y axis.
    /// returns:  { minX, maxX, minY, maxY }
    /// </summary>
    public List<float> Bounds(bool localCoordinates = false) {
        float minX = 0; float maxX = 0;
        float minZ = 0; float maxZ = 0;
        foreach (Vector3 controlPoint in GetControlPoints(localCoordinates: true)) {
            if (controlPoint.x < minX) { minX = controlPoint.x; }
            if (controlPoint.x > maxX) { maxX = controlPoint.x; }
            if (controlPoint.z < minZ) { minZ = controlPoint.z; }
            if (controlPoint.z > maxZ) { maxZ = controlPoint.z; }
        }
        return new List<float> { minX, maxX, minZ, maxZ };
    }

}
