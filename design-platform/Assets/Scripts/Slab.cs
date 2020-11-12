using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using DesignPlatform.Utils;

namespace DesignPlatform.Core {
    public class Slab : MonoBehaviour {
        public Interface interFace { get; private set; }
        public float slabThickness = 0.2f;
        public Material slabMaterial;
        private Slab prefabSlab;

        /// <summary>
        /// Construct slabs.
        /// </summary>
        public void InitializeSlab(Interface interFace) {

            this.interFace = interFace;

            gameObject.layer = 14; // Slab layer

            GameObject prefabSlabObject = AssetUtil.LoadAsset<GameObject>("prefabs", "SlabPrefab");

            prefabSlab = (Slab)prefabSlabObject.GetComponent(typeof(Slab));
            gameObject.name = "CLT Slab";

            List<Vector3> slabControlPoints = interFace.attachedFaces[0].parentRoom.GetControlPoints(localCoordinates: true);

            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            mesh.CreateShapeFromPolygon(slabControlPoints, -slabThickness, false);

            gameObject.transform.position = interFace.attachedFaces[0].parentRoom.transform.position
               + Vector3.up * interFace.attachedFaces[0].GetControlPoints()[0].y;

            mesh.GetComponent<MeshRenderer>().material = prefabSlab.slabMaterial;

        }

        /// <summary>
        /// Deletes a slab and removes it from the slab list.
        /// </summary>
        public void DeleteSlab() {
            if (Building.Instance.slabs.Contains(this)) {
                Building.Instance.RemoveSlab(this);
            }
            Destroy(gameObject);
        }
    }
}