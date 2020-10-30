using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Core {
    public enum Orientation {
        HORIZONTAL,
        VERTICAL
    }

    public enum RoomShape {
        RECTANGLE,
        LSHAPE,
        USHAPE,
        SSHAPE
    }

    public enum Axis {
        X,
        Y,
        Z
    }

    public enum RoomType {
        //Til senere implementering
        DELETED = -2,
        HIDDEN = -1,
        //------------------
        PREVIEW = 0,
        DEFAULT = 1,
        SINGLEROOM = 10,
        DOUBLEROOM = 11,
        LIVINGROOM = 12,
        KITCHEN = 13,
        BATHROOM = 14
    }

    public enum WallType {
        STANDARD = 0,
        LOADCARRYING = 1
    }
}