using UnityEngine;

public class ButtonManagerDashboard : MonoBehaviour {

    public void PopulateDashboard() {
        Dashboard.Dashboard.Instance.InsertWidgets();
    }

}
