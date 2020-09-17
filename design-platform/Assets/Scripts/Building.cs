﻿using System;
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
    public GameObject roomPrefab;

    private GameObject previewObject; // Object that will be moving around in the scene
    private GameObject currentlySelectedObject; // The currently selected room (that's already placed)
    private List<Room> rooms = new List<Room>();

    private void Awake()
    {
        grid = FindObjectOfType<Grid>();
        //Finds rooms already placed
        //rooms = FindObjectsOfType(typeof(GameObject)).Cast<GameObject>().Where(go => go.layer == 8).ToList();
    }

    // Returns a list of all the rooms in the building
    public List<Room> GetAllRooms()
    {
        return rooms;
    }

    // Removes room from the list of rooms
    public void RemoveRoom(Room room)
    {
        rooms.Remove(room);
    }

    public Room BuildRoom(int shape = 0, bool preview = false, Room templateRoom = null)
    {
        GameObject newRoomGameObject = new GameObject();
        Room newRoom = (Room)newRoomGameObject.AddComponent(typeof(Room));
        newRoom.InitializeRoom(shape: shape, building:this);

        if(templateRoom != null)
        {
            newRoomGameObject.transform.position = templateRoom.transform.position;
            newRoomGameObject.transform.rotation = templateRoom.transform.rotation;
        }
        
        rooms.Add(newRoom);
        return newRoom;
    }
}


