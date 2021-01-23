using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// Contains functions that are called when pressing buttons in the Dashboard
    /// </summary>
    public class ButtonManagerDashboard : MonoBehaviour {
        /// <summary>
        /// Inserts all currently selected and available Widgets onto the Dashboard
        /// </summary>
        public void PopulateDashboard() {
            Dashboard.Instance.InsertWidgets();
        }
        /// <summary>
        /// Prompts all widgets to update their content
        /// </summary>
        public void RefreshDashboard() {
            Dashboard.Instance.UpdateCurrentWidgets();
        }

    }
}
