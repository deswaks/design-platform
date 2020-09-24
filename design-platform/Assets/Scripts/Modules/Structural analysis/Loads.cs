using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loads
{

    public bool IsHorizontalShortestSpan(Room room) {
        List<Vector3> controlPoints = room.GetControlPoints(closed: true);
        float horizontalDistance = 0;
        float verticalDistance = 0;
        for (int i = 0; i < controlPoints.Count; i++) {

        }
        return true;
    }
}
