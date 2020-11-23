using System.Collections;
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

        public float RoofPitch = 25;
        public float wallThickness = 0.2f;
        public float overhang = 0.4f;

        public void CreateRoof() {

            List<Vector2> roofPolygon = RoofUtils.GetBuildingOutline().Select(v => v.ToVector2XZ()).ToList();

            roofPolygon = RoofUtils.OffsetPolyline2D(roofPolygon, wallThickness / 2);

            Config config = new Config();
            config.roofConfig.thickness = 0.00f;
            config.roofConfig.overhang = overhang = 0.4f;
            config.roofConfig.type = RoofType.Gabled;

            // Creates base roof mesh using ProceduralToolkit
            IConstructible<MeshDraft> constructible = RoofUtils.GenerateRoofPlan(roofPolygon, config);


            Mesh roofMesh = constructible.Construct(Vector2.zero).ToMesh();
            roofMesh.RecalculateNormals();

            // Roof mesh triangle indices and vertices. 
            int[] allTriangleIndices = roofMesh.triangles;
            Vector3[] vertices = roofMesh.vertices;
            Vector3[] normals = roofMesh.normals;

            // List for each individual triangle made up by vertices (a,b,c)
            List<Vector3[]> triangleVertices = new List<Vector3[]>();
            List<Vector3[]> triangleNormals = new List<Vector3[]>();

            // Splits index-list into chunks of three (providing the three vertex-indices of each triangle) and adds triangles to list
            RoofUtils.SplitList(allTriangleIndices.ToList(), 3).ForEach(t => triangleVertices.Add(new Vector3[] { vertices[t[0]], vertices[t[1]], vertices[t[2]] }));
            RoofUtils.SplitList(allTriangleIndices.ToList(), 3).ForEach(t => triangleNormals.Add(new Vector3[] { normals[t[0]], normals[t[1]], normals[t[2]] }));

            List<int> indicesToKeep = new List<int>();

            // Removes faulty triangles, where two vertices have the same coordiates
            triangleVertices.Where(t => !(Vector3.Distance(t[0], t[1]) < 0.01 || Vector3.Distance(t[0], t[2]) < 0.01 || Vector3.Distance(t[2], t[1]) < 0.01)).ToList().ForEach(t => indicesToKeep.Add(triangleVertices.IndexOf(t)));
            triangleVertices = indicesToKeep.Select(j => triangleVertices[j]).ToList();
            triangleNormals = indicesToKeep.Select(j => triangleNormals[j]).ToList();

            // Rounds normals to nearest 1 digit
            triangleNormals = triangleNormals.Select(t => new Vector3[] { 
                new Vector3(Mathf.Round(t[0].x * 10) / 10, Mathf.Round(t[0].y * 10) / 10, Mathf.Round(t[0].z * 10) / 10),
                new Vector3(Mathf.Round(t[1].x * 10) / 10, Mathf.Round(t[1].y * 10) / 10, Mathf.Round(t[1].z * 10) / 10),
                new Vector3(Mathf.Round(t[2].x * 10) / 10, Mathf.Round(t[2].y * 10) / 10, Mathf.Round(t[2].z * 10) / 10)
            }).ToList();


            // Identifier ID (Integer) for each roof triangle referring to its roof element
            List<int> roofIDs = Enumerable.Repeat(-1, triangleVertices.Count).ToList();

            for (int j = 0; j < roofIDs.Count; j++){

                List<Vector3[]> parallelAndConnectedTriangles = new List<Vector3[]>();
                for (int k = 0; k < roofIDs.Count; k++) {
                    if (j == k) continue;
                    // Finds parallel triangles:
                    if(Vector3.Distance(triangleNormals[j][0], triangleNormals[k][0]) < 0.01) {
                        // Tests if parallel triangle is connect to current by more than one vertex (thus picking only fully attached neighbouring triangles)
                        if( RoofUtils.NumberOfSharedVertices( triangleVertices[j], triangleVertices[k])>1 )
                            parallelAndConnectedTriangles.Add(triangleVertices[k]);
                    }
                }

                // Sees if one of parallel triangles belongs to a roof face and saves its ID
                int? currentID = parallelAndConnectedTriangles.Select(i => roofIDs[triangleVertices.IndexOf(i)]).Where(wg => wg != -1)?.FirstOrDefault();

                // If one triangle already had an ID attached, all identified parallel triangles gets this ID
                if (currentID != 0) {
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
            IEnumerable<IGrouping<int, Vector3[]>> roofFaceNormalsGroups = triangleNormals.GroupBy(i => roofIDs[triangleNormals.IndexOf(i)]);

            // Converts groupings to lists
            List<Vector3> roofFaceNormals = roofFaceNormalsGroups.Select(fg => fg.SelectMany(triNorms => triNorms.ToList()).ToList()[0]).ToList();

            // List for distinct, ordered roof face vertices
            List<List<Vector3>> finalVertices = new List<List<Vector3>>();

            int index = 0;
            foreach (var face in roofFaceGroups) {
                // List of triangles in current face. ( Vector3[] is one triangle )
                List<Vector3[]> trianglesInFace = face.Select(g => g).ToList();
                // Gets outline (unordered) from group of triangles
                List<List<Vector3>> segments = RoofUtils.GetMeshOutline(trianglesInFace);
                // Find ordered polyline vertices from unordered segments 
                List<Vector3> outline = RoofUtils.SegmentsToPolyline(segments);

                // Moves vertical roof faces (gables) in towards the building, if there is overhang
                Vector3 normal = roofFaceNormals[index];
                if (Mathf.Abs(Vector3.Dot(Vector3.up, normal)) < 0.01) {
                    outline = outline.Select(v => v + -overhang * normal).ToList();
                }

                finalVertices.Add(outline);

                InitializeRoof(finalVertices.Last(), normal);
                index++;
            }
        }

        /// <summary>
        /// Construct roof.
        /// </summary>
        public void InitializeRoof(List<Vector3> roofFaceVertices, Vector3 Normal) {
            Vector3 midpoint = RoofUtils.Midpoint(roofFaceVertices);


            //Vector3 midpoint = new Vector3(roofFaceVertices[0].x, roofFaceVertices[0].y, roofFaceVertices[0].z);
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, Normal).normalized;
            float rotationAngle = Vector3.Angle(Vector3.up, Normal);
            Vector3 rotationVector = (-rotationAxis * rotationAngle);

            roofFaceVertices = roofFaceVertices.Select(v => v - midpoint).ToList();

            Debug.Log(RotatePointAroundPivot(Normal, new Vector3(0,0,0), rotationVector).ToString());

            List<Vector3> transformedPoints = new List<Vector3>();
            foreach (Vector3 v in roofFaceVertices) {
                transformedPoints.Add(RotatePointAroundPivot(v, new Vector3(0,0,0), rotationVector));
            }

            roofFaceVertices = transformedPoints;
            //roofFaceVertices = roofFaceVertices.Select(v => v - midpoint).ToList();


            GameObject gameObject = new GameObject("roof");

            gameObject.layer = 13; // Wall layer

            Material roofMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");
            gameObject.name = "Roof";

            gameObject.transform.position += new Vector3(0, 3.08f, 0);

            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            List<Vector3> wallMeshControlPoints = roofFaceVertices.Select(p => p -= Vector3.up * (wallThickness / 2)).ToList();

            mesh.CreateShapeFromPolygon(wallMeshControlPoints, -wallThickness, false);

            mesh.GetComponent<MeshRenderer>().material = roofMaterial;

        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

    }
}