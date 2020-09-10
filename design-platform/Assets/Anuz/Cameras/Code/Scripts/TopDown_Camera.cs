using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDown_Camera : MonoBehaviour
{
    // Variables
    public Transform camera_target;
    // public float camera_Height = 10f;
    //public float camera_Distance = 20f;
    //public float camera_Angle =45f;
    //https://www.youtube.com/watch?v=oknQEX60nKY

    // Start is called before the first frame update
    void Start()
    {
        HandleCamera();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCamera();
    }


    protected virtual void HandleCamera()
    {
        if (!camera_target)
        {
            return;
        }
        Vector3 flatTargetPosition = camera_target.position;
        transform.LookAt(flatTargetPosition);


    }
}
