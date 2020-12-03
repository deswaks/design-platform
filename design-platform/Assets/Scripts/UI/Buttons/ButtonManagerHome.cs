using DesignPlatform.Database;
using DesignPlatform.Export;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// 
    /// </summary>
    public class ButtonManagerHome : MonoBehaviour {
        public void SaveLocal() {
            LocalDatabase.SaveAllUnitySpacesAndOpeningsToJson();
        }
        public void LoadLocal() {
            LocalDatabase.CreateAllUnitySpacesFromJson();
            LocalDatabase.CreateAllUnityOpeningsFromJson();
        }
        public void SaveToGraph() {
            GraphDatabase.Instance.PushAllUnitySpacesToGraph();
        }
        public void LoadFromGraph() {
            GraphDatabase.Instance.LoadAndBuildUnitySpacesFromGraph();
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