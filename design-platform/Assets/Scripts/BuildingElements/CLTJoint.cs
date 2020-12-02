using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Core {

    /// <summary>
    /// Describes the type of a joint between walls.
    /// </summary>
    public enum WallJointType {
        /// <summary> No type </summary>
        None,
        /// <summary> Element is extended to side of other element. </summary>
        Corner_Primary,
        /// <summary> Element is shortened. </summary>
        Corner_Secondary,
        /// <summary> Element going through (element has midpoint in T-joint). </summary>
        T_Primary,
        /// <summary> Element ends onto primary element (endpoint in T-joint). </summary>
        T_Secondary,
        /// <summary> Element going through (element has midpoint in X-joint). </summary>
        X_Primary,
        /// <summary> Element ends onto primary element (endpoint in X-joint). </summary>
        X_Secondary,
        /// <summary> The walls are parallel. </summary>
        Parallel
    }

    public struct CLTJoint {

        public CLTElement Element { get; set; }

        public Vector3 Point { get; set; }

        public WallJointType Type { get; set; }

        public float Parameter {
            get { return Element.Length; }
        }

        public CLTJoint (CLTElement parent, Vector3 location, WallJointType type = WallJointType.None) {
            Element = parent;
            Point = location;
            Type = type;
        }
    }
}