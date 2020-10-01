using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public static class VectorFunctions
{
    public static int IndexLargestComponent(Vector3 v) {
        int indexLargestComponent = 0;
        for (int i = 0; i <= 2; i++) {
            if (v[i] > v[indexLargestComponent]) {
                indexLargestComponent = i;
            }
        }
        return indexLargestComponent;
    }

    public static int IndexNumLargestComponent(Vector3 v) {
        int indexLargestComponent = 0;
        for (int i = 0; i <= 2; i++) {
            if (Mathf.Abs(v[i]) > Mathf.Abs(v[indexLargestComponent])) {
                indexLargestComponent = i;
            }
        }
        return indexLargestComponent;
    }

    public static List<float> ListFromVector(Vector3 v) {
        List<float> vectorList = new List<float>();
        for (int i = 0; i <= 2; i++) {
            vectorList.Add(v[i]);
        }
        return vectorList;
    }

}
