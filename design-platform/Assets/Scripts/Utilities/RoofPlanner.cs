using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit.Buildings;
using ProceduralToolkit;

namespace DesignPlatform.Core {
    public class ProceduralRoofPlanner : RoofPlanner {
        public override IConstructible<MeshDraft> Plan(List<Vector2> foundationPolygon, BuildingGenerator.Config config) {
            switch (config.roofConfig.type) {
                case RoofType.Flat:
                    return new ProceduralFlatRoof(foundationPolygon, config.roofConfig, config.palette.roofColor);
                case RoofType.Hipped:
                    return new ProceduralHippedRoof(foundationPolygon, config.roofConfig, config.palette.roofColor);
                case RoofType.Gabled:
                    return new ProceduralGabledRoof(foundationPolygon, config.roofConfig, config.palette.roofColor);
                default:
                    return new ProceduralGabledRoof(foundationPolygon, config.roofConfig, config.palette.roofColor);
            }
        }
    }
    public class ProceduralRoofConstructor : RoofConstructor {
        [SerializeField]
        private RendererProperties rendererProperties = null;
        [SerializeField]
        private Material roofMaterial = null;

        public override void Construct(IConstructible<MeshDraft> constructible, Transform parentTransform) {
            var draft = constructible.Construct(Vector2.zero);

            var meshFilter = parentTransform.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = draft.ToMesh();

            var meshRenderer = parentTransform.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.ApplyProperties(rendererProperties);
            meshRenderer.material = roofMaterial;
        }
    }

    public static class RoofUtils {
        public static List<List<T>> SplitList<T>(this List<T> me, int size = 50) {
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Mathf.Min(size, me.Count - i)));
            return list;
        }
        public static MeshDraft ConstructGableDraft(List<Vector2> skeletonPolygon2, float roofPitch) {
            Vector2 edgeA2 = skeletonPolygon2[0];
            Vector2 edgeB2 = skeletonPolygon2[1];
            Vector2 peak2 = skeletonPolygon2[2];
            Vector2 edgeDirection2 = (edgeB2 - edgeA2).normalized;

            float peakHeight = CalculateVertexHeight(peak2, edgeA2, edgeDirection2, roofPitch);
            Vector3 edgeA3 = edgeA2.ToVector3XZ();
            Vector3 edgeB3 = edgeB2.ToVector3XZ();
            Vector3 peak3 = new Vector3(peak2.x, peakHeight, peak2.y);
            Vector2 gableTop2 = Closest.PointSegment(peak2, edgeA2, edgeB2);
            Vector3 gableTop3 = new Vector3(gableTop2.x, peakHeight, gableTop2.y);

            return new MeshDraft().AddTriangle(edgeA3, edgeB3, gableTop3, true)
                .AddTriangle(edgeA3, gableTop3, peak3, true)
                .AddTriangle(edgeB3, peak3, gableTop3, true);
        }
        public static MeshDraft ConstructContourDraft(List<Vector2> skeletonPolygon2, float roofPitch) {
            Vector2 edgeA = skeletonPolygon2[0];
            Vector2 edgeB = skeletonPolygon2[1];
            Vector2 edgeDirection2 = (edgeB - edgeA).normalized;
            Vector3 roofNormal = CalculateRoofNormal(edgeDirection2, roofPitch);

            var skeletonPolygon3 = skeletonPolygon2.ConvertAll(v => v.ToVector3XZ());

            var tessellator = new Tessellator();
            tessellator.AddContour(skeletonPolygon3);
            tessellator.Tessellate(normal: Vector3.up);
            var contourDraft = tessellator.ToMeshDraft();

            for (var i = 0; i < contourDraft.vertexCount; i++) {
                Vector2 vertex = contourDraft.vertices[i].ToVector2XZ();
                float height = CalculateVertexHeight(vertex, edgeA, edgeDirection2, roofPitch);
                contourDraft.vertices[i] = new Vector3(vertex.x, height, vertex.y);
                contourDraft.normals.Add(roofNormal);
            }
            return contourDraft;
        }
        public static float CalculateVertexHeight(Vector2 vertex, Vector2 edgeA, Vector2 edgeDirection, float roofPitch) {
            float distance = Distance.PointLine(vertex, edgeA, edgeDirection);
            return Mathf.Tan(roofPitch * Mathf.Deg2Rad) * distance;
        }

        public static Vector3 CalculateRoofNormal(Vector2 edgeDirection2, float roofPitch) {
            return Quaternion.AngleAxis(roofPitch, edgeDirection2.ToVector3XZ()) * Vector3.up;
        }


    }

}