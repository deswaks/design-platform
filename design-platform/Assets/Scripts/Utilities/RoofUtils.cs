using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit.Buildings;
using ProceduralToolkit;
using System.Linq;
using System;

namespace DesignPlatform.Core {
    public static class RoofUtils {
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

        public static List<Vector3> GetBuildingOutline(){
            // Grabs external interfaces (1 attached face) and culls out interfaces with 0 length
            List<Interface> culledInterfaces = Building.Instance.InterfacesVertical.Where(i => i.EndPoint != i.StartPoint).Where(iface => iface.Faces.Count == 1).ToList();

            List<List<Vector3>> segments = new List<List<Vector3>>();
            List<int> interfaceGroupIDs = CLTElementGenerator.GroupParallelJoinedInterfaces(culledInterfaces);

            // Each group consists of interfaces making up an entire wall
            IEnumerable<IGrouping<int, Interface>> interfaceGroups = culledInterfaces.GroupBy(i => interfaceGroupIDs[culledInterfaces.IndexOf(i)]);

            // Loops through list of wall interfaces belong to each wall to find out wall end points and wall midpoints.
            foreach (List<Interface> wallInterfaces in interfaceGroups.Select(list => list.ToList()).ToList()) {
                List<Vector3> currentInterfaceVertices = wallInterfaces.SelectMany(i => new List<Vector3> { i.EndPoint, i.StartPoint }).Distinct().ToList();
                // X-values
                List<float> xs = currentInterfaceVertices.Select(p => p.x).ToList();
                // Z-values
                List<float> zs = currentInterfaceVertices.Select(p => p.z).ToList();
                // Endpoints are (Xmin,0,Zmin);(Xmax,0,Zmax) ////////////////// ONLY TRUE FOR WALLS LYING ALONG X-/Z-AXIS
                segments.Add(new List<Vector3>{ 
                    new Vector3(xs.Min(), 0, zs.Min()),
                    new Vector3(xs.Max(), 0, zs.Max())
                });
            }

            List<Vector3> outline = SegmentsToPolyline(segments);

            return outline;
        }

        public static void VisualizePointsAsSpheres(List<Vector3> vertices,string name = "vertex")
        {
            foreach (Vector3 v in vertices)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = v;
                sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                sphere.name = name;
            }
        }

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

        public static int NumberOfSharedVertices(Vector3[] triangle1, Vector3[] triangle2) {
            return triangle1.ToList().SelectMany(v1 => triangle2.Where(v2 => Vector3.Distance(v1, v2) < 0.01)).Count();
        }

        public static Vector3 Midpoint(List<Vector3> vertices){
            Vector3 midpoint = new Vector3();
            vertices.ForEach(v => midpoint += v);
            midpoint /= vertices.Count();

            return midpoint;
        }

        public static List<List<T>> SplitList<T>(this List<T> me, int size = 50){
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Mathf.Min(size, me.Count - i)));
            return list;
        }

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
    }

}