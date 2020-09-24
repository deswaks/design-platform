using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

public class Building : MonoBehaviour
{
    public Camera cam;
    public GameObject moveHandlePrefab;
    public Grid grid;
    private List<Room> rooms;

    private void Awake()
    {
        grid = FindObjectOfType<Grid>();

        //Finds rooms already placed
        rooms = FindObjectsOfType<GameObject>().ToList().Where(go => go.layer == 8).Select(go => go.GetComponent<Room>()).ToList();
    }

    // Returns a list of all the rooms in the building
    public List<Room> GetAllRooms()
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
        GameObject newRoomGameObject = new GameObject(name);
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
}


