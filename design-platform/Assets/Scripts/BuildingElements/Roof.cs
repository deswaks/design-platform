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

        void Start() {
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

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = draft.ToMesh();

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
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
            List<(Vector3 a, Vector3 b, Vector3 c)> triangleVertices = new List<(Vector3 a, Vector3 b, Vector3 c)>();
            List<(Vector3 a, Vector3 b, Vector3 c)> triangleNormals = new List<(Vector3 a, Vector3 b, Vector3 c)>();


            // Splits index-list into chunks of three (providing the three vertex-indices of each triangle) and adds triangles to list
            RoofUtils.SplitList(allTriangleIndices.ToList(), 3).ForEach(t => triangleVertices.Add((vertices[t[0]], vertices[t[1]], vertices[t[2]])));
            RoofUtils.SplitList(allTriangleIndices.ToList(), 3).ForEach(t => triangleNormals.Add((normals[t[0]], normals[t[1]], normals[t[2]])));


            List<int> indicesToKeep = new List<int>();

            // Removes faulty triangles, where two vertices have the same coordiates
            triangleVertices.Where(t => !(Vector3.Distance(t.a, t.b) < 0.01 || Vector3.Distance(t.a, t.c) < 0.01 || Vector3.Distance(t.c, t.b) < 0.01)).ToList().ForEach(t=> indicesToKeep.Add(triangleVertices.IndexOf(t)));
            triangleVertices = indicesToKeep.Select(j => triangleVertices[j]).ToList();
            triangleNormals = indicesToKeep.Select(j => triangleNormals[j]).ToList();

            triangleNormals = triangleNormals.Select(t =>
            (
                new Vector3(Mathf.Round(t.a.x * 10) / 10, Mathf.Round(t.a.y * 10) / 10, Mathf.Round(t.a.z * 10) / 10),
                new Vector3(Mathf.Round(t.b.x * 10) / 10, Mathf.Round(t.b.y * 10) / 10, Mathf.Round(t.b.z * 10) / 10),
                new Vector3(Mathf.Round(t.c.x * 10) / 10, Mathf.Round(t.c.y * 10) / 10, Mathf.Round(t.c.z * 10) / 10)
            )
            ).ToList();

            Debug.Log("True for all? "+ triangleNormals.Where(t=> ( Mathf.Abs(Vector3.Angle(t.a, t.b)) < 0.01 && Mathf.Abs(Vector3.Angle(t.a, t.c)) < 0.01 && Mathf.Abs(Vector3.Angle(t.b, t.c)) < 0.01)).Count().ToString());
            (Vector3 aaa, Vector3 bbb, Vector3 ccc) = triangleNormals
                .Where(t => !(Mathf.Abs(Vector3.Angle(t.a, t.b)) < 0.01 && Mathf.Abs(Vector3.Angle(t.a, t.c)) < 0.01 && Mathf.Abs(Vector3.Angle(t.c, t.b)) < 0.01))
                .ToList()[0];
            Debug.Log("aaa: " + aaa.ToString());
            Debug.Log("bbb: " + bbb.ToString());
            Debug.Log("ccc: " + ccc.ToString());
            Debug.Log("Angle? " + triangleNormals
                .Where(t => !(Vector3.Angle(t.a, t.b) < 0.01 && Vector3.Angle(t.a, t.c) < 0.01 && Vector3.Angle(t.c, t.b) < 0.01))
                .Select(t=> (Vector3.Angle(t.a, t.b), Vector3.Angle(t.a, t.c), Vector3.Angle(t.c, t.b)))
                .ToList()[0]
                .ToString()
                );
            //Debug.Log("Dist? " + triangleNormals
            //    .Where(t => !(Vector3.Distance(t.a, t.b) < 0.1 && Vector3.Distance(t.a, t.c) < 0.1 && Vector3.Distance(t.c, t.b) < 0.1))
            //    .Select(t => (Vector3.Distance(t.a, t.b), Vector3.Distance(t.a, t.c), Vector3.Distance(t.c, t.b)))
            //    .ToList()[0]
            //    .ToString()
            //);

            Debug.Log("Vertices amount: " + triangleVertices.Count);
            Debug.Log("Normals amount: " + triangleNormals.Count);
            Debug.Log("Indices amount: " + indicesToKeep.Count);



            Debug.Log(triangleNormals.Count + "   " + triangleVertices.Count);

            for(int i=0; i < triangleVertices.Count() ; i++) {

                Debug.Log("("+triangleVertices[i].a.ToString() + ";" + triangleVertices[i].b.ToString() + ";" + triangleVertices[i].c.ToString() + ") N: (" +
                    triangleNormals[i].a.ToString() + ";" + triangleNormals[i].b.ToString() + ";" + triangleNormals[i].c.ToString());

                List<Vector3> vx = new List<Vector3> { triangleVertices[i].a, triangleVertices[i].b, triangleVertices[i].c };
                vx.ForEach(v => {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.name = "gable " + i.ToString();
                    sphere.transform.position = v;
                    sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                });
            }






        }

    }
}