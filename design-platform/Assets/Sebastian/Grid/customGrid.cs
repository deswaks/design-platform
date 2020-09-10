using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customGrid : MonoBehaviour
{
    public GameObject placedObject;
    public float gridSize;

    Vector3 gridPosition;
    Vector3 mousePosition;

    void LateUpdate()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        gridPosition.x = Mathf.Floor(mousePosition.x / gridSize) * gridSize;
        gridPosition.y = Mathf.Floor(mousePosition.y / gridSize) * gridSize;
        gridPosition.z = Mathf.Floor(mousePosition.z / gridSize) * gridSize;

        placedObject.transform.position = gridPosition;
    }
}
