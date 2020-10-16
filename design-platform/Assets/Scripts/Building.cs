using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

public class Building
{
    private static Building instance;
    private List<Room> rooms = new List<Room>();
    
    private List<Wall> walls = new List<Wall>();

    public static Building Instance {
        // Use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new Building()); }
    }

    // Returns a list of all the rooms in the building
    public List<Room> GetRooms()
    {
        return rooms;
    }

    // Removes room from the list of rooms
    public void RemoveRoom(Room room)
    {
        if (rooms.Contains(room)) { rooms.Remove(room); }
    }

    /// <summary>
    /// Builds a new room
    /// </summary>
    public Room BuildRoom(RoomShape buildShape = RoomShape.RECTANGLE, bool preview = false, Room templateRoom = null)
    {
        GameObject newRoomGameObject = new GameObject("Room");
        Room newRoom = (Room)newRoomGameObject.AddComponent(typeof(Room));
        newRoom.InitializeRoom(buildShape: buildShape, building: this);
        if (preview) { newRoomGameObject.name = "Preview room"; }

        if (templateRoom != null)
        {
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
    public Wall BuildWall(List<Vector3> startEndPoints, Vector3 normal, Room room = null) {
        GameObject newWallGameObject = new GameObject("Wall");
        Wall newWall = (Wall)newWallGameObject.AddComponent(typeof(Wall));
        newWall.InitializeWall(startEndPoints, normal, room);
     
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
        if (walls.Contains(wall)) { walls.Remove(wall); }
    }
}


