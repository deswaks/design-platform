

namespace DesignPlatform.Core {

    /// <summary>
    /// Describes the orientation, eg. of interfaces and faces.
    /// </summary>
    public enum Orientation {
        /// <summary> For elements oriented horizontally </summary>
        HORIZONTAL,
        /// <summary> For elements oriented vertically </summary>
        VERTICAL
    }
    
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
}