using DesignPlatform.Utils;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using DesignPlatform.Geometry;

namespace DesignPlatform.Core {

    /// <summary>
    /// Represents horizontal space-dividing building elements such as floors and ceilings.
    /// </summary>
    public class Slab : MonoBehaviour {

        public float Thickness = 0.1f;

        /// <summary>The spaces adjacent to this slab.</summary>
        public List<Space> Spaces {
            get { return Faces.Select(f => f.Space).ToList(); }
        }

        /// <summary>The faces coincident with this slab.</summary>
        public List<Face> Faces {
            get { return Interface.Faces; }
        }

        /// <summary>The interface coincident with this slab.</summary>
        public Interface Interface { get; private set; }

        /// <summary>The openings on this slab.</summary>
        public List<Opening> Openings {
            get { return Interface.Openings; }
        }

        /// <summary>
        /// Construct slabs.
        /// </summary>
        public void InitializeSlab(Interface interFace) {
            Interface = interFace;

            gameObject.layer = 14; // Slab layer
            Material slabMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");
            gameObject.name = "CLT Slab";

            List<Vector3> slabControlPoints = Spaces[0].GetControlPoints(localCoordinates: true);

            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            mesh.CreateShapeFromPolygon(slabControlPoints, -Thickness, false);

            gameObject.transform.position = Spaces[0].transform.position
               + Vector3.up * Faces[0].GetControlPoints()[0].y;
            gameObject.transform.rotation = Spaces[0].transform.rotation;

            mesh.GetComponent<MeshRenderer>().material = slabMaterial;

        }

        /// <summary>
        /// Deletes a slab and removes it from the slab list.
        /// </summary>
        public void DeleteSlab() {
            if (Building.Instance.Slabs.Contains(this)) {
                Building.Instance.RemoveSlab(this);
            }
            Destroy(gameObject);
        }
    }
}