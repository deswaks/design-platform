using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {

    /// <summary>
    /// Represents building wall as a vertical space-dividing building elements.
    /// </summary>
    public class Wall : MonoBehaviour {

        public List<Space> Spaces {
            get { return Faces.Select(f => f.Space).ToList(); }
        }
        public List<Face> Faces {
            get { return Interfaces.SelectMany(iface => iface.Faces).ToList(); }
        }
        //public Interface Interface { get; private set; }
        public List<Interface> Interfaces { get; private set; }

        public List<Opening> Openings {
            get { return Interfaces.SelectMany(iface => iface.Openings).ToList(); }
        }

        private List<Vector3> wallControlPoints { get; set; }

        public float wallThickness = 0.2f;
        public Vector3 Normal {
            get { return Spaces[0].GetWallNormals()[Faces[0].FaceIndex]; }
        }
        public float Length {
            get { return (wallControlPoints[0] - wallControlPoints[2]).magnitude; }
        }
        public float Height {
            get { return Spaces[0].height; }
        }


        /// <summary>
        /// Construct walls.
        /// </summary>
        public void InitializeWall(Interface interFace) {
            //Interface = interFace;
            Interfaces.Add(interFace);

            gameObject.layer = 13; // Wall layer

            Material wallMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");
            gameObject.name = "Generic Wall";

            wallControlPoints = new List<Vector3> {
                    new Vector3 (interFace.Length/2,0,0),
                    new Vector3 (interFace.Length/2,0,Height-0.01f),
                    new Vector3 (-interFace.Length/2,0,Height-0.01f),
                    new Vector3 (-interFace.Length/2,0,0) };

            gameObject.transform.position = interFace.CenterPoint;
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
        /// Construct wall elements based on CLT element information
        /// </summary>
        public void InitializeWall(CLTElement wallElement) {
            Interfaces = wallElement.interfaces;

            gameObject.layer = 13; // Wall layer

            Material wallMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");
            gameObject.name = "CLT Wall";

            List<Vector3> wallControlPoints = new List<Vector3> {
                    new Vector3 (wallElement.Length/2, 0, 0),
                    new Vector3 (wallElement.Length/2, 0, wallElement.Height),//-0.01f),
                    new Vector3 (-wallElement.Length/2, 0, wallElement.Height),//-0.01f),
                    new Vector3 (-wallElement.Length/2, 0, 0) };

            // Adjust end points according to joint types
            if (wallElement.startPoint.jointType.ToString().Contains("Secondary")) {
                wallControlPoints[0] -= new Vector3(wallThickness / 2, 0, 0);
                wallControlPoints[1] -= new Vector3(wallThickness / 2, 0, 0);
            }
            if (wallElement.startPoint.jointType.ToString().Contains("Primary")) {
                wallControlPoints[0] += new Vector3(wallThickness / 2, 0, 0);
                wallControlPoints[1] += new Vector3(wallThickness / 2, 0, 0);
            }
            if (wallElement.endPoint.jointType.ToString().Contains("Secondary")) {
                wallControlPoints[2] += new Vector3(wallThickness / 2, 0, 0);
                wallControlPoints[3] += new Vector3(wallThickness / 2, 0, 0);
            }
            if (wallElement.endPoint.jointType.ToString().Contains("Primary")) {
                wallControlPoints[2] -= new Vector3(wallThickness / 2, 0, 0);
                wallControlPoints[3] -= new Vector3(wallThickness / 2, 0, 0);
            }


            gameObject.transform.position = wallElement.CenterPoint;
            gameObject.transform.rotation = Quaternion.LookRotation(Vector3.up, -VectorFunctions.Rotate90ClockwiseXZ(wallElement.GetDirection()));

            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            List<Vector3> wallMeshControlPoints = wallControlPoints.Select(p => p -= Vector3.up * (wallThickness / 2)).ToList();

            if (wallElement.interfaces.Any(interFace => interFace.OpeningsVertical.Count > 0)) {
                mesh.CreateShapeFromPolygon(wallMeshControlPoints, wallThickness, false, GetHoleVertices());
            }
            else {
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