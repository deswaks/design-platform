using UnityEngine;

namespace DesignPlatform.UI {
    public class ButtonManagerDashboard : MonoBehaviour {

        public void PopulateDashboard() {
            Dashboard.Instance.InsertWidgets();
        }

        public void RefreshDashboard() {
            Dashboard.Instance.UpdateCurrentWidgets();
        }

    }
}
