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
        /// 
        /// </summary>
        /// <param name="foundationPolygon"></param>
        /// <param name="config"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
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

            List<Vector3> outline = SegmentsToPolyline(segments);

            return outline;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="name"></param>
        public static void VisualizePointsAsSpheres(List<Vector3> vertices,string name = "vertex")
        {
            foreach (Vector3 v in vertices){
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = v;
                sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                sphere.name = name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="midpoint"></param>
        /// <param name="rotationVector"></param>
        /// <returns></returns>
        public static List<Vector3> TransformPointsToXZ(List<Vector3> vertices, out Vector3 midpoint, out Vector3 rotationVector) {
            // Normal is calculated assuming all vertices is in the same plane
            Vector3 Normal = Vector3.Cross(vertices[2] - vertices[1], vertices[0] - vertices[1]).normalized;

            // Calculates midpoint from vertices
            Vector3 midpointTemp = RoofUtils.Midpoint(vertices);

            // Finds rotation vector that rotates points from current global position to xz-plane
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, Normal).normalized;
            float rotationAngle = Vector3.Angle(Vector3.up, Normal);
            rotationVector = (-rotationAxis * rotationAngle);

            // Moves vertices to be located around (0,0,0)
            vertices = vertices.Select(v => v - midpointTemp).ToList();

            // Rotates all points around (0,0,0) according to rotation vector
            List<Vector3> transformedPoints = new List<Vector3>();
            foreach (Vector3 v in vertices) {
                transformedPoints.Add(RoofUtils.RotatePointAroundPivot(v, new Vector3(0, 0, 0), rotationVector));
            }
            midpoint = midpointTemp;

            return transformedPoints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trianglesInface"></param>
        /// <returns></returns>
        public static List<List<Vector3>> GetMeshOutline(List<Vector3[]> trianglesInface) {
            List<List<Vector3>> allSegments = new List<List<Vector3>>();
            foreach (Vector3[] tri in trianglesInface) {
                allSegments.Add(new List<Vector3> { tri[0], tri[1] });
                allSegments.Add(new List<Vector3> { tri[1], tri[2] });
                allSegments.Add(new List<Vector3> { tri[2], tri[0] });
            }

            // Removes segments that share midpoints (internal segments), leaving only outline segments
            allSegments = allSegments.Where(s1 => allSegments.Count(s2 => Vector3.Distance(Midpoint(s1), Midpoint(s2)) < 0.005) == 1).ToList();

            return allSegments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineSegments"></param>
        /// <returns></returns>
        public static List<Vector3> SegmentsToPolyline(List<List<Vector3>> lineSegments) {
            List<Vector3> outlineVertices = new List<Vector3>();
            outlineVertices.Add(lineSegments[0][0]);
            outlineVertices.Add(lineSegments[0][1]);

            for (int i = 1; i < lineSegments.Count - 1; i++) {

                Vector3 currentPoint = outlineVertices.Last();
                //Debug.Log("Current point: " + currentPoint.ToString());
                //lineSegments.Where(ps => ps.Any(p => Vector3.Distance(p, currentPoint) < 0.1f)).ToList().ForEach(ps => Debug.Log(ps[0].ToString() + " ; " + ps[1].ToString()));

                // Next segment identified as sharing a point with the current point.
                List<Vector3> segment = lineSegments.Where(ps => ps.Any(p => Vector3.Distance(p, currentPoint) < 0.1f)).Where(ps => !ps.Any(p => Vector3.Distance(p, outlineVertices[i - 1]) < 0.1f)).First();

                // Point of current segment that is not the current point is added to polyline vertices
                if (Vector3.Distance(segment[0], currentPoint) < 0.01) outlineVertices.Add(segment[1]);
                if (Vector3.Distance(segment[1], currentPoint) < 0.01) outlineVertices.Add(segment[0]);
            }
            return outlineVertices;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triangle1"></param>
        /// <param name="triangle2"></param>
        /// <returns></returns>
        public static int NumberOfSharedVertices(Vector3[] triangle1, Vector3[] triangle2) {
            return triangle1.ToList().SelectMany(v1 => triangle2.Where(v2 => Vector3.Distance(v1, v2) < 0.01)).Count();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Vector3 Midpoint(List<Vector3> vertices){
            Vector3 midpoint = new Vector3();
            vertices.ForEach(v => midpoint += v);
            midpoint /= vertices.Count();

            return midpoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="me"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<List<T>> SplitList<T>(this List<T> me, int size = 50){
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Mathf.Min(size, me.Count - i)));
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static List<Vector2> OffsetPolyline2D(List<Vector2> polyline, float offset) {
            List<Vector2> extrudedPolygon = new List<Vector2>();
            extrudedPolygon.Add(polyline[0]);
            for (int i = 1; i < polyline.Count; i++) {
                extrudedPolygon.Add(polyline[i]);
                Vector2 dir = (polyline[i] - polyline[i - 1]).normalized;
                Vector2 normal = new Vector2(-dir.y, dir.x);
                extrudedPolygon[i - 1] += normal * offset;
                extrudedPolygon[i] += normal * offset;
            }
            Vector2 last_dir = (polyline.First() - polyline.Last()).normalized;
            Vector2 last_normal = new Vector2(-last_dir.y, last_dir.x);
            extrudedPolygon[0] += last_normal * offset;
            extrudedPolygon[extrudedPolygon.Count - 1] += last_normal * offset;

            return extrudedPolygon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="pivot"></param>
        /// <param name="angles"></param>
        /// <returns></returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }
    }

}