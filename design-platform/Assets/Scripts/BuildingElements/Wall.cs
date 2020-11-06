using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using DesignPlatform.Utils;

namespace DesignPlatform.Core {
    public class Wall : MonoBehaviour {
        public Interface interFace { get; private set; }
        public float wallThickness = 0.2f;
        public Material wallMaterial;
        private Wall prefabWall;

        /// <summary>
        /// Construct walls.
        /// </summary>
        public void InitializeWall(Interface interFace) {
            // lav swich wall types (fra Enum - WallType)
            // Fix hjørnesamlinger

            this.interFace = interFace;

            gameObject.layer = 13; // Wall layer

            GameObject prefabWallObject = AssetUtil.LoadAsset<GameObject>("prefabs", "WallPrefab");

            prefabWall = (Wall)prefabWallObject.GetComponent(typeof(Wall));
            gameObject.name = "CLT Wall";

            List<Vector3> startEndPoints = new List<Vector3>() { interFace.GetStartPoint(),
                                                                 interFace.GetEndPoint()};
            Vector3 normal = Vector3.Cross(startEndPoints[1] - startEndPoints[0], Vector3.up).normalized;
            List<Vector3> dubStartEndPoints = startEndPoints.Select(v => new Vector3(v.x, v.y, v.z)).ToList();
            List<Vector3> wallControlPoints = new List<Vector3> {
            startEndPoints[0] + normal * wallThickness/2,
            dubStartEndPoints[0] + normal * wallThickness/2 + Vector3.up * 3f,
            dubStartEndPoints[1] + normal * wallThickness/2 + Vector3.up * 3f,
            startEndPoints[1] + normal * wallThickness/2
            };

            gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent<PolyShape>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(wallControlPoints);
            polyshape.extrude = wallThickness; 
            polyshape.CreateShapeFromPolygon();
            //
            if(interFace.GetCoincidentOpenings().Count() > 0) {
                AppendElements.CreateShapeFromPolygon(mesh, wallControlPoints, wallThickness, false, (IList<IList<Vector3>>)GetHoleVertices());
                gameObject.GetComponent<MeshRenderer>().material = prefabWall.wallMaterial;
            }

            gameObject.GetComponent<MeshRenderer>().material = prefabWall.wallMaterial;
            mesh.ToMesh();
            mesh.Refresh();

            

        }
        
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