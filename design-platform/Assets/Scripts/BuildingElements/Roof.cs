using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit.Buildings;
using ProceduralToolkit;
using static ProceduralToolkit.Buildings.BuildingGenerator;
using ProceduralToolkit.Skeleton;

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

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = draft.ToMesh();

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = roofMaterial;
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

            List<int> indices = Enumerable.Range(0, triangleVertices.Count).ToList();

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

            string ids2 = string.Join(" ; ", roofIDs.Distinct().Select(n => n.ToString()+", ant. :"+ roofIDs.Count(id => id == n).ToString() ));
            string ids = string.Join(" ; ", roofIDs.OrderBy(n=>n).Select(n => n.ToString()) );

            Debug.Log(ids);

            //IEnumerable<IGrouping<int, Vector3[]>> roofFaceGroups = triangleVertices.GroupBy(i => roofIDs[triangleVertices.IndexOf(i)]);

            //RoofUtils.IdentifyBoundaryVertices( roofFaceGroups.  )
        }
        }
}