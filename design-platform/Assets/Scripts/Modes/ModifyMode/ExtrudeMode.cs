using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrudeMode : Mode {

    private static ExtrudeMode instance;
    public static ExtrudeMode Instance {
        get { return instance ?? (instance = new ExtrudeMode()); }
    }

    public override void Tick() {
    }

    public override void OnModeResume() {
        if (SelectMode.Instance.selection != null) {
            SelectMode.Instance.selection.SetEditHandles();
        }
    }

    public override void OnModePause() {
        if (SelectMode.Instance.selection != null) {
            SelectMode.Instance.selection.RemoveEditHandles();
            SelectMode.Instance.selection.ResetOrigin();
        }
    }


}
