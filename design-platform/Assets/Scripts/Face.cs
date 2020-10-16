using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    private Room parentRoom;
    private int faceIndex;
    private List<Interface> interfaces { get; }
    private Dictionary<Interface, float[]> paramerters;
    private Orientation orientation;

    public Face(Room parent, int index) {
        parentRoom = parent;
        faceIndex = index;
        SetOrientation();
    }

    private void SetOrientation() {
        if (faceIndex < parentRoom.GetControlPoints().Count) {
            orientation = Orientation.VERTICAL;
        }
        else orientation = Orientation.HORIZONTAL;
    }

    public Vector3[] GetOGControlPoints(bool localCoordinates = false) {
        List<Vector3> cp;
        Vector3[] endpoints = new Vector3[1];

        switch (orientation) {
            case Orientation.HORIZONTAL:
                cp = parentRoom.GetControlPoints(localCoordinates: localCoordinates, closed:true);
                endpoints = new Vector3[2];
                endpoints[0] = cp[faceIndex];
                endpoints[1] = cp[faceIndex + 1];
                break;
            case Orientation.VERTICAL:
                cp = parentRoom.GetControlPoints(localCoordinates: localCoordinates);
                endpoints = cp.ToArray();
                break;
            default:
                break;
        }
        
        return endpoints;
    }

    public void AddInterface(Interface interFac) {
        interfaces.Add(interFac);
    }
}
