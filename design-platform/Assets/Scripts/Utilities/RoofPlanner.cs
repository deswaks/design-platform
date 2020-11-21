﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit.Buildings;
using ProceduralToolkit;
using System.Linq;
using System;

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
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer){
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return _(); IEnumerable<TSource> _()
            {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach (var element in source)
                {
                    if (knownKeys.Add(keySelector(element)))
                        yield return element;
                }
            }
        }
        public static int NumberOfSharedVertices(Vector3[] triangle1, Vector3[] triangle2) {
            return triangle1.ToList().SelectMany(v1 => triangle2.Where(v2 => Vector3.Distance(v1, v2) < 0.01)).Count();
        }

        public static List<Vector3> DistinctVertices(List<Vector3> vertices) {
            
            List<Vector3> culledVertices = vertices.DistinctBy(v => new { X = Math.Round(v.x,1), Y= Math.Round(v.y,1), Z = Math.Round(v.z,1)}).ToList();

            return culledVertices;
        }

        public static List<Vector3> OrderClockwise(List<Vector3> vertices, Vector3 axis) {

            Vector3 midpoint = Midpoint(vertices);
            Vector3 start = vertices[0] - midpoint;

            List<Vector3> orderedVertices = vertices.OrderBy(v => Vector3.SignedAngle(start,v-midpoint, axis)).ToList();

            return orderedVertices;
        }

        public static Vector3 Midpoint(List<Vector3> vertices){
            Vector3 midpoint = new Vector3();
            vertices.ForEach(v => midpoint += v);
            midpoint /= vertices.Count();

            return midpoint;
        }

        public static List<List<T>> SplitList<T>(this List<T> me, int size = 50){
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Mathf.Min(size, me.Count - i)));
            return list;
        }

        public static MeshDraft ConstructGableDraft(List<Vector2> skeletonPolygon2, float roofPitch)
        {
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