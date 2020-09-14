using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCreator : MonoBehaviour
{
    public GameObject roomPrefab;
    public float gridSize = 1;
    Room activeRoom;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (activeRoom == null)
            {
                GameObject meshGameObject = Instantiate(roomPrefab);
                activeRoom = meshGameObject.GetComponent<Room>();
                activeRoom.Begin(mouse_grid_position());
            }
            else
            {
                activeRoom.UpdateRoom(mouse_grid_position());
                activeRoom = null;
            }
        }
        else if (activeRoom != null)
        {
            activeRoom.UpdateRoom(mouse_grid_position());
        }
    }

    Vector3 mouse_grid_position()
    {
        Vector3 gridPosition;
        Vector3 hitPoint = new Vector3(0, 0, 0);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        Plane basePlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        if (basePlane.Raycast(ray, out distance))
        {
            hitPoint = ray.GetPoint(distance);
        }
        
        gridPosition.x = Mathf.Round(hitPoint[0] / gridSize) * gridSize;
        gridPosition.y = Mathf.Round(hitPoint[1] / gridSize) * gridSize;
        gridPosition.z = Mathf.Round(hitPoint[2] / gridSize) * gridSize;

        return gridPosition;
    }
}