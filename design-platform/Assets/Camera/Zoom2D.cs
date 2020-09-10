using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom2D : MonoBehaviour
{
    float scroll;
    float size;
    public float speed;
    void Update()
    {
        scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            size = GetComponent<Camera>().orthographicSize;
            
            if (scroll > 0 && size >= 5.0)
            {
                GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - speed;
            }
            
            if (scroll < 0 && size <= 200.0)
            {
                GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + speed;
            }
        }
    }
}
