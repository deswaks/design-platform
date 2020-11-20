using DesignPlatform.Core;
using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StructuralAnalysis {

    public static class LoadDistribution {

        /// <summary>
        /// Used to find out which direction the loads form above the room should be distributed in
        /// </summary>
        /// <param name="room">The room to analyze</param>
        /// <returns>The cumulative spans in each direction</returns>
        public static List<float> Spans(Room room) {
            List<Vector3> controlPoints = room.GetControlPoints(localCoordinates: true, closed: true);
            List<float> spans = new List<float>() { 0.0f, 0.0f, 0.0f };
            for (int i = 0; i < controlPoints.Count - 1; i++) {
                Vector3 wallVector = controlPoints[i] - controlPoints[i + 1];
                spans[0] += Mathf.Abs(wallVector[0]);
                spans[1] += Mathf.Abs(wallVector[1]);
                spans[2] += Mathf.Abs(wallVector[2]);
            }
            return spans;
        }


        public static Dictionary<int, List<Load>> AreaLoad(Room room) {
            Dictionary<int, List<Load>> loadTables = WallLoadTables(room);
            List<Vector3> points = room.GetControlPoints(localCoordinates: true, closed: true);
            List<Vector3> normals = room.GetWallNormals(localCoordinates: true);

            for (int iWall = 0; iWall < normals.Count; iWall++) {
                if (loadTables.Keys.Contains(iWall)) {
                    // Dette er en load bearing wall
                }
                else {
                    // Dette er en load giving wall
                    // Hvis naboer har modsat-rettede normaler
                    int indexWall1 = IndexingUtils.WrapIndex(iWall - 1, normals.Count);
                    int indexWall2 = IndexingUtils.WrapIndex(iWall + 1, normals.Count);

                    if (normals[indexWall1] != normals[indexWall2]) {
                        float loadLength = Vector3.Distance(points[iWall], points[iWall + 1]);

                        int indexLoad1 = IndexingUtils.WrapIndex(-1, loadTables[indexWall1].Count);
                        Load load1 = loadTables[indexWall1][indexLoad1];
                        if (load1.magnitude <= 0.0f) {
                            load1.magnitude = loadLength / 2;
                            loadTables[indexWall1][indexLoad1] = load1;
                        }

                        int indexLoad2 = IndexingUtils.WrapIndex(0, loadTables[indexWall2].Count);
                        Load load2 = loadTables[indexWall2][indexLoad2];
                        if (load2.magnitude <= 0.0f) {
                            load2.magnitude = loadLength / 2;
                            loadTables[indexWall2][indexLoad2] = load2;
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
            List<List<float>> uniqueValuesOnWallAxes = room.UniqueCoordinates(localCoordinates: true);

            foreach (int wallIndex in loadCarryingWalls) {
                Vector3 startPoint = points[wallIndex];
                Vector3 endPoint = points[wallIndex + 1];

                // Find aksen som væggen spænder
                Vector3 wallVector = startPoint - endPoint;
                int wallAxis = VectorFunctions.IndexAbsLargestComponent(wallVector);

                // Find unikke værdier på denne akse og reparameteriser disse over væggens længde
                List<float> lengthParameters = RangeUtils.Reparametrize(uniqueValuesOnWallAxes[wallAxis],
                                                                        startPoint[wallAxis],
                                                                        endPoint[wallAxis]);
                lengthParameters = lengthParameters.OrderBy(o => o).ToList();

                // Tilføj en load for denne væg mellem hver unik værdi på væggens akse
                WallLoads.Add(wallIndex, new List<Load>());
                for (int i = 0; i < lengthParameters.Count - 1; i++) {
                    float startParam = lengthParameters[i];
                    float endParam = lengthParameters[i + 1];
                    if (startParam >= 0.0f && endParam <= 1.0f) {
                        WallLoads[wallIndex].Add(new Load(mag: 0.0f, start: startParam, end: endParam));
                    }
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
                // Distribute loads along first room axis (will do if they are equal)
                if (spans[0] <= spans[2] && i % 2 == 0) { loadCarryingWalls.Add(i); }
                // Distribute loads along last room axis
                if (spans[0] > spans[2] && i % 2 == 1) { loadCarryingWalls.Add(i); }
            }
            return loadCarryingWalls;
        }
    }
}
