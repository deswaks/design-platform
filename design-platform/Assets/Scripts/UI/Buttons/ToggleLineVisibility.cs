using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DesignPlatform.Core;
using DesignPlatform.Database;

namespace DesignPlatform.UI {

    public class ToggleLineVisibility : MonoBehaviour {
        public void ToggleVisibility(bool showLines) {


            Settings.ShowWallLines = showLines;
            Settings.ShowOpeningLines = showLines;
            foreach (Room room in Building.Instance.Rooms) {
                room.UpdateRender2D();
            }
            foreach (Opening opening in Building.Instance.Openings) {
                opening.UpdateRender2D();
            }
        }

    }
}