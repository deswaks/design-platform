using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Structural {

    public static class LoadDistribution {

        /// <summary>
        /// Used to find out which direction the loads form above the room should be distributed in
        /// </summary>
        /// <param name="room">The room to analyze</param>
        /// <returns>The cumulative spans in each direction</returns>
        public static List<float> Spans(Room room) {
            List<Vector3> controlPoints = room.GetControlPoints(localCoordinates: true, closed: true);
            List<float> spans = new List<float>() { 0.0f, 0.0f, 0.0f};
            for (int i = 0; i < controlPoints.Count - 1; i++) {
                Vector3 wallVector = (controlPoints[i] - controlPoints[i + 1]);
                spans[0] += Mathf.Abs(wallVector[0]);
                spans[1] += Mathf.Abs(wallVector[1]);
                spans[2] += Mathf.Abs(wallVector[2]);
            }
            return spans;
        }

        public static Dictionary<int, List<Load>> AreaLoad(Room room) {
            Dictionary<int, List<Load>> loadTables = WallLoadTables(room);
            List<Vector3> points = room.GetControlPoints(localCoordinates: true);
            List<Vector3> normals = room.GetWallNormals(localCoordinates : true);

            for (int iWall = 0; iWall < normals.Count; iWall++) {
                if (loadTables.Keys.Contains(iWall)) {
                    // Dette er en load bearing wall
                }
                else {
                    // Dette er en load giving wall
                    // Hvis naboer har modsat-rettede normaler
                    if (normals[iWall-1] != normals[iWall + 1]) {
                        float loadLength = Vector3.Distance(points[iWall], points[iWall+1]);
                        Load load1 = loadTables[iWall - 1][-1];
                        if (load1.magnitude <= 0.0f) {
                            load1.magnitude = loadLength / 2;
                            loadTables[iWall - 1][-1] = load1;
                        }
                        Load load2 = loadTables[iWall + 1][0];
                        if (load2.magnitude <= 0.0f) {
                            load2.magnitude = loadLength / 2;
                            loadTables[iWall + 1][0] = load2;
                        }

                    }
                }
            }
            return loadTables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room">Room to analyze</param>
        /// <returns></returns>
        public static Dictionary<int, List<Load>> WallLoadTables(Room room) {
            Dictionary<int, List<Load>> WallLoads = new Dictionary<int, List<Load>>();
            List<int> loadCarryingWalls = LoadCarryingWalls(room);
            List<Vector3> points = room.GetControlPoints(localCoordinates: true, closed: true);

            foreach (int wallIndex in loadCarryingWalls) {

                // Find aksen som væggen spænder
                Vector3 wallVector = (points[wallIndex] - points[wallIndex + 1]);
                int wallAxisIndex = VectorFunctions.IndexLargestComponent(wallVector);

                // Find unikke værdier på denne akse og reparameteriser disse over væggens længde
                List<float> uniqueValuesOnWallAxis = room.UniqueCoordinates(localCoordinates: true)[wallAxisIndex];
                List<float> lengthParameters = rangeUtils.reparametrize(uniqueValuesOnWallAxis);

                // Tilføj en load for denne væg mellem hver unik værdi på væggens akse
                for (int i = 0; i < (lengthParameters.Count-1); i++) {
                    WallLoads[wallIndex].Add(new Load(mag: 0.0f,
                                                      start: lengthParameters[i],
                                                      end: lengthParameters[i+1]));
                }
            }
            return WallLoads;
        }

        /// <summary>
        /// Identifies the load carrying walls of a room as the walls that will carry the room area using the shortest span.
        /// </summary>
        /// <param name="room">Room to analyze</param>
        /// <returns>List of indices of the load carrying walls of the room</returns>
        public static List<int> LoadCarryingWalls(Room room) {
            List<int> loadCarryingWalls = new List<int>();
            List<float> spans = Spans(room);

            // Go through all wall indices
            for (int i = 0; i < room.GetControlPoints().Count; i++) {
                // Distribute loads along first room axis
                if (spans[0] > spans[2] && i % 2 == 0) { loadCarryingWalls.Add(i); }
                // Distribute loads along last room axis
                if (spans[0] < spans[2] && i % 2 == 1) { loadCarryingWalls.Add(i); }
            }
            return loadCarryingWalls;
        }
    }
}
