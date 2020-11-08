using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditorInternal;
using System.Runtime.InteropServices;

namespace DesignPlatform.Core {
    public class Wall : MonoBehaviour {
        public Interface interFace { get; private set; }
        public float wallThickness = 0.2f;
        public Material wallMaterial;
        private Wall prefabWall;
        private List<Vector3> wallControlPoints { get; set; }

        /// <summary>
        /// Construct walls.
        /// </summary>
        public void InitializeWall(Interface interFace) {
            // lav swich wall types (fra Enum - WallType)
            // Fix hjørnesamlinger

            this.interFace = interFace;

            gameObject.layer = 13; // Wall layer

            GameObject prefabWallObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/WallPrefab.prefab");
            prefabWall = (Wall)prefabWallObject.GetComponent(typeof(Wall));
            gameObject.name = "CLT Wall";


            List<Vector3> startEndPoints = new List<Vector3>() { interFace.GetStartPoint(),
                                                             interFace.GetEndPoint()};
            Vector3 normal = Vector3.Cross(startEndPoints[1] - startEndPoints[0], Vector3.up).normalized;
            List<Vector3> dubStartEndPoints = startEndPoints.Select(v => new Vector3(v.x, v.y, v.z)).ToList();

            //----------------------------------------------------------------------------------------------------------
            List<Vector3> dubdubStartEndPoints = startEndPoints.Select(v => new Vector3(v.x, v.y, v.z)).ToList();
            List<Vector3> dubdubdubStartEndPoints = startEndPoints.Select(v => new Vector3(v.x, v.y, v.z)).ToList();
            //----------------------------------------------------------------------------------------------------------

            wallControlPoints = new List<Vector3> {
            startEndPoints[0] + normal * wallThickness/2,
            dubStartEndPoints[0] + normal * wallThickness/2 + Vector3.up * 3f,
            dubStartEndPoints[1] + normal * wallThickness/2 + Vector3.up * 3f,
            startEndPoints[1] + normal * wallThickness/2
            };

            gameObject.AddComponent<MeshCollider>();
            //gameObject.AddComponent<PolyShape>();
            //ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            //PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            //polyshape.SetControlPoints(wallControlPoints);
            //polyshape.extrude = wallThickness; 
            //polyshape.CreateShapeFromPolygon();
            
            IList<Vector3> wallHole = new List<Vector3> {
                dubdubStartEndPoints[0] + normal * wallThickness/2 + (dubdubStartEndPoints[1]-dubdubStartEndPoints[0]) *(dubdubStartEndPoints[1]-dubdubStartEndPoints[0]).magnitude/2 ,
                dubdubdubStartEndPoints[0] + normal * wallThickness/2 +(dubdubStartEndPoints[1]-dubdubStartEndPoints[0]) *(dubdubStartEndPoints[1]-dubdubStartEndPoints[0]).magnitude/2 + Vector3.up * 2f,
                dubdubdubStartEndPoints[1] + normal * wallThickness/2 +(dubdubStartEndPoints[1]-dubdubStartEndPoints[0]) *(dubdubStartEndPoints[1]-dubdubStartEndPoints[0]).magnitude/2+ Vector3.up * 2f,
                dubdubStartEndPoints[1] + normal * wallThickness/2+(dubdubStartEndPoints[1]-dubdubStartEndPoints[0]) *(dubdubStartEndPoints[1]-dubdubStartEndPoints[0]).magnitude/2
            };
            IList<IList<Vector3>> wallHoles = new List<IList<Vector3>>();
            wallHoles.Append(wallHole);
            Debug.Log(wallHoles.Count);

            //gameObject.GetComponent<MeshRenderer>().material = prefabWall.wallMaterial;

            //mesh.ToMesh();
            //mesh.Refresh();
            ProBuilderMesh mesh = ProBuilderMesh.Create();

            mesh.CreateShapeFromPolygon(wallControlPoints, wallThickness, false, wallHoles);
            //AppendElements.CreateShapeFromPolygon()
            mesh.GetComponent<MeshRenderer>().material = prefabWall.wallMaterial;
            mesh.ToMesh();
            mesh.Refresh();



        }
        //public void CreateOpeningHoles() {
        //    if (interFace.GetCoincidentOpenings().Count() > 0) {
        //        ProBuilderMesh mesh = ProBuilderMesh.Create();

        //        mesh.CreateShapeFromPolygon(wallControlPoints, wallThickness, false, (IList<IList<Vector3>>)GetHoleVertices());
        //        //AppendElements.CreateShapeFromPolygon()
        //        mesh.GetComponent<MeshRenderer>().material = prefabWall.wallMaterial;
        //        mesh.ToMesh();
        //        mesh.Refresh();
        //    }

        //}
        /// <summary>
        /// Deletes a wall and removes it from the wall list.
        /// </summary>
        public void DeleteWall() {
            if (Building.Instance.walls.Contains(this)) {
                Building.Instance.RemoveWall(this);
            }
            Destroy(gameObject);
        }
        public List<List<Vector3>> GetHoleVertices() {
            List<List<Vector3>> allHoleVertices = new List<List<Vector3>>();
            for (int i = 0; i < interFace.GetCoincidentOpenings().Count; i++) {
                allHoleVertices.Add(interFace.GetCoincidentOpenings()[i].GetControlPoints());
            }
            return allHoleVertices;
        }
    }
}