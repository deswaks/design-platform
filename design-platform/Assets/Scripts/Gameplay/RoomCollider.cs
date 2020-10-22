using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;

public class RoomCollider : MonoBehaviour {
    public bool isCurrentlyColliding = false;

    //Detects collision with other rooms when placing them
    void OnCollisionEnter(Collision other) {
        CheckCollisionWithRoomColliders(other, isColliding: true);
    }

    void OnCollisionStay(Collision other) {
        CheckCollisionWithRoomColliders(other, isColliding: true);
    }

    void OnCollisionExit(Collision other) {
        CheckCollisionWithRoomColliders(other, isColliding: false);
    }

    void CheckCollisionWithRoomColliders(Collision other, bool isColliding) {  

        GameObject parentObject = gameObject.transform.parent.gameObject;
        // Only acts if collision object is other room room object and not a sibling (other colliders in same room)
        if (other.gameObject.GetComponent<RoomCollider>() && other.gameObject.transform.parent != parentObject.transform) {
            isCurrentlyColliding = isColliding;
            //Debug.Log("Is colliding with "+other.gameObject.name);

        }
        parentObject.GetComponent<Room>().SetIsRoomCurrentlyColliding();
    }


    public static void GiveCollider(Room room) {

        // Remove existing colliders
        foreach (RoomCollider roomCollider in room.GetComponentsInChildren<RoomCollider>()) {
            Destroy(roomCollider.gameObject);
        }

        // Every collider cube is defined by the controlpoints of the room object. Both the x- and y- position of the collider cube is defined by two indices each.
        // For instance, the location of the collider cube for a rectangular room is defined as follows: 
        //      The x coordinate of the location is defined by the average value from the [1] and [2] controlpoints (or [0] and [3]).
        //      The y coordinate of the location is defined by the average value from the [0] and [1] controlpoints (or [2] and [3]).
        List<List<int>> colliderIndexPairsList = new List<List<int>>();

        switch (room.GetRoomShape()) {
            case RoomShape.RECTANGLE:
                //colliderIndexPairsList.Add(new List<int> { x1, x2, y1, y2 });
                colliderIndexPairsList.Add(new List<int> { 1, 2, 0, 1 });

                break;
            case RoomShape.LSHAPE:
                colliderIndexPairsList.Add(new List<int> { 1, 2, 0, 1 }); // Collider cube 1
                colliderIndexPairsList.Add(new List<int> { 3, 4, 0, 3 }); // Collider cube 2
                break;
        }

        int counter = 0;
        List<Vector3> vertices = room.GetControlPoints(localCoordinates: true);
        foreach (List<int> xyPairs in colliderIndexPairsList) {
            GameObject colliderObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderObject.name = "RoomCollider_" + counter++.ToString();

            Vector3 positionVector =
                new Vector3(
                    vertices[xyPairs[0]].x + System.Math.Abs(vertices[xyPairs[0]].x - vertices[xyPairs[1]].x) / 2, // x - location
                    room.height / 2,                                                                                    // y - location
                    vertices[xyPairs[2]].z + System.Math.Abs(vertices[xyPairs[2]].z - vertices[xyPairs[3]].z) / 2  // z - location
                    );
            Vector3 scaleVector =
                new Vector3(
                    System.Math.Abs(vertices[xyPairs[0]].x - vertices[xyPairs[1]].x), // x - scale
                    room.height,                                                       // y - scale
                    System.Math.Abs(vertices[xyPairs[2]].z - vertices[xyPairs[3]].z)  // z - scale
                    );

            colliderObject.transform.parent = room.gameObject.transform;
            colliderObject.transform.localScale = scaleVector * 0.99f;
            colliderObject.transform.localPosition = positionVector;
            colliderObject.transform.rotation = room.transform.rotation;

            colliderObject.GetComponent<BoxCollider>().isTrigger = false;
            Rigidbody rigidBody = colliderObject.AddComponent<Rigidbody>();
            rigidBody.isKinematic = false;
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;

            colliderObject.AddComponent<RoomCollider>();
            Destroy(colliderObject.GetComponent<MeshRenderer>());
        }
    }
}
