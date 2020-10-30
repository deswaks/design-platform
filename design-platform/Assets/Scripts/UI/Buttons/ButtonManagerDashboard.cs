using UnityEngine;

namespace DesignPlatform.Core {
    public class ButtonManagerDashboard : MonoBehaviour {

        public void PopulateDashboard() {
            Dashboard.Instance.InsertWidgets();
        }

    public void RefreshDashboard() {
        Dashboard.Dashboard.Instance.UpdateCurrentWidgets();
    }

}
