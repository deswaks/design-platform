using UnityEngine;
using UnityEngine.UI;

namespace DesignPlatform.Core {

    public class ToggleLineVisibility : MonoBehaviour {
        public void ToggleVisibility(bool showLines) {
            
            GlobalSettings.ShowWallLines = showLines;
            GlobalSettings.ShowOpeningLines = showLines;
            foreach (Room room in Building.Instance.rooms) {
                room.UpdateRender2D();
            }
            foreach (Opening opening in Building.Instance.Openings) {
                opening.UpdateRender2D();
            }
        }

    }
}