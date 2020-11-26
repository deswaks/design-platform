﻿using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Utils {

    /// <summary>
    /// Helper functions for vector objects
    /// </summary>
    public static class VectorFunctions {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int IndexLargestComponent(Vector3 v) {
            int indexLargestComponent = 0;
            for (int i = 0; i < 3; i++) {
                if (v[i] > v[indexLargestComponent]) {
                    indexLargestComponent = i;
                }
            }
            return indexLargestComponent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int IndexAbsLargestComponent(Vector3 v) {
            int indexLargestComponent = 0;
            for (int i = 0; i < 3; i++) {
                if (Mathf.Abs(v[i]) > Mathf.Abs(v[indexLargestComponent])) {
                    indexLargestComponent = i;
                }
            }
            return indexLargestComponent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static List<float> ListFromVector(Vector3 v) {
            List<float> vectorList = new List<float>();
            for (int i = 0; i < 2; i++) {
                vectorList.Add(v[i]);
            }
            return vectorList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="addition"></param>
        /// <returns></returns>
        public static Vector3 AddConstant(Vector3 v, float addition) {
            return new Vector3(v[0] + addition, v[1] + addition, v[2] + addition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 LineClosestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
            var vVector1 = point - lineStart;
            var vVector2 = (lineEnd - lineStart).normalized;

            var d = Vector3.Distance(lineStart, lineEnd);
            var t = Vector3.Dot(vVector2, vVector1);

            if (t <= 0) return lineStart;
            if (t >= d) return lineEnd;

            var vVector3 = vVector2 * t;
            var vClosestPoint = lineStart + vVector3;

            return vClosestPoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Rotate90ClockwiseXZ(Vector3 vector) {
            return new Vector3(-vector.z, 0, vector.x);
        }
    }
}