using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face
{
    public Room parentRoom { get;  private set; }
    public int faceIndex { get; private set; }
    public List<Interface> interfaces { get; private set; }
    public Dictionary<Interface, float[]> paramerters { get; private set; }
    public Orientation orientation { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public Face(Room parent, int index) {
        interfaces = new List<Interface>();
        paramerters = new Dictionary<Interface, float[]>();
        parentRoom = parent;
        faceIndex = index;
        SetOrientation();
    }

    /// <summary>
    /// Determs the orientation of the face (horizontal or vertical)
    /// </summary>
    private void SetOrientation() {
        if (faceIndex < parentRoom.GetControlPoints().Count) {
            orientation = Orientation.VERTICAL;
        }
        else orientation = Orientation.HORIZONTAL;
    }

    /// <summary>
    /// Gets the original controlpoints of the face (horizontal and vertical faces)
    /// </summary>
    public Vector3[] GetOGControlPoints(bool localCoordinates = false) {
        List<Vector3> cp;
        Vector3[] endpoints = new Vector3[1];

        switch (orientation) {
            case Orientation.VERTICAL:
                cp = parentRoom.GetControlPoints(localCoordinates: localCoordinates, closed:true);
                endpoints = new Vector3[2];
                endpoints[0] = cp[faceIndex];
                endpoints[1] = cp[faceIndex + 1];
                break;
            case Orientation.HORIZONTAL:
                cp = parentRoom.GetControlPoints(localCoordinates: localCoordinates);
                endpoints = cp.ToArray();
                break;
            default:
                break;
        }
        
        return endpoints;
    }

    public (Vector3, Vector3) Get2DEndPoints(bool localCoordinates = false) {
        (Vector3, Vector3) endpoints = (new Vector3(), new Vector3());
        if (orientation == Orientation.VERTICAL) {
            List<Vector3> cp = parentRoom.GetControlPoints(localCoordinates: localCoordinates, closed: true);
            endpoints.Item1 = cp[faceIndex];
            endpoints.Item2 = cp[faceIndex + 1];
        }
        return endpoints;
    }


    /// <summary>
    /// Add a interface to a face
    /// </summary>
    public void AddInterface(Interface interFace, float startParameter = 0.0f, float endParameter = 1.0f) {
        interfaces.Add(interFace);
        paramerters.Add(interFace, new float[] { startParameter, endParameter });
    }

    /// <summary>
    /// Remove the interface 
    /// </summary>
    public void RemoveInterface(Interface interFace) {
        if(interfaces.Contains(interFace)) interfaces.Remove(interFace);
    }

    public bool CollidesWith(Vector3 point) {
        (Vector3 startPoint, Vector3 endPoint) = Get2DEndPoints(localCoordinates: false);
        float dxc = point.x - startPoint.x;
        float dyc = point.y - startPoint.y;
        float dxl = endPoint.x - startPoint.x;
        float dyl = endPoint.y - startPoint.y;
        float cross = dxc * dyl - dyc * dxl;

        // The point is on the line
        if (cross < 0.001f) {

            // Compare x
            if (Mathf.Abs(dxl) >= Mathf.Abs(dyl))
                return dxl > 0 ?
                  startPoint.x <= point.x && point.x <= endPoint.x :
                  endPoint.x <= point.x && point.x <= startPoint.x;

            // Compare y
            else
                return dyl > 0 ?
                  startPoint.z <= point.z && point.z <= endPoint.z :
                  endPoint.z <= point.z && point.z <= startPoint.z;
        }
        else return false;
    }

    public bool CollidesWithGrid(Vector3 point) {
        (Vector3 startPoint, Vector3 endPoint) = Get2DEndPoints(localCoordinates: false);
        
        // The line is along y
        if ( startPoint.x == point.x && endPoint.x == point.x) {
            if (Mathf.Min(startPoint.z,endPoint.z) < point.z
                && Mathf.Max(startPoint.z, endPoint.z) > point.z) {
                return true;
            }
        }
        // The line is along x
        if (startPoint.z == point.z && endPoint.z == point.z) {
            if (Mathf.Min(startPoint.x, endPoint.x) < point.x
                && Mathf.Max(startPoint.x, endPoint.x) > point.x) {
                return true;
            }
        }

        return false;
    }

}
