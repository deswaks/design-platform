namespace DesignPlatform.Core {
    public enum Orientation {
        HORIZONTAL,
        VERTICAL
    }

    public enum Axis {
        X,
        Y,
        Z
    }

    public enum WallType {
        STANDARD = 0,
        LOADCARRYING = 1
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