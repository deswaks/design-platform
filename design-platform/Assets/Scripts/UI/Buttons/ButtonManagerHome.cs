using DesignPlatform.Database;
using DesignPlatform.Export;
using UnityEngine;

namespace DesignPlatform.UI {
    public class ButtonManagerHome : MonoBehaviour {
        public void SaveLocal() {
            LocalDatabase.SaveAllUnityRoomsAndOpeningsToJson();
        }
        public void LoadLocal() {
            LocalDatabase.CreateAllUnityRoomsFromJson();
            LocalDatabase.CreateAllUnityOpeningsFromJson();
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