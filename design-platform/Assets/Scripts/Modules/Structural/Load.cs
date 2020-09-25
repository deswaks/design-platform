using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structural {
    public struct Load {
        public float pStart;
        public float pEnd;
        public float magnitude;

        public Load(float start = 0.0f, float end = 0.0f, float mag = 0.0f) {
            pStart = start;
            pEnd = end;
            magnitude = mag;
        }
    }
}
