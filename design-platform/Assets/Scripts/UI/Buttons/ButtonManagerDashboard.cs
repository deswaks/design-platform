using UnityEngine;

public class ButtonManagerDashboard : MonoBehaviour {

    public void PopulateDashboard() {
        Dashboard.Dashboard.Instance.InsertWidgets();
    }

    public void RefreshDashboard() {
        Dashboard.Dashboard.Instance.UpdateCurrentWidgets();
    }

}
