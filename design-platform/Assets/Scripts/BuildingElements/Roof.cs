using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit.Buildings;
using ProceduralToolkit;
using static ProceduralToolkit.Buildings.BuildingGenerator;
using ProceduralToolkit.Skeleton;
using DesignPlatform.Utils;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {
    
    public class Roof : MonoBehaviour {

        public ProceduralToolkit.RendererProperties rendererProperties;
        public Material roofMaterial;
        public float RoofPitch = 25;

        void Awake() {
            List<Vector2> foundationPolygon = new List<Vector2> {
                new Vector2(0, 0),
                new Vector2(-2, 0),
                new Vector2(-2, 3),
                new Vector2(5, 3),
                new Vector2(5, 0),
                new Vector2(3, 0),
                new Vector2(3, -3),
                new Vector2(0, -3),
            };

            Config config = new Config();
            config.roofConfig.thickness = 0.01f;
            config.roofConfig.overhang = 0.0f;
            config.roofConfig.type = RoofType.Gabled;

            // ROOF CREATION ////////////////////////////
            ProceduralRoofPlanner roofPlanner = new ProceduralRoofPlanner();

            IConstructible<MeshDraft> constructible = roofPlanner.Plan(foundationPolygon, config);
            var draft = constructible.Construct(Vector2.zero);

            //MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            //meshFilter.mesh = draft.ToMesh();

            //MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            //meshRenderer.material = roofMaterial;
            //////////////////////////////////////////////



            // COMBINING TRIANGLES TO FACES /////////////////////////////////////
            Mesh roofMesh = draft.ToMesh();
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
                        // Tests if parallel triangle is connect to current by more than one vertex
                        if( RoofUtils.NumberOfSharedVertices( triangleVertices[j], triangleVertices[k])>1 )
                            parallelAndConnectedTriangles.Add(triangleVertices[k]);
                    }
                }

                // Sees if one of parallel interfaces belongs to a wall and saves its ID
                int? currentID = parallelAndConnectedTriangles.Select(i => roofIDs[triangleVertices.IndexOf(i)]).Where(wg => wg != -1)?.FirstOrDefault();

                // If one interface already had an ID attached, all identified parallel walls gets this ID
                if (currentID != 0) {
                    parallelAndConnectedTriangles.ForEach(i => roofIDs[triangleVertices.IndexOf(i)] = currentID.Value);
                    roofIDs[j] = currentID.Value;
                }
                else // A new ID is created for the set of parallel walls
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
            List<List<Vector3>> roofFaceVertices = roofFaceGroups.Select(fg => fg.SelectMany(g => g.ToList()).ToList()).ToList();
            List<Vector3> roofFaceNormals = roofFaceNormalsGroups.Select(fg => fg.SelectMany(triNorms => triNorms.ToList()).ToList()[0]).ToList();

            // List for distinct, ordered roof face vertices
            List<List<Vector3>> finalVertices = new List<List<Vector3>>();

            int index = 0;
            foreach (List<Vector3> vs in roofFaceVertices)
            {
                finalVertices.Add(RoofUtils.OrderClockwise(RoofUtils.DistinctVertices(vs),roofFaceNormals[index]) );

                InitializeRoof(finalVertices.Last(), roofFaceNormals[index]);

                index++;
            }



            ////////////////////////////////////
            // Visualization of vertices
            foreach (List<Vector3> vs in finalVertices)
            {

                foreach (Vector3 v in vs)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = v;
                    sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    sphere.name = finalVertices.IndexOf(vs).ToString();

                }
            }
        }

        /// <summary>
        /// Construct walls.
        /// </summary>
        public void InitializeRoof(List<Vector3> controlPoints, Vector3 Normal)
        {
            GameObject game = new GameObject("roof");

            float wallThickness = 0.2f;
            game.layer = 13; // Wall layer

            Material wallMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");
            game.name = "Roof";

            //game.transform.position = RoofUtils.Midpoint(controlPoints);
            //game.transform.rotation = Quaternion.LookRotation(Vector3.up, Normal);

            game.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = game.AddComponent<ProBuilderMesh>();

            List<Vector3> wallMeshControlPoints = controlPoints.Select(p => p -= Vector3.up * (wallThickness / 2)).ToList();

            mesh.CreateShapeFromPolygon(wallMeshControlPoints, wallThickness, false);

            mesh.GetComponent<MeshRenderer>().material = wallMaterial;
        }
    }
}