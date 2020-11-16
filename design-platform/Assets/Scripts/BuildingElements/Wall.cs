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
        private List<Vector3> wallControlPoints { get; set; }

        /// <summary>
        /// Construct walls.
        /// </summary>
        public void InitializeWall(Interface interFace) {
            this.interFace = interFace;
            //if (interFace.GetStartPoint() != interFace.GetEndPoint()) {
                gameObject.layer = 13; // Wall layer

                GameObject prefabWallObject = AssetUtil.LoadAsset<GameObject>("prefabs", "WallPrefab");

                prefabWall = (Wall)prefabWallObject.GetComponent(typeof(Wall));
                gameObject.name = "CLT Wall";

                Vector3 wallCenter = interFace.GetCenterPoint();
                Vector3 wallNormal = interFace.attachedFaces[0].parentRoom.GetWallNormals()[interFace.attachedFaces[0].faceIndex];
                float wallLength = (interFace.GetStartPoint() - interFace.GetEndPoint()).magnitude;
                float wallHeight = interFace.attachedFaces[0].parentRoom.height;

                wallControlPoints = new List<Vector3> {
                    new Vector3 (wallLength/2,0,0),
                    new Vector3 (wallLength/2,0,wallHeight),
                    new Vector3 (-wallLength/2,0,wallHeight),
                    new Vector3 (-wallLength/2,0,0) };

                gameObject.transform.position = wallCenter;
                gameObject.transform.rotation = Quaternion.LookRotation(Vector3.up, wallNormal);

                gameObject.AddComponent<MeshCollider>();
                ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

                List<Vector3> wallMeshControlPoints = wallControlPoints.Select(p => p -= Vector3.up * (wallThickness / 2)).ToList();

                if (interFace.GetCoincidentOpenings().Count > 0) {
                    mesh.CreateShapeFromPolygon(wallMeshControlPoints, wallThickness, false, GetHoleVertices());
                }

                if (interFace.GetCoincidentOpenings().Count < 1) {
                    mesh.CreateShapeFromPolygon(wallMeshControlPoints, wallThickness, false);
                }
                mesh.GetComponent<MeshRenderer>().material = prefabWall.wallMaterial;
            //}
        }

        /// <summary>
        /// Deletes a wall and removes it from the wall list.
        /// </summary>
        public void DeleteWall() {
            if (Building.Instance.Walls.Contains(this)) {
                Building.Instance.RemoveWall(this);
            }
            Destroy(gameObject);
        }

        public IList<IList<Vector3>> GetHoleVertices() {
            IList<IList<Vector3>> allHoleVertices = new List<IList<Vector3>>();
            for (int i = 0; i < interFace.GetCoincidentOpenings().Count; i++) {
                List<Vector3> holeVertices = interFace.GetCoincidentOpenings()[i].GetControlPoints()
                    .Select(p => gameObject.transform.InverseTransformPoint(p)).ToList();

                allHoleVertices.Add(holeVertices.Select(p => p -= Vector3.up * (wallThickness / 2)).ToList());
            }
            return allHoleVertices;
        }
    }
}