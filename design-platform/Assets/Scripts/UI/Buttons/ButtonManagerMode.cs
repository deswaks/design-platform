﻿using DesignPlatform.Core;
using DesignPlatform.Modes;
using UnityEngine;

namespace DesignPlatform.UI {
    public class ButtonManagerMode : MonoBehaviour {
        public void StartSelectMode() {
            Main.Instance.SetMode(SelectMode.Instance);
        }
        public void StartBuildMode(int buildShape) {
            BuildMode.Instance.SelectedShape = (SpaceShape)buildShape;
            Main.Instance.SetMode(BuildMode.Instance);
        }
        public void StartPOVMode() {
            Main.Instance.SetMode(POVMode.Instance);
        }
        public void StartOpeningMode(int openingShape) {
            OpeningMode.Instance.SelectedFunction = (OpeningFunction)openingShape;
            Main.Instance.SetMode(OpeningMode.Instance);
        }
    }
}