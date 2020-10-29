using Database;
using UnityEngine;

public class ButtonManagerHome : MonoBehaviour {
    public void SaveLocal() {
        LocalDatabase.SaveAllUnityRoomsToJson();
    }
    public void LoadLocal() {
        LocalDatabase.CreateAllUnityRoomsFromJson();
    }
    public void SaveToGraph() {
        GraphDatabase.Instance.PushAllUnityRoomsToGraph();
    }
    public void LoadFromGraph() {
        GraphDatabase.Instance.LoadAndBuildUnityRoomsFromGraph();
    }
    public void ExportPDF() {
        PdfExport.ExportPlan();
    }
    public void ExportGbXML() {
        gbXML.Exporter.Export();
    }
    public void ExportIFC() {
        Ifc.Exporter.Export();
    }
}
