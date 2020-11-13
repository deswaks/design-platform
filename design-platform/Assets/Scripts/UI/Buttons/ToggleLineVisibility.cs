using UnityEngine;
using UnityEngine.UI;

namespace DesignPlatform.Core {

    public class ToggleLineVisibility : MonoBehaviour {
        public void ToggleVisibility(bool showLines) {
            
            GlobalSettings.ShowWallLines = showLines;
            GlobalSettings.ShowOpeningLines = showLines;
            foreach (Room room in Building.Instance.Rooms) {
                room.UpdateRender2D();
            }
            foreach (Opening opening in Building.Instance.openings) {
                opening.UpdateRender2D();
            }
        }

    }
}