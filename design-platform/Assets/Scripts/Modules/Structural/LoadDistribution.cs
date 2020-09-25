using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Structural {

    public class LoadDistribution {

        /// <summary>
        /// Used to find out which direction the loads form above the room should be distributed in
        /// </summary>
        /// <param name="room">The room to analyze</param>
        /// <returns>The cumulative spans in each direction</returns>
        public Dictionary<Axis, float> Spans(Room room) {
            List<Vector3> controlPoints = room.GetControlPoints(localCoordinates: true, closed: true);
            Dictionary<Axis, float> spans = new Dictionary<Axis, float>() { { Axis.X, 0.0f }, { Axis.Y , 0.0f}, {Axis.Z , 0.0f}  };
            for (int i = 0; i < controlPoints.Count - 1; i++) {
                Vector3 wallVector = (controlPoints[i] - controlPoints[i + 1]);
                spans[Axis.X] += Mathf.Abs(wallVector[0]);
                spans[Axis.Y] += Mathf.Abs(wallVector[1]);
                spans[Axis.Z] += Mathf.Abs(wallVector[2]);
            }
            return spans;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public Dictionary<int, List<Load>> AreaLoad(Room room) {
            if (room.GetRoomShape() == RoomShape.RECTANGLE) { return AreaLoadRectangle(room); }
            else { return new Dictionary<int, List<Load>>(); }
        }

        public Dictionary<int, List<Load>> AreaLoadRectangle(Room room) {
            Dictionary<int, List<Load>> loadAreas = new Dictionary<int, List<Load>>();
            List<Vector3> points = room.GetControlPoints();

            Dictionary<Axis, float> spans = Spans(room);
            // Distribute loads along X-axis
            if (spans[Axis.X] <= spans[Axis.Z]) {
                float loadDistance = Vector3.Distance(points[1], points[2]);
                loadAreas[0].Add(new Load(0, 1, loadDistance / 2));
                loadAreas[2].Add(new Load(0, 1, loadDistance / 2));
            }
            // Distribute loads along Z-axis
            else {
                float loadDistance = Vector3.Distance(points[0], points[1]);
                loadAreas[1].Add(new Load(0, 1, loadDistance / 2));
                loadAreas[3].Add(new Load(0, 1, loadDistance / 2));
            }

            return loadAreas;
        }

        public Dictionary<int, List<Load>> AreaLoadLShape(Room room) {
            Dictionary<int, List<Load>> loadAreas = new Dictionary<int, List<Load>>();
            List<Vector3> points = room.GetControlPoints();

            // Classify walls according to span
            List<int> loadCarryingWalls = new List<int>();
            Dictionary<Axis, float> spans = Spans(room);
            for (int i = 0; i < room.GetControlPoints().Count; i++) {
                if (spans[Axis.X] > spans[Axis.Z] && i%2 == 0) { loadCarryingWalls.Add(i); }
                if (spans[Axis.X] < spans[Axis.Z] && i%2 == 1) { loadCarryingWalls.Add(i); }
            }

            List<Vector3> normals = room.GetNormals();
            foreach (int indexWall in loadCarryingWalls) {

            }

            return loadAreas;
        }
    }
}
