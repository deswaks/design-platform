using DesignPlatform.Database;
using DesignPlatform.Export;
using DesignPlatform.PdfExport;
using UnityEngine;

namespace DesignPlatform.Core {
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
            PdfExporter.ExportPlan();
        }
        public void ExportGbXML() {
            GbxmlExporter.Export();
        }
        public void ExportIFC() {
            IfcExporter.Export();
        }
        public void ExportWallsForRevit() {
            //LocalDatabase.SaveAllUnityInterfacesToJson();
            LocalDatabase.SaveAllWallElementsToJson();
        }
    }
}