using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IndexingUtils
{
    public static int wrapIndex(int i, int listLength) {
        if (i > listLength) {
            i = i % listLength;
        }
        if (i < 0) {

        }
        return i;
    }
}
