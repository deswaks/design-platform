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

            GameObject prefabSlabObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SlabPrefab.prefab");
            prefabSlab = (Slab)prefabSlabObject.GetComponent(typeof(Slab));
            gameObject.name = "CLT Slab";

            List<Vector3> slabControlPoints = interFace.GetSlabControlPoints(localCoordinates: false);


            gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent<PolyShape>();
            gameObject.AddComponent<ProBuilderMesh>();

            PolyShape polyshape = gameObject.GetComponent<PolyShape>();
            polyshape.SetControlPoints(slabControlPoints);
            polyshape.extrude = slabThickness;
            polyshape.CreateShapeFromPolygon();

            gameObject.GetComponent<ProBuilderMesh>().Refresh();
            gameObject.GetComponent<MeshRenderer>().material = prefabSlab.slabMaterial;

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