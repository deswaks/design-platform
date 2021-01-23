using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DesignPlatform.Core;
using DesignPlatform.Database;

namespace DesignPlatform.UI {

    /// <summary>
    /// Class for handling line (wall) visibility on 2D Canvas
    /// </summary>
    public class ToggleLineVisibility : MonoBehaviour {
        /// <summary>
        /// Toggles line (wall) visibility on 2D Canvas
        /// </summary>
        /// <param name="showLines"></param>
        public void ToggleVisibility(bool showLines) {


            Settings.ShowWallLines = showLines;
            Settings.ShowOpeningLines = showLines;
            foreach (Core.Space room in Building.Instance.Spaces) {
                room.UpdateRender2D();
            }
            foreach (Opening opening in Building.Instance.Openings) {
                opening.UpdateRender2D();
            }
        }

    }
}