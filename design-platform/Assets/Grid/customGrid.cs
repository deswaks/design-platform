using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customGrid : MonoBehaviour
{
    public GameObject placedObject;
    public float gridSize;

    Vector3 truePos;

    void LateUpdate()
    {
        truePos.x = Mathf.Floor(placedObject.transform.position.x / gridSize) * gridSize;
        truePos.y = Mathf.Floor(placedObject.transform.position.y / gridSize) * gridSize;
        truePos.z = Mathf.Floor(placedObject.transform.position.z / gridSize) * gridSize;

        placedObject.transform.position = truePos;
    }
}
