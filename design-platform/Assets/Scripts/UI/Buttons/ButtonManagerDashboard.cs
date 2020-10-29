using UnityEngine;

namespace DesignPlatform.Core {
    public class ButtonManagerDashboard : MonoBehaviour {

        public void PopulateDashboard() {
            Dashboard.Instance.InsertWidgets();
        }

    }
}