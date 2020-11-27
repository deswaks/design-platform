using DesignPlatform.Core;
using DesignPlatform.Geometry;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Utils {

    /// <summary>
    /// A collider object consisting of multiple cubic collider objects to represent complex spaces.
    /// </summary>
    public class SpaceCollider : MonoBehaviour {
        public bool isCurrentlyColliding = false;

        //Detects collision with other spaces when placing them
        void OnCollisionEnter(Collision other) {
            CheckCollisionWithSpaceColliders(other, isColliding: true);
        }

        void OnCollisionStay(Collision other) {
            CheckCollisionWithSpaceColliders(other, isColliding: true);
        }

        void OnCollisionExit(Collision other) {
            CheckCollisionWithSpaceColliders(other, isColliding: false);
        }

        void CheckCollisionWithSpaceColliders(Collision other, bool isColliding) {

            GameObject parentObject = gameObject.transform.parent.gameObject;
            // Only acts if collision object is other space object and not a sibling (other colliders in same space)
            if (other.gameObject.GetComponent<SpaceCollider>() && other.gameObject.transform.parent != parentObject.transform) {
                isCurrentlyColliding = isColliding;
            }
            parentObject.GetComponent<Core.Space>().SetIsSpaceCurrentlyColliding();
        }


        public static void GiveCollider(Core.Space space) {

            // Remove existing colliders
            foreach (SpaceCollider spaceCollider in space.GetComponentsInChildren<SpaceCollider>()) {
                Destroy(spaceCollider.gameObject);
            }

            // Every collider cube is defined by the controlpoints of the space object. Both the x- and y- position of the collider cube is defined by two indices each.
            // For instance, the location of the collider cube for a rectangular space is defined as follows: 
            //      The x coordinate of the location is defined by the average value from the [1] and [2] controlpoints (or [0] and [3]).
            //      The y coordinate of the location is defined by the average value from the [0] and [1] controlpoints (or [2] and [3]).
            List<List<int>> colliderIndexPairsList = new List<List<int>>();

            switch (space.Shape) {
                case SpaceShape.RECTANGLE:
                    //colliderIndexPairsList.Add(new List<int> { x1, x2, y1, y2 });
                    colliderIndexPairsList.Add(new List<int> { 1, 2, 0, 1 });
                    break;
                case SpaceShape.LSHAPE:
                    colliderIndexPairsList.Add(new List<int> { 1, 2, 0, 1 }); // Collider cube 1
                    colliderIndexPairsList.Add(new List<int> { 3, 4, 0, 3 }); // Collider cube 2
                    break;
                case SpaceShape.USHAPE:
                    colliderIndexPairsList.Add(new List<int> { 1, 2, 0, 1 }); // Collider cube 1
                    colliderIndexPairsList.Add(new List<int> { 3, 4, 0, 3 }); // Collider cube 2                    
                    colliderIndexPairsList.Add(new List<int> { 5, 6, 7, 6 }); // Collider cube 3                    
                    break;
                case SpaceShape.SSHAPE:
                    colliderIndexPairsList.Add(new List<int> { 1, 2, 3, 2 }); // Collider cube 1
                    colliderIndexPairsList.Add(new List<int> { 0, 4, 0, 3 }); // Collider cube 2                    
                    colliderIndexPairsList.Add(new List<int> { 6, 5, 6, 7 }); // Collider cube 3                    
                    break;
                case SpaceShape.TSHAPE:
                    colliderIndexPairsList.Add(new List<int> { 1, 0, 1, 2 }); // Collider cube 1
                    colliderIndexPairsList.Add(new List<int> { 7, 6, 6, 2 }); // Collider cube 2                    
                    colliderIndexPairsList.Add(new List<int> { 5, 4, 4, 3 }); // Collider cube 3                    
                    break;
            }

            int counter = 0;
            List<Vector3> vertices = space.GetControlPoints(localCoordinates: true);
            foreach (List<int> xyPairs in colliderIndexPairsList) {
                GameObject colliderObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                colliderObject.layer = 8;
                colliderObject.name = "SpaceCollider_" + counter++.ToString();

                Vector3 positionVector =
                    new Vector3(
                        vertices[xyPairs[0]].x + System.Math.Abs(vertices[xyPairs[0]].x - vertices[xyPairs[1]].x) / 2, // x - location
                        space.height / 2,                                                                                    // y - location
                        vertices[xyPairs[2]].z + System.Math.Abs(vertices[xyPairs[2]].z - vertices[xyPairs[3]].z) / 2  // z - location
                        );
                Vector3 scaleVector =
                    new Vector3(
                        System.Math.Abs(vertices[xyPairs[0]].x - vertices[xyPairs[1]].x), // x - scale
                        space.height,                                                       // y - scale
                        System.Math.Abs(vertices[xyPairs[2]].z - vertices[xyPairs[3]].z)  // z - scale
                        );

                colliderObject.transform.parent = space.gameObject.transform;
                colliderObject.transform.localScale = scaleVector * 0.99f;
                colliderObject.transform.localPosition = positionVector;
                colliderObject.transform.rotation = space.transform.rotation;

                colliderObject.GetComponent<BoxCollider>().isTrigger = false;
                Rigidbody rigidBody = colliderObject.AddComponent<Rigidbody>();
                rigidBody.isKinematic = false;
                rigidBody.constraints = RigidbodyConstraints.FreezeAll;

                colliderObject.AddComponent<SpaceCollider>();
                Destroy(colliderObject.GetComponent<MeshRenderer>());
            }
        }
    }
}