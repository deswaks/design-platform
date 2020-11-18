using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Utils {
    public static class VectorFunctions {
        public static int IndexLargestComponent(Vector3 v) {
            int indexLargestComponent = 0;
            for (int i = 0; i < 3; i++) {
                if (v[i] > v[indexLargestComponent]) {
                    indexLargestComponent = i;
                }
            }
            return indexLargestComponent;
        }

        public static int IndexAbsLargestComponent(Vector3 v) {
            int indexLargestComponent = 0;
            for (int i = 0; i < 3; i++) {
                if (Mathf.Abs(v[i]) > Mathf.Abs(v[indexLargestComponent])) {
                    indexLargestComponent = i;
                }
            }
            return indexLargestComponent;
        }

        public static List<float> ListFromVector(Vector3 v) {
            List<float> vectorList = new List<float>();
            for (int i = 0; i < 2; i++) {
                vectorList.Add(v[i]);
            }
            return vectorList;
        }

        public static Vector3 AddConstant(Vector3 v, float addition) {
            return new Vector3(v[0] + addition, v[1] + addition, v[2] + addition);
        }
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
    }
}