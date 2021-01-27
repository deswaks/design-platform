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

        /// <summary>Thickness perpendicular to the surface of the roof.</summary>
        public float Thickness { get; private set; }

        /// <summary>Pitch of the slope of the roof.</summary>
        public float Pitch { get; private set; }

        /// <summary>Determines the offset of the roof edge from the wall edges.</summary>
        public float Overhang { get; private set; }

        /// <summary>Type of roof can be Gabled, Hipped or Flat.</summary>
        public RoofType Type { get; private set; }


        /// <summary>
        /// Underlying controlpoints of the roof element. The vertices of the polygon that gets extruded into the 3d geometry.
        /// </summary>
        public List<Vector3> ControlPoints { get; private set; }


        /// <summary>
        /// The default constructor of the roof must come in the form of this initialize function
        /// since the roof is a Unity MonoBehavior object (cannot have a constructor).
        /// Any roof must be initialized.
        /// </summary>
        /// <param name="roofFaceVertices">The vertices for the extrusion into 3d object.</param>
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
            ControlPoints = VectorUtils.TransformPointsToXZ(roofFaceVertices, out location, out rotationVector);

            gameObject.layer = 13; // Wall layer
            Material roofMaterial = AssetUtil.LoadAsset<Material>("materials", "CLT");

            // Creates roof mesh
            gameObject.AddComponent<MeshCollider>();
            ProBuilderMesh mesh = gameObject.AddComponent<ProBuilderMesh>();

            //Debug.Log(Normal.ToString());
            if (Normal.y < 0.0f) Thickness = -Thickness;
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