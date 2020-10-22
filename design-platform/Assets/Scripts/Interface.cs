using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interface {
    public Face[] attachedFaces = new Face[2];
    public Wall wall;

    public override string ToString() {
        string textString = "Interface: ";
        if (attachedFaces[0] != null) textString += attachedFaces[0].ToString();
        if (attachedFaces[1] != null) textString += " and " + attachedFaces[1].ToString();
        return textString;
    }

    public Vector3 GetStartPoint(bool localCoordinates = false) {
        float[] parameters = attachedFaces[0].paramerters[this];
        (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
        Vector3 startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[0];

        return startPoint;
    }

    public Vector3 GetEndPoint(bool localCoordinates = false) {
        float[] parameters = attachedFaces[0].paramerters[this];
        (Vector3 fStartPoint, Vector3 fEndPoint) = attachedFaces[0].Get2DEndPoints(localCoordinates: localCoordinates);
        Vector3 startPoint = fStartPoint + (fEndPoint - fStartPoint) * parameters[1];

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

    /// <summary>
    /// 
    /// </summary>
    public float GetWallThickness() {
        float[] thicknesses = attachedFaces.Where(f => f != null).Select(f => f.wallThickness).ToArray();
        return thicknesses.Max();
    }

}
