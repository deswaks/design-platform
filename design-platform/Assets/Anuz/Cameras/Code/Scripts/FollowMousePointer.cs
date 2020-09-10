using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMousePointer : MonoBehaviour
{
    public Transform pointer;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    void FixedUpdate()
    {
        Vector3 desiredPosition = pointer.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(pointer);
    }
}
