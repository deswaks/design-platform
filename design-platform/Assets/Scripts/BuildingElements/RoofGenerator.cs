﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit.Buildings;
using ProceduralToolkit;
using static ProceduralToolkit.Buildings.BuildingGenerator;
using DesignPlatform.Utils;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {
    public class RoofGenerator{

        private static RoofGenerator instance;
        public static RoofGenerator Instance{
            get { return instance ?? (instance = new RoofGenerator()); }
        }

        public float RoofPitch = 15;
        public float wallThickness = 0.2f;
        public float overhang = 0.15f;
        public RoofType roofType = RoofType.Gabled;

        public List<List<Vector3>> CreateRoofOutlines() {

            List<Vector2> roofPolygon = RoofUtils.GetBuildingOutline().Select(v => v.ToVector2XZ()).ToList();

            if (roofPolygon.Count == 0) return new List<List<Vector3>>();

            roofPolygon = RoofUtils.OffsetPolyline2D(roofPolygon, wallThickness / 2);

            Config config = new Config();
            config.roofConfig.thickness = 0.00f;
            config.roofConfig.overhang = overhang;
            config.roofConfig.type = roofType;

            // Creates base roof mesh using ProceduralToolkit
            IConstructible<MeshDraft> constructible = RoofUtils.GenerateRoofPlan(roofPolygon, config);

            Mesh roofMesh;
            //Mesh roofMesh = constructible.Construct(Vector2.zero).ToMesh();
            if(roofType == RoofType.Gabled) {
                roofMesh = ((ProceduralGabledRoof)constructible).ConstructWithPitch(Vector2.zero, RoofPitch).ToMesh();
            }
            else if(roofType == RoofType.Hipped) {
                roofMesh = ((ProceduralHippedRoof)constructible).ConstructWithPitch(Vector2.zero, RoofPitch).ToMesh();
            }
            else {
                roofMesh = constructible.Construct(Vector2.zero).ToMesh();
            }


            roofMesh.RecalculateNormals();

            // Roof mesh triangle indices and vertices. 
            int[] allTriangleIndices = roofMesh.triangles;
            Vector3[] vertices = roofMesh.vertices;

            // List for each individual triangle made up by vertices (a,b,c)
            List<Vector3[]> triangleVertices = new List<Vector3[]>();

            // Splits index-list into chunks of three (providing the three vertex-indices of each triangle) and adds triangles to list
            RoofUtils.SplitList(allTriangleIndices.ToList(), 3).ForEach(t => triangleVertices.Add(new Vector3[] { vertices[t[0]], vertices[t[1]], vertices[t[2]] }));

            // Removes faulty triangles, where two vertices have the same coordinates
            triangleVertices = triangleVertices.Where(t => !(Vector3.Distance(t[0], t[1]) < 0.01 || Vector3.Distance(t[0], t[2]) < 0.01 || Vector3.Distance(t[2], t[1]) < 0.01)).ToList();

            // Calculates normals of each triangle
            List<Vector3> triangleNormals = triangleVertices.Select(t => Vector3.Cross(t[2] - t[1], t[0] - t[1]).normalized).ToList();

            if (roofType != RoofType.Flat) {
                // Removes horizontal faces (horizontal triangles are created when choosing an overhang length >0 )
                triangleVertices = triangleVertices.Where((vs, i) => triangleNormals[i].y >= -0.1f).ToList();
                triangleNormals = triangleNormals.Where(n => n.y >= -0.1f).ToList();
            }

            // Identifier ID (Integer) for each roof triangle referring to its roof element
            List<int> roofIDs = Enumerable.Repeat(-1, triangleVertices.Count).ToList();

            for (int j = 0; j < roofIDs.Count; j++){

                List<Vector3[]> parallelAndConnectedTriangles = new List<Vector3[]>();
                for (int k = 0; k < roofIDs.Count; k++){
                    if (j == k) continue;
                    // Finds parallel triangles:
                    if (Vector3.Distance(triangleNormals[j], triangleNormals[k]) < 0.01){
                        // Tests if parallel triangle is connect to current by more than one vertex (thus picking only fully attached neighbouring triangles)
                        if (RoofUtils.NumberOfSharedVertices(triangleVertices[j], triangleVertices[k]) > 1){
                            parallelAndConnectedTriangles.Add(triangleVertices[k]);
                        }
                    }
                }

                // Sees if one of parallel triangles belongs to a roof face and saves its ID
                int? currentID = parallelAndConnectedTriangles.Select(i => roofIDs[triangleVertices.IndexOf(i)]).Where(wg => wg != -1)?.FirstOrDefault();

                // If one triangle already had an ID attached, all identified parallel triangles gets this ID
                if (currentID != 0) {
                    // The IDs of all found connected triangles changed to the current ID (thus also targeting triangles not directly adjacent to current one)
                    List<int> connectedIDs = parallelAndConnectedTriangles.Select(t => roofIDs[ triangleVertices.IndexOf(t) ]).Where(wg => wg != -1).ToList();
                    roofIDs = roofIDs.Select(id => id = connectedIDs.Contains(id) ? currentID.Value : id).ToList();

                    parallelAndConnectedTriangles.ForEach(i => roofIDs[triangleVertices.IndexOf(i)] = currentID.Value);
                    roofIDs[j] = currentID.Value;
                }
                else // Or, a new ID is created for the set of parallel triangles
                {
                    currentID = roofIDs.Max() + 1;
                    parallelAndConnectedTriangles.ForEach(i => roofIDs[triangleVertices.IndexOf(i)] = currentID.Value);
                    roofIDs[j] = currentID.Value;
                }
            }

            // Groups vertices and normals by roofIDs
            IEnumerable<IGrouping<int, Vector3[]>> roofFaceGroups = triangleVertices.GroupBy(i => roofIDs[triangleVertices.IndexOf(i)]);

            // List for distinct, ordered roof face vertices
            List<List<Vector3>> finalPanelOutlines = new List<List<Vector3>>();

            foreach (var face in roofFaceGroups) {
                // List of triangles in current face. ( Vector3[] is one triangle )
                List<Vector3[]> trianglesInFace = face.Select(g => g).ToList();
                // Gets outline (unordered) from group of triangles
                List<List<Vector3>> segments = RoofUtils.GetMeshOutline(trianglesInFace);
                // Find ordered polyline vertices from unordered segments 
                List<Vector3> outline = RoofUtils.SegmentsToPolyline(segments);

                finalPanelOutlines.Add(outline);
            }

            return finalPanelOutlines;
        }
    }
}