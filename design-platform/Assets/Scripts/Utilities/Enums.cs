

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
    
    public enum WallJointType {
        None,
        Corner_Primary,     // Element is extended to side of other element
        Corner_Secondary,   // Element is shortened
        T_Primary,          // Element going through (element has midpoint in T-joint)
        T_Secondary,        // Element ends onto primary element (endpoint in T-joint)
        X_Primary,          // Element going through (element has midpoint in X-joint)
        X_Secondary,        // Element ends onto primary element (endpoint in X-joint)
        Parallel
    }
}