using DesignPlatform.Geometry;
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

        /// <summary>The underlying controlpoints for the polyline of the wall</summary>
        private List<Vector3> controlPoints;


        /// <summary>
        /// Construct wall elements based on interface information.
        /// </summary>
        /// <param name="interFace">Interface to base the wall upon.</param>
        public void InitializeWall(Interface interFace) {
            Interfaces.Add(interFace);
            gameObject.name = "Generic Wall";

            controlPoints = new List<Vector3> {
                    new Vector3 (interFace.Length/2,0,0),
                    new Vector3 (interFace.Length/2,0, Height-0.01f),
                    new Vector3 (-interFace.Length/2,0, Height-0.01f),
                    new Vector3 (-interFace.Length/2,0,0) };

            gameObject.transform.position = interFace.CenterPoint;
            gameObject.transform.rotation = Quaternion.LookRotation(Vector3.up, Normal);
            InitializeWallFinish();
        }

        /// <summary>
        /// Construct wall elements based on CLT element information
        /// </summary>
        public void InitializeWall(CLTElement wallElement) {
            Interfaces = wallElement.interfaces;
            gameObject.name = "CLT Wall";

            controlPoints = new List<Vector3> {
                    new Vector3 (wallElement.Length/2, 0, 0),
                    new Vector3 (wallElement.Length/2, 0, wallElement.Height),//-0.01f),
                    new Vector3 (-wallElement.Length/2, 0, wallElement.Height),//-0.01f),
                    new Vector3 (-wallElement.Length/2, 0, 0)
            };

            // Adjust end points according to joint types
            if (wallElement.startPoint.jointType.ToString().Contains("Secondary")) {
                controlPoints[0] -= new Vector3(Thickness / 2, 0, 0);
                controlPoints[1] -= new Vector3(Thickness / 2, 0, 0);
            }
            if (wallElement.startPoint.jointType.ToString().Contains("Primary")) {
                controlPoints[0] += new Vector3(Thickness / 2, 0, 0);
                controlPoints[1] += new Vector3(Thickness / 2, 0, 0);
            }
            if (wallElement.endPoint.jointType.ToString().Contains("Secondary")) {
                controlPoints[2] += new Vector3(Thickness / 2, 0, 0);
                controlPoints[3] += new Vector3(Thickness / 2, 0, 0);
            }
            if (wallElement.endPoint.jointType.ToString().Contains("Primary")) {
                controlPoints[2] -= new Vector3(Thickness / 2, 0, 0);
                controlPoints[3] -= new Vector3(Thickness / 2, 0, 0);
            }
            gameObject.transform.position = wallElement.CenterPoint;
            gameObject.transform.rotation = Quaternion.LookRotation(Vector3.up, -VectorFunctions.Rotate90ClockwiseXZ(wallElement.GetDirection()));
            InitializeWallFinish();
        }

        /// <summary>
        /// Collective internal initializer for the overloaded initializers.
        /// </summary>
        private void InitializeWallFinish() {
            gameObject.layer = 13;

            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();
            List<Vector3> meshControlPoints = controlPoints
                .Select(p => p -= Vector3.up * (Thickness / 2)).ToList();

            if (Openings.Count == 0) {
                mesh.CreateShapeFromPolygon(meshControlPoints, Thickness, false);
            }
            else {
                mesh.CreateShapeFromPolygon(meshControlPoints, Thickness, false, GetHoleVertices());
            }
            Material wallMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");
            mesh.GetComponent<MeshRenderer>().material = wallMaterial;
        }

        /// <summary>The spaces adjacent to this wall.</summary>
        public List<Space> Spaces {
            get { return Faces.Select(f => f.Space).ToList(); }
        }

        /// <summary>The faces coincident with this wall.</summary>
        public List<Face> Faces {
            get { return Interfaces.SelectMany(iface => iface.Faces).ToList(); }
        }

        /// <summary>The interfaces coincident with this wall.</summary>
        public List<Interface> Interfaces { get; private set; }

        /// <summary>The openings on this wall.</summary>
        public List<Opening> Openings {
            get { return Interfaces.SelectMany(iface => iface.Openings).ToList(); }
        }

        /// <summary>The maximum preferred thickness of all the connected faces</summary>
        public float Thickness {
            get {
                return Interfaces.Where(f => f != null).Max(i => i.Thickness);
            }
        }

        /// <summary>Normal vector of the wall.</summary>
        public Vector3 Normal {
            get { return Spaces[0].GetWallNormals()[Faces[0].SpaceIndex]; }
        }

        /// <summary>Total length of the wall.</summary>
        public float Length {
            get { return Vector3.Distance(controlPoints[0], controlPoints[controlPoints.Count - 1]); }
        }

        /// <summary>The height of the heighest adjacent space along the wall.</summary>
        public float Height {
            get { return Spaces.Max(s => s.height); }
        }

        

        /// <summary>
        /// Deletes a wall and removes it from the wall list.
        /// </summary>
        public void Delete() {
            if (Building.Instance.Walls.Contains(this)) {
                Building.Instance.RemoveWall(this);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Finds the vertices of all the holes on this wall,
        /// where the vertices are transformed to the local coordinate system of the wall.
        /// </summary>
        /// <returns>List of holes, where each element is a list of its vertices.</returns>
        private IList<IList<Vector3>> GetHoleVertices() {
            IList<IList<Vector3>> allHoleVertices = new List<IList<Vector3>>();
            for (int i = 0; i < Openings.Count; i++) {
                List<Vector3> holeVertices = Openings[i].GetControlPoints()
                    .Select(p => gameObject.transform.InverseTransformPoint(p)).ToList();

                allHoleVertices.Add(holeVertices.Select(p => p -= Vector3.up * (Thickness / 2)).ToList());
            }
            return allHoleVertices;
        }
    }
}