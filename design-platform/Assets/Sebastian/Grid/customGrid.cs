using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customGrid : MonoBehaviour
{
    public GameObject placedObject;
    public float gridSize;
    public GameObject basePlane;

    Vector3 gridPosition;
    Vector3 worldPosition;

    void LateUpdate()
    {
        //create new ray from camera mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Check ray plane collision and save distance
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Find world coordinates of ray plane collision
            worldPosition = hit.point;
        }

        gridPosition.x = Mathf.Round(worldPosition.x / gridSize) * gridSize;
        gridPosition.y = Mathf.Round(worldPosition.y / gridSize) * gridSize;
        gridPosition.z = Mathf.Round(worldPosition.z / gridSize) * gridSize;

        placedObject.transform.position = gridPosition;
    }
}
