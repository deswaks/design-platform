using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using ProceduralToolkit.Buildings;

namespace DesignPlatform.Core {

    /// <summary>
    /// Represents the roof of the building
    /// </summary>
    public class Roof : MonoBehaviour {

        public float Thickness { get; private set; }
        public float Pitch { get; private set; }
        public float Overhang { get; private set; }
        public RoofType Type { get; private set; }

        public List<Vector3> ControlPoints { get; private set; }

        /// <summary>
        /// Construct roof.
        /// </summary>
        public void InitializeRoof(List<Vector3> roofFaceVertices) {
            // Save values at instantiation
            Thickness = Settings.RoofThickness;
            Pitch = Settings.RoofPitch;
            Overhang = Settings.RoofOverhang;
            Type = Settings.RoofType;

            Vector3 Normal = Vector3.Cross(roofFaceVertices[2] - roofFaceVertices[1], roofFaceVertices[0] - roofFaceVertices[1]).normalized;

            // Moves vertical roof faces (gables) in towards the building, if there is overhang
            if (Mathf.Abs(Vector3.Dot(Vector3.up, Normal)) < 0.01) {
                roofFaceVertices = roofFaceVertices.Select(v => v + -Overhang * Normal).ToList();
            }

            // Transforms points to origin and in XZ plane
            Vector3 location; 
            Vector3 rotationVector; 
            ControlPoints = RoofUtils.TransformPointsToXZ(roofFaceVertices, out location, out rotationVector);
            
            gameObject.layer = 13; // Wall layer
            Material roofMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");

            // Creates roof mesh
            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            mesh.CreateShapeFromPolygon(ControlPoints, -Thickness, false);
            mesh.GetComponent<MeshRenderer>().material = roofMaterial;

            gameObject.transform.position = location + new Vector3(0, 3.0f, 0);
            gameObject.transform.rotation = Quaternion.Euler(-rotationVector);
        }

        /// <summary>
        /// Deletes a Roof and removes it from the Roof list.
        /// </summary>
        public void DeleteRoof() {
            if (Building.Instance.Roofs.Contains(this)) {
                Building.Instance.RemoveRoof(this);
            }
            Destroy(gameObject);
        }

    }
}