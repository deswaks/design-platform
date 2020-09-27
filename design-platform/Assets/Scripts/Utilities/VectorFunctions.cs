using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public static class VectorFunctions
{
    public static int IndexLargestComponent(Vector3 v) {
        int indexLargestComponent = 0;
        for (int i = 0; i < 2; i++) {
            if (v[i] > v[indexLargestComponent]) {
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
        return new Vector3 ( v[0] + addition, v[1] + addition, v[2] + addition );
    }

}
