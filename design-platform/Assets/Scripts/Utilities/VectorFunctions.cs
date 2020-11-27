using DesignPlatform.Geometry;
using System.Collections.Generic;
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
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Rotate90ClockwiseXZ(Vector3 vector) {
            return new Vector3(-vector.z, 0, vector.x);
        }
    }
}