using DesignPlatform.Core;
using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StructuralAnalysis {

    /// <summary>
    /// Used to Distribute loads on the walls of the building.
    /// Currently it is only able to create and distribute vertical area loads of each building space.
    /// </summary>
    public static class LoadDistributor {

        /// <summary>
        /// Determines which direction the loads form above the room should be distributed in.
        /// </summary>
        /// <param name="space">The building space to analyze.</param>
        /// <returns>The cumulative spans in each direction.</returns>
        public static List<float> GetSpans(DesignPlatform.Core.Space space) {
            List<Vector3> controlPoints = space.GetControlPoints(localCoordinates: true, closed: true);
            List<float> spans = new List<float>() { 0.0f, 0.0f, 0.0f };
            for (int i = 0; i < controlPoints.Count - 1; i++) {
                Vector3 wallVector = controlPoints[i] - controlPoints[i + 1];
                spans[0] += Mathf.Abs(wallVector[0]);
                spans[1] += Mathf.Abs(wallVector[1]);
                spans[2] += Mathf.Abs(wallVector[2]);
            }
            return spans;
        }

        /// <summary>
        /// Distributes the vertical area loads of a building space to its boundary walls.
        /// </summary>
        /// <param name="space">The building space to analyze.</param>
        /// <returns>Table of loads on all of the walls.</returns>
        public static Dictionary<int, List<DistributedLoad>> DistributeAreaLoads(DesignPlatform.Core.Space space) {
            Dictionary<int, List<DistributedLoad>> loadTables = CreateWallLoadTables(space);
            List<Vector3> points = space.GetControlPoints(localCoordinates: true, closed: true);
            List<Vector3> normals = space.GetFaceNormals(localCoordinates: true);

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
                        DistributedLoad load1 = loadTables[indexWall1][indexLoad1];
                        if (load1.Magnitude <= 0.0f) {
                            load1.Magnitude = loadLength / 2;
                            loadTables[indexWall1][indexLoad1] = load1;
                        }

                        int indexLoad2 = IndexingUtils.WrapIndex(0, loadTables[indexWall2].Count);
                        DistributedLoad load2 = loadTables[indexWall2][indexLoad2];
                        if (load2.Magnitude <= 0.0f) {
                            load2.Magnitude = loadLength / 2;
                            loadTables[indexWall2][indexLoad2] = load2;
                        }
                    }
                }
            }
            return loadTables;
        }

        /// <summary>
        /// Creates load distribution tables for the boundary walls of a building space.
        /// </summary>
        /// <param name="space">The building space to analyze.</param>
        /// <returns>The load distribution tables for the walls of the anallyzed room.</returns>
        public static Dictionary<int, List<DistributedLoad>> CreateWallLoadTables(DesignPlatform.Core.Space space) {
            Dictionary<int, List<DistributedLoad>> WallLoads = new Dictionary<int, List<DistributedLoad>>();
            List<int> loadCarryingWalls = LoadCarryingWallsIndices(space);
            List<Vector3> points = space.GetControlPoints(localCoordinates: true, closed: true);

            List<List<float>> uniqueValuesOnWallAxes = new List<List<float>> {
                space.GetControlPoints(localCoordinates: true).Select(p => p[0]).Distinct().OrderBy(n => n).ToList(),
                space.GetControlPoints(localCoordinates: true).Select(p => p[1]).Distinct().OrderBy(n => n).ToList(),
                space.GetControlPoints(localCoordinates: true).Select(p => p[2]).Distinct().OrderBy(n => n).ToList()
            };

            foreach (int wallIndex in loadCarryingWalls) {
                Vector3 startPoint = points[wallIndex];
                Vector3 endPoint = points[wallIndex + 1];

                // Find aksen som væggen spænder
                Vector3 wallVector = startPoint - endPoint;
                int wallAxis = VectorUtils.IndexLargestComponent(wallVector);

                // Find unikke værdier på denne akse og reparameteriser disse over væggens længde
                List<float> lengthParameters = RangeUtils.Reparametrize(uniqueValuesOnWallAxes[wallAxis],
                                                                        startPoint[wallAxis],
                                                                        endPoint[wallAxis]);
                lengthParameters = lengthParameters.OrderBy(o => o).ToList();

                // Tilføj en load for denne væg mellem hver unik værdi på væggens akse
                WallLoads.Add(wallIndex, new List<DistributedLoad>());
                for (int i = 0; i < lengthParameters.Count - 1; i++) {
                    float startParam = lengthParameters[i];
                    float endParam = lengthParameters[i + 1];
                    if (startParam >= 0.0f && endParam <= 1.0f) {
                        WallLoads[wallIndex].Add(new DistributedLoad(magnitude: 0.0f, startParameter: startParam, endParameter: endParam));
                    }
                }
            }
            return WallLoads;
        }

        /// <summary>
        /// Identifies the load carrying walls of a room as the walls that will carry the room area using the shortest span.
        /// </summary>
        /// <param name="space">The building space to analyze.</param>
        /// <returns>List of indices of the load carrying walls of the building space.</returns>
        public static List<int> LoadCarryingWallsIndices(DesignPlatform.Core.Space space) {
            List<int> loadCarryingWalls = new List<int>();
            List<float> spans = GetSpans(space);

            // Go through all wall indices
            for (int i = 0; i < space.GetControlPoints().Count; i++) {
                // Distribute loads along first room axis (will do if they are equal)
                if (spans[0] <= spans[2] && i % 2 == 0) { loadCarryingWalls.Add(i); }
                // Distribute loads along last room axis
                if (spans[0] > spans[2] && i % 2 == 1) { loadCarryingWalls.Add(i); }
            }
            return loadCarryingWalls;
        }
    }
}
