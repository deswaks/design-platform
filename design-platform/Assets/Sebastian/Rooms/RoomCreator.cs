using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCreator : MonoBehaviour
{
    public GameObject meshPrefab;
    Room activeMesh;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (activeMesh == null)
            {
                GameObject meshGameObject = Instantiate(meshPrefab);
                activeMesh = meshGameObject.GetComponent<Room>();
                activeMesh.UpdateRoom(mouse_grid_position());
            }
            else
            {
                activeMesh.UpdateRoom(mouse_grid_position());
                activeMesh = null;
            }
        }
    }

    Vector3 mouse_grid_position()
    {
        Vector3 gridPosition;
        Vector3 hitPoint = new Vector3(0,0,0);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        Plane basePlane = new Plane(Vector3.up, new Vector3(0,1,0));
        if (basePlane.Raycast(ray, out distance))
        {
            hitPoint = ray.GetPoint(distance);
        }
        float gridSize = 10;
        gridPosition.x = Mathf.Round(hitPoint[0] / gridSize) * gridSize;
        gridPosition.y = Mathf.Round(hitPoint[1] / gridSize) * gridSize;
        gridPosition.z = Mathf.Round(hitPoint[2] / gridSize) * gridSize;

        return gridPosition;
    }
}