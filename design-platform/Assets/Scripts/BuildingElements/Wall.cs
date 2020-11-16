using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {
    public class Wall : MonoBehaviour {

        public List<Room> Rooms {
            get { return Faces.Select(f => f.Room).ToList(); }
            set {; }
        }
        public List<Face> Faces { get; private set; }
        public Interface Interface {
            get {
                if (Faces != null && Faces.Count() > 0) {
                    return Faces[0].InterfaceWalls.FirstOrDefault(x => x.Value == this).Key;
                }
                else return null;
            }
            private set {; }
        }
        public List<Opening> Openings {
            get { return Faces.SelectMany(f => f.InterfaceOpenings[Interface]).ToList().Distinct().ToList(); }
            private set {; }
        }

        private List<Vector3> wallControlPoints { get; set; }

        public float wallThickness = 0.2f;
        public Vector3 Normal {
            get { return Rooms[0].GetWallNormals()[Faces[0].FaceIndex]; }
            private set {; }
        }
        public float Length {
            get { return (Interface.GetStartPoint() - Interface.GetEndPoint()).magnitude; }
            private set {; }
        }
        public float Height {
            get { return Rooms[0].height; }
            private set {; }
        }
        public Vector3 CenterPoint {
            get { return Interface.GetCenterPoint(); }
            private set {; }
        }


        /// <summary>
        /// Construct walls.
        /// </summary>
        public void InitializeWall(Interface interFace) {
            Interface = interFace;
            gameObject.layer = 13; // Wall layer

            Material wallMaterial = AssetUtil.LoadAsset<Material>("materials", "wallMaterial");
            gameObject.name = "CLT Wall";

            wallControlPoints = new List<Vector3> {
                    new Vector3 (Length/2,0,0),
                    new Vector3 (Length/2,0,Height),
                    new Vector3 (-Length/2,0,Height),
                    new Vector3 (-Length/2,0,0) };

            gameObject.transform.position = CenterPoint;
            gameObject.transform.rotation = Quaternion.LookRotation(Vector3.up, Normal);

            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            List<Vector3> wallMeshControlPoints = wallControlPoints.Select(p => p -= Vector3.up * (wallThickness / 2)).ToList();

            if (interFace.OpeningsVertical.Count > 0) {
                mesh.CreateShapeFromPolygon(wallMeshControlPoints, wallThickness, false, GetHoleVertices());
            }

            if (interFace.OpeningsVertical.Count < 1) {
                mesh.CreateShapeFromPolygon(wallMeshControlPoints, wallThickness, false);
            }
            mesh.GetComponent<MeshRenderer>().material = wallMaterial;
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
            for (int i = 0; i < Openings.Count; i++) {
                List<Vector3> holeVertices = Openings[i].GetControlPoints()
                    .Select(p => gameObject.transform.InverseTransformPoint(p)).ToList();

                allHoleVertices.Add(holeVertices.Select(p => p -= Vector3.up * (wallThickness / 2)).ToList());
            }
            return allHoleVertices;
        }
    }
}