using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface {
    public Face[] attachedFaces = new Face[2];
    public Wall wall;

    public Vector3 GetStartPoint(bool localCoordinates = false) {
        float[] parameters = attachedFaces[0].paramerters[this];
        (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
        Vector3 startPoint = (fEndPoint - fStartPoint).normalized * parameters[0];

        return startPoint;
    }

    public Vector3 GetEndPoint(bool localCoordinates = false) {
        float[] parameters = attachedFaces[0].paramerters[this];
        (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
        Vector3 startPoint = (fEndPoint - fStartPoint).normalized * parameters[1];

        return startPoint;
    }

    /// <summary>
    /// Deletes the room
    /// </summary>
    public void Delete() {
        if (Building.Instance.interfaces.Contains(this)) {
            Building.Instance.RemoveInterface(this);
        }
    }
}
