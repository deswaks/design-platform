using Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManagerHome : MonoBehaviour
{
    public void SaveLocal()
    {
        LocalDatabase.SaveAllUnityRoomsToJson();
    }

    public void LoadLocal()
    {
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
        Debug.Log("Export to IFC has not yet been implemented");
    }
}
