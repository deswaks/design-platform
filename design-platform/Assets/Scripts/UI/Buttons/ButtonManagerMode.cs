using UnityEngine;

namespace DesignPlatform.Core {
    public class ButtonManagerMode : MonoBehaviour {
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
        public void StartOpeningMode(int openingShape) {
            OpeningMode.Instance.SetSelectedShape((OpeningShape)openingShape);
            Main.Instance.SetMode(OpeningMode.Instance);
        }
    }
}