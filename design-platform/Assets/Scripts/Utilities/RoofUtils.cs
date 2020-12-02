using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit.Buildings;
using ProceduralToolkit;
using System.Linq;
using System;
using DesignPlatform.Core;
using DesignPlatform.Geometry;

namespace DesignPlatform.Utils {

    /// <summary>
    /// Helper functions for the creation of roofs
    /// </summary>
    public static class RoofUtils {

        /// <summary>
        /// Creates a roof plan of the given type
        /// </summary>
        /// <param name="foundationPolygon">Polygon that defines the outer perimeter of the building.</param>
        /// <param name="config">Roof plan generator config options.</param>
        /// <returns>The generated roof plan.</returns>
        public static IConstructible<MeshDraft> GenerateRoofPlan(List<Vector2> foundationPolygon, BuildingGenerator.Config config) {
            switch (config.roofConfig.type) {
                case RoofType.Flat:
                    return new ProceduralFlatRoof(foundationPolygon, config.roofConfig, config.palette.roofColor);
                case RoofType.Hipped:
                    return new ProceduralHippedRoof(foundationPolygon, config.roofConfig, config.palette.roofColor);
                case RoofType.Gabled:
                    return new ProceduralGabledRoof(foundationPolygon, config.roofConfig, config.palette.roofColor);
                default:
                    return new ProceduralGabledRoof(foundationPolygon, config.roofConfig, config.palette.roofColor);
            }
        }

        /// <summary>
        /// Finds the polygon that defines the outer perimeter of the building.
        /// </summary>
        /// <returns>The building outline.</returns>
        public static List<Vector3> GetBuildingOutline(){
            // Grabs external interfaces (1 attached face) and culls out interfaces with 0 length
            List<Interface> culledInterfaces = Building.Instance.InterfacesVertical.Where(i => i.EndPoint != i.StartPoint).Where(iface => iface.Faces.Count == 1).ToList();

            List<List<Vector3>> segments = new List<List<Vector3>>();

            // Each group consists of interfaces making up an entire wall
            List<List<Interface>> groupedInterfaces = Building.GroupAdjoiningInterfaces(culledInterfaces);

            // Loops through list of wall interfaces belong to each wall to find out wall end points and wall midpoints.
            foreach (List<Interface> wallInterfaces in groupedInterfaces) {
                List<Vector3> currentInterfaceVertices = wallInterfaces.SelectMany(i => new List<Vector3> { i.EndPoint, i.StartPoint }).Distinct().ToList();
                List<float> xValues = currentInterfaceVertices.Select(p => p.x).ToList();
                List<float> zValues = currentInterfaceVertices.Select(p => p.z).ToList();
                // Endpoints are (Xmin,0,Zmin);(Xmax,0,Zmax) ////////////////// ONLY TRUE FOR WALLS LYING ALONG X-/Z-AXIS
                segments.Add(new List<Vector3>{ 
                    new Vector3(xValues.Min(), 0, zValues.Min()),
                    new Vector3(xValues.Max(), 0, zValues.Max())
                });
            }

            List<Vector3> outline = JoinLineSegments(segments);

            return outline;
        }

        /// <summary>
        /// Finds the naked edges (Edges that only belongs to a single mesh face).
        /// </summary>
        /// <param name="MeshFaces">A list of mesh faces, each represented as an array of 3 vector3 vertices.</param>
        /// <returns>The naked edges of the mesh.</returns>
        public static List<List<Vector3>> GetNakedEdgesOfMesh(List<Vector3[]> MeshFaces) {

            List<List<Vector3>> allSegments = new List<List<Vector3>>();
            foreach (Vector3[] tri in MeshFaces) {
                allSegments.Add(new List<Vector3> { tri[0], tri[1] });
                allSegments.Add(new List<Vector3> { tri[1], tri[2] });
                allSegments.Add(new List<Vector3> { tri[2], tri[0] });
            }

            // Removes segments that share midpoints (internal segments), leaving only outline segments
            allSegments = allSegments.Where(s1 => allSegments.Count(s2 =>
                Vector3.Distance(Midpoint(s1), Midpoint(s2)) < 0.005) == 1).ToList();

            return allSegments;
        }

        /// <summary>
        /// Joins all the given line segments to a single polyline.
        /// </summary>
        /// <param name="lineSegments">A collection of line segments that must all be connected.</param>
        /// <returns>The vertices of the single polyline.</returns>
        public static List<Vector3> JoinLineSegments(List<List<Vector3>> lineSegments) {
            List<Vector3> polyline = new List<Vector3>();
            polyline.Add(lineSegments[0][0]);
            polyline.Add(lineSegments[0][1]);

            for (int i = 1; i < lineSegments.Count - 1; i++) {

                Vector3 currentPoint = polyline.Last();

                // Next segment identified as sharing a point with the current point.
                List<Vector3> segment = lineSegments.Where(ps => ps.Any(p => Vector3.Distance(p, currentPoint) < 0.1f)).Where(ps => !ps.Any(p => Vector3.Distance(p, polyline[i - 1]) < 0.1f)).First();

                // Point of current segment that is not the current point is added to polyline vertices
                if (Vector3.Distance(segment[0], currentPoint) < 0.01) polyline.Add(segment[1]);
                if (Vector3.Distance(segment[1], currentPoint) < 0.01) polyline.Add(segment[0]);
            }
            return polyline;
        }

        /// <summary>
        /// The midpoint expressed as the average of all the given vertices.
        /// </summary>
        /// <param name="vertices">Vertices to average</param>
        /// <returns>The average point of all the vertices.</returns>
        public static Vector3 Midpoint(List<Vector3> vertices){
            Vector3 midpoint = new Vector3();
            vertices.ForEach(v => midpoint += v);
            midpoint /= vertices.Count();

            return midpoint;
        }

        /// <summary>
        /// Splits the list into sublists of the given size.
        /// </summary>
        /// <typeparam name="T">Type of objects in the list</typeparam>
        /// <param name="listOfItems">List to split.</param>
        /// <param name="size">Size of each sublist.</param>
        /// <returns></returns>
        public static List<List<T>> SplitList<T>(this List<T> listOfItems, int size = 50){
            var list = new List<List<T>>();
            for (int i = 0; i < listOfItems.Count; i += size)
                list.Add(listOfItems.GetRange(i, Mathf.Min(size, listOfItems.Count - i)));
            return list;
        }

        /// <summary>
        /// Rotate a point around a pivot point.
        /// </summary>
        /// <param name="point">Point to rotate.</param>
        /// <param name="pivot">Pivot to rotate around.</param>
        /// <param name="angles">Angle to rotate in degrees.</param>
        /// <returns></returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }
    }

}