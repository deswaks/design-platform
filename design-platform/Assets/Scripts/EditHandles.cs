using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditHandles : MonoBehaviour {

    private Vector3 wallNormalDirection;
    public int selectedIndex;
    public Room parentRoom;
    public int wallIndex;

    public void InitializeHandle(int wall) {
        parentRoom = gameObject.transform.parent.gameObject.GetComponent<Room>();
        wallIndex = wall;
    }

    public void OnMouseDown() {

        //Index of the selected handle
        selectedIndex = parentRoom.activeEditHandles.IndexOf(gameObject);
        //Normal of the selected handles wall
        wallNormalDirection = parentRoom.GetWallNormals()[selectedIndex];
    }

    public void OnMouseDrag() {
        Vector3 handleStartPosition = gameObject.transform.position;
        Vector3 diffPosition = Grid.GetNearestGridpoint(
           Camera.main.ScreenToWorldPoint(Input.mousePosition) - handleStartPosition);
        Vector3 Distance = Vector3.Project(diffPosition, wallNormalDirection);

        //transform.position = handleStartPosition + Distance;
        parentRoom.ExtrudeWall(selectedIndex, Distance);
        //Debug.Log(selectedIndex);
        UpdateTransform();

        foreach (EditHandles handle in parentRoom.GetComponentsInChildren<EditHandles>()) {
            handle.UpdateTransform();
        }
    }

    public void UpdateTransform(bool updatePosition = true, bool updateRotation = false) {
        if (updatePosition) {
            transform.position = parentRoom.GetWallMidpoints()[wallIndex] + new Vector3(0, parentRoom.height + 0.01f, 0);
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
