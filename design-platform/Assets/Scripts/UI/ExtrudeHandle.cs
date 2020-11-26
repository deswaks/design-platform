﻿using DesignPlatform.Core;
using DesignPlatform.Utils;
using UnityEngine;

namespace DesignPlatform.UI {
    public class ExtrudeHandle : MonoBehaviour {
        public Room parentRoom;
        public int wallIndex;

        public Vector3 wallNormal;

        public void InitHandle(int wall) {
            parentRoom = gameObject.transform.parent.gameObject.GetComponent<Room>();
            wallIndex = wall;
            wallNormal = parentRoom.GetWallNormals()[wallIndex];
            gameObject.name = "Extrude handle";
            UpdateTransform(updateRotation: true);
            gameObject.AddComponent<BoxCollider>();

        }

        public void OnMouseDown() {
            //Debug.Log("Extruding wall:" + wallIndex.ToString() + " in direction:" + wallNormal.ToString());
        }

        public void OnMouseDrag() {
            Vector3 handleStartPosition = transform.position;
            Vector3 mouseGridPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 diffPosition = Core.Grid.GetNearestGridpoint(mouseGridPosition - handleStartPosition);

            Vector3 extrusionVector = Vector3.Scale(Vector3.Project(diffPosition, wallNormal), wallNormal);
            float extrusion = extrusionVector[VectorFunctions.IndexAbsLargestComponent(extrusionVector)];

            //transform.position = handleStartPosition + Distance;
            parentRoom.ExtrudeWall(wallIndex, extrusion);

            foreach (ExtrudeHandle handle in parentRoom.GetComponentsInChildren<ExtrudeHandle>()) {
                handle.UpdateTransform();
            }
        }

        public void UpdateTransform(bool updatePosition = true, bool updateRotation = false) {
            if (updatePosition) {
                transform.position = parentRoom.GetWallMidpoints()[wallIndex] + Vector3.up * (parentRoom.height + 0.05f);
            }
            if (updateRotation) {
                transform.RotateAround(
                point: parentRoom.GetWallMidpoints()[wallIndex],
                axis: new Vector3(0, 1, 0),
                angle: Vector3.SignedAngle(new Vector3(1, 0, 0), parentRoom.GetWallNormals()[wallIndex], new Vector3(0, 1, 0))
            );
            }
        }
    }
}