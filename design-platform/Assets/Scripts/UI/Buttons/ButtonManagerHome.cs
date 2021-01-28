using DesignPlatform.Database;
using DesignPlatform.Core;
using DesignPlatform.Export;
using UnityEngine;
using TMPro;
using Michsky;

namespace DesignPlatform.UI {

    /// <summary>
    /// Contains functions that are called when pressing buttons in the Home menu tab
    /// </summary>
    public class ButtonManagerHome : MonoBehaviour {
        public void StartNewProject() {
            // Delete everything
            Building.Instance.DeleteEverything();
            // Set project info
            GameObject.Find("InputProjectName").GetComponent<TMP_InputField>().text = "";
            GameObject.Find("InputAuthor").GetComponent<TMP_InputField>().text = "";
            GameObject.Find("InputAddress").GetComponent<TMP_InputField>().text = "";
            GameObject.Find("InputZip").GetComponent<TMP_InputField>().text = "";
        }
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