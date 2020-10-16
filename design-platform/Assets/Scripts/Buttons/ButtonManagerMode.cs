using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerMode : MonoBehaviour {
    public Main main;
    public void StartSelectMode() {
        Main.Instance.SetMode(SelectMode.Instance);
    }

    public void StartBuildMode(int buildShape) {
        BuildMode.Instance.SetSelectedShape((RoomShape)buildShape);
        Main.Instance.SetMode(BuildMode.Instance);
    }

    public void StartPOVMode() {
        Main.Instance.SetMode(POVMode.Instance);
    }



}
