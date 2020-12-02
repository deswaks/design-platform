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


    /// <summary>
    /// The CLT joint represents a joint between two structural panels made of CLT (cross laminated timber)
    /// </summary>
    public struct CLTJoint {

        /// <summary>The element on which the joint is located.</summary>
        public CLTElement Element { get; set; }

        /// <summary>The point at which the joint is located.</summary>
        public Vector3 Point { get; set; }

        /// <summary>The type of joint in relation to the connecting elements.</summary>
        public WallJointType Type { get; set; }

        /// <summary>The parameter on the CLT element location line where the joint is located.</summary>
        public float Parameter {
            get { return Element.Length; }
        }

        /// <summary>Default constructor for the joint.</summary>
        public CLTJoint (CLTElement parent, Vector3 location, WallJointType type = WallJointType.None) {
            Element = parent;
            Point = location;
            Type = type;
        }
    }
}