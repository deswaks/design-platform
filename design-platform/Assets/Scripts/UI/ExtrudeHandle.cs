using DesignPlatform.Utils;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// Handle for dragging the faces of spaces to manipulate their geometry.
    /// </summary>
    public class ExtrudeHandle : MonoBehaviour {
        public Core.Space parentSpace;
        public int wallIndex;

        public Vector3 wallNormal;

        public void InitHandle(int wall) {
            parentSpace = gameObject.transform.parent.gameObject.GetComponent<Core.Space>();
            wallIndex = wall;
            wallNormal = parentSpace.GetWallNormals()[wallIndex];
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
            parentSpace.ExtrudeWall(wallIndex, extrusion);

            foreach (ExtrudeHandle handle in parentSpace.GetComponentsInChildren<ExtrudeHandle>()) {
                handle.UpdateTransform();
            }
        }

        public void UpdateTransform(bool updatePosition = true, bool updateRotation = false) {
            if (updatePosition) {
                transform.position = parentSpace.GetWallMidpoints()[wallIndex] + Vector3.up * (parentSpace.height + 0.05f);
            }
            if (updateRotation) {
                transform.RotateAround(
                point: parentSpace.GetWallMidpoints()[wallIndex],
                axis: new Vector3(0, 1, 0),
                angle: Vector3.SignedAngle(new Vector3(1, 0, 0), parentSpace.GetWallNormals()[wallIndex], new Vector3(0, 1, 0))
            );
            }
        }
    }
}