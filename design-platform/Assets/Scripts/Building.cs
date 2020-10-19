using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

public class Building {
    private static Building instance;
    public List<Room> rooms { get; private set; }
    public List<Wall> walls { get; private set; }
    public List<Interface> interfaces { get; private set; }

    public static Building Instance {
        // Use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new Building()); }
    }

    public Building() {
        rooms = new List<Room>();
        walls = new List<Wall>();
        interfaces = new List<Interface>();
    }

    // Returns a list of all the rooms in the building
    public List<Room> GetRooms() {
        return rooms;
    }

    // Removes room from the list of rooms
    public void RemoveRoom(Room room) {
        if (rooms.Contains(room)) { rooms.Remove(room); }
    }
    // Removes room from the list of rooms
    public void RemoveInterface(Interface interFace) {
        if (interfaces.Contains(interFace)) { interfaces.Remove(interFace); }
    }


    /// <summary>
    /// Builds a new room
    /// </summary>
    public Room BuildRoom(RoomShape buildShape = RoomShape.RECTANGLE, bool preview = false, Room templateRoom = null) {
        GameObject newRoomGameObject = new GameObject("Room");
        Room newRoom = (Room)newRoomGameObject.AddComponent(typeof(Room));
        newRoom.InitializeRoom(buildShape: buildShape, building: this);
        if (preview) { newRoomGameObject.name = "Preview room"; }

        if (templateRoom != null) {
            newRoomGameObject.transform.position = templateRoom.transform.position;
            newRoomGameObject.transform.rotation = templateRoom.transform.rotation;
        }

        if (preview == false) { rooms.Add(newRoom); }

        return newRoom;
    }

    /// <summary>
    /// Finds the boundaries of the building in the X and Y axis.
    /// returns:  { minX, maxX, minY, maxY }
    /// </summary>
    public List<float> Bounds() {
        float minX = 0; float maxX = 0;
        float minY = 0; float maxY = 0;
        foreach (Room room in rooms) {
            foreach (Vector3 controlPoint in room.GetControlPoints()) {
                if (controlPoint[0] < minX) { minX = controlPoint[0]; }
                if (controlPoint[0] > maxX) { minX = controlPoint[0]; }
                if (controlPoint[2] < minY) { minX = controlPoint[2]; }
                if (controlPoint[2] > maxY) { minX = controlPoint[2]; }
            }
        }
        return new List<float> { minX, maxX, minY, maxY };
    }

    /// <summary>
    /// Builds a new wall
    /// </summary>
    public Wall BuildWall(Interface interFace) {
        GameObject newWallGameObject = new GameObject("Wall");
        Wall newWall = (Wall)newWallGameObject.AddComponent(typeof(Wall));

        newWall.InitializeWall(interFace);

        walls.Add(newWall);

        return newWall;
    }

    /// <summary>
    /// Get a list of builded walls
    /// </summary>
    public List<Wall> GetWalls() {
        return walls;
    }

    /// <summary>
    /// Removes wall from the list of walls
    /// </summary>
    public void RemoveWall(Wall wall) {
        if (walls.Contains(wall)) walls.Remove(wall);
    }

    /// <summary>
    /// Removes ALL walls
    /// </summary>
    public void DeleteAllWalls() {
        int amount = walls.Count;
        for (int i = 0; i < amount; i++) {
            walls[0].DeleteWall();
        }
    }

    /// <summary>
    /// Removes ALL interfaces
    /// </summary>
    public void DeleteAllInterfaces() {
        int amount = interfaces.Count;
        for (int i = 0; i < amount; i++) {
            interfaces[0].Delete();
        }
    }

    public void CreateInterfaces() {
        // For all faces
        for (int r = 0; r < rooms.Count; r++) {
            for (int f = 0; f < rooms[r].faces.Count; f++) {

                Face face = rooms[r].faces[f];

                // Skip if face is not a wall
                if (face.orientation != Orientation.VERTICAL) continue;

                // Find points on face line from the controlpoints of all other rooms
                List<Vector3> splitPoints = new List<Vector3>();
                for (int r2 = 0; r2 < rooms.Count; r2++) {
                    if (r == r2) continue;
                    foreach (Vector3 point in rooms[r2].GetControlPoints()) {
                        if (face.CollidesWithGrid(point)) {
                            splitPoints.Add(point);
                        }
                    }
                }

                // Sort splitpoints between startpoint and endpoint
                (Vector3 startPoint, Vector3 endPoint) = face.Get2DEndPoints(localCoordinates: false);
                splitPoints.Add(startPoint);
                splitPoints.Add(endPoint);
                splitPoints = splitPoints.OrderBy(p => (p - startPoint).magnitude).ToList();
                List<float> splitParameters = splitPoints.Select(p => (p - startPoint).magnitude).ToList();
                splitParameters = RangeUtils.Reparametrize(splitParameters, splitParameters[0], splitParameters[splitParameters.Count-1]);

                
                // Hvert interface-sted
                for (int i = 0; i < splitParameters.Count-1; i++) {

                    // Check if an interface exists with the same points
                    Vector3 ifStartPoint = splitPoints[i];
                    Vector3 ifEndPoint = splitPoints[i+1];
                    Interface existingInterface = null;
                    foreach (Interface interFace in interfaces) {
                        if (interFace.GetStartPoint() == ifStartPoint || interFace.GetEndPoint() == ifEndPoint
                            && interFace.GetStartPoint() == ifStartPoint || interFace.GetEndPoint() == ifEndPoint) {
                            existingInterface = interFace;
                        }
                    }

                    // Attach to existing interface
                    if (existingInterface != null) {
                        existingInterface.attachedFaces[1] = face;
                        face.AddInterface(existingInterface, splitParameters[i], splitParameters[i + 1]);
                    }

                    // Create new interface
                    else {
                        Interface interfac = new Interface();
                        interfac.attachedFaces[0] = face;
                        interfaces.Add(interfac);
                        face.AddInterface(interfac, splitParameters[i], splitParameters[i + 1]);
                    }
                }
                

            }
        }
    }
}


