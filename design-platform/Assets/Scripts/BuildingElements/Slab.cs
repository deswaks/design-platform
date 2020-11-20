using DesignPlatform.Utils;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace DesignPlatform.Core {
    public class Slab : MonoBehaviour {

        public List<Room> Rooms {
            get { return Faces.Select(f => f.Room).ToList(); }
        }
        public List<Face> Faces {
            get { return Interface.Faces; }
        }
        public Interface Interface { get; private set; }

        public List<Opening> Openings {
            get { return Interface.Openings; }
        }

        public float Thickness = 0.1f;


        /// <summary>
        /// Construct slabs.
        /// </summary>
        public void InitializeSlab(Interface interFace) {
            Interface = interFace;

            gameObject.layer = 14; // Slab layer
            Material slabMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");
            gameObject.name = "CLT Slab";

            List<Vector3> slabControlPoints = Rooms[0].GetControlPoints(localCoordinates: true);

            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            mesh.CreateShapeFromPolygon(slabControlPoints, -Thickness, false);

            gameObject.transform.position = Rooms[0].transform.position
               + Vector3.up * Faces[0].GetControlPoints()[0].y;
            gameObject.transform.rotation = Rooms[0].transform.rotation;

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