using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Room : MonoBehaviour {
    private List<Vector3> controlPoints;

    public List<Face> faces { get; private set; }

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

    private bool isHighlighted { set; get; }
    private ProBuilderMesh mesh3D;
    private Room prefabRoom;


    private GameObject moveHandle;
    private bool isRoomInMoveMode = false;

    private GameObject editHandle;
    public GameObject editHandlePrefab;
    public List<GameObject> activeEditHandles;


    public GameObject moveHandlePrefab;
    private Vector3 moveModeScreenPoint;
    private Vector3 moveModeOffset;

    private Vector3 lastLegalPlacementPoint;

    public RoomType RoomType { get; private set; }

    public enum RoomStates {
        Stationary,
        Preview,
        Moving
    }
    private RoomStates roomState;

    public bool isCurrentlyColliding = false;

    // Construct room of type 0 (Rectangle) or 1 (L-shape)
    public void InitializeRoom(RoomShape buildShape = RoomShape.RECTANGLE, Building building = null) {
        // Set constant values
        parentBuilding = building;
        gameObject.layer = 8; // Rooom layer

        shape = buildShape;
        roomState = RoomStates.Preview;

        // Get relevant properties from prefab object
        GameObject prefabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RoomPrefab.prefab");

        prefabRoom = (Room)prefabObject.GetComponent(typeof(Room));

        switch (shape) {
            case RoomShape.RECTANGLE:
                controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                   new Vector3(0, 0, 3),
                                                   new Vector3(3, 0, 3),
                                                   new Vector3(3, 0, 0)};
                gameObject.name = "Rectange room (" + (Building.Instance.rooms.Count+1).ToString()+")";
                break;
            case RoomShape.LSHAPE:
                controlPoints = new List<Vector3> {new Vector3(0, 0, 0),
                                                          new Vector3(0, 0, 5),
                                                          new Vector3(3, 0, 5),
                                                          new Vector3(3, 0, 3),
                                                          new Vector3(5, 0, 3),
                                                          new Vector3(5, 0, 0)};
                gameObject.name = "L-room (" + (Building.Instance.rooms.Count+1).ToString()+")";
                break;
        }
        faces = new List<Face>();
        for (int i = 0; i < controlPoints.Count+2; i++) {
            faces.Add(new Face(this, i));
        }

        // Create and attach collider objects
        gameObject.AddComponent<MeshCollider>();


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

    public void RefreshView() {
        PolyShape polyshape = gameObject.GetComponent<PolyShape>();
        polyshape.SetControlPoints(controlPoints);
        polyshape.extrude = height;
        polyshape.CreateShapeFromPolygon();
        gameObject.GetComponent<ProBuilderMesh>().Refresh();
        gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
        RoomCollider.GiveCollider(this);
    }

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

    /// <summary>
    /// Gets a list of normals. They are in same order as controlpoints (clockwise). localCoordinates : true (for local coordinates, Sherlock)
    /// </summary>
    public List<Vector3> GetWallNormals(bool localCoordinates = false) {
        List<Vector3> wallNormals = new List<Vector3>();
        List<Vector3> circularControlpoints = GetControlPoints(localCoordinates: localCoordinates, closed: true);
        for (int i = 0; i < controlPoints.Count; i++) {
            wallNormals.Add(Vector3.Cross((circularControlpoints[i + 1] - circularControlpoints[i]), Vector3.up).normalized);
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
            RefreshView();
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
            moveModeScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
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
                Material meshRenderMaterial = gameObject.GetComponent<MeshRenderer>().material;
                if (isHighlighted) meshRenderMaterial = highlightMaterial;
                else meshRenderMaterial = currentMaterial;

            }
            else { // if there is one or more collision(s)
                //Debug.Log("Is colliding");

                isCurrentlyColliding = true;
                gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }
    public void SetRoomState(RoomStates roomState) {
        this.roomState = roomState;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool GetIsMoveHandleVisible() { return isRoomInMoveMode; }

    /// <summary>
    /// 
    /// </summary>
    public Room GetPrefabRoom() { return prefabRoom; }


    public void SetRoomType(RoomType type) {
        RoomType = type;

        switch (type) {
            case RoomType.PREVIEW:
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
