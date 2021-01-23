using DesignPlatform.Core;
using DesignPlatform.Modes;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// Manages functions for changing modes. 
    /// </summary>
    public class ButtonManagerMode : MonoBehaviour {
        /// <summary>
        /// Initiates Select Mode
        /// </summary>
        public void StartSelectMode() {
            Main.Instance.SetMode(SelectMode.Instance);
        }
        /// <summary>
        /// Initiates Build Mode
        /// </summary>
        /// <param name="buildShape"></param>
        public void StartBuildMode(int buildShape) {
            BuildMode.Instance.SelectedShape = (SpaceShape)buildShape;
            Main.Instance.SetMode(BuildMode.Instance);
        }
        /// <summary>
        /// Initiates 3D Mode
        /// </summary>
        public void StartPOVMode() {
            Main.Instance.SetMode(POVMode.Instance);
        }
        /// <summary>
        /// Initiates Opening Mode
        /// </summary>
        /// <param name="openingShape"></param>
        public void StartOpeningMode(int openingShape) {
            OpeningMode.Instance.SelectedFunction = (OpeningFunction)openingShape;
            Main.Instance.SetMode(OpeningMode.Instance);
        }
    }
}