using DesignPlatform.Core;
using DesignPlatform.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DesignPlatform.UI {
    public class WallElementScheduleWidget : Widget {
        // Names of used GameObjects in prefab
        private static readonly string HeaderRowName = "Row_Headers";
        private static readonly string ContentTemplateName = "Row_ContentTemplate";
        private GameObject PrefabPanel;

        GameObject headerRow;   // 
        GameObject contentTemplate; // Content row template from prefab
        List<GameObject> contentRows = new List<GameObject>();

        private int columnCount = 0;

        public WallElementScheduleWidget() : base() {
            Size = (width: 2, height: 1);
            Name = "Wall Element Schedule";
        }

        public void InsertInDashboard() {

        }

        public GameObject DrawPanel() // Initialize widget
        {
            return Host;
        }

        public override Object CreatePanel() // Initialize widget
        {
            // Loads prefab object and instantiates Widget
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("prefabs", "Widget_WallElementSchedule");
            Panel = GameObject.Instantiate(PrefabPanel); //, parent.transform).transform.GetChild(0).gameObject;

            // Locates primary components of widget
            headerRow = Panel.transform.Find(HeaderRowName).gameObject;
            contentTemplate = Panel.transform.Find(ContentTemplateName).gameObject;

            columnCount = contentTemplate.transform.childCount;

            //UpdatePanel();

            return Panel;
        }

        public override void UpdatePanel() {
            contentTemplate.SetActive(true);

            DeleteContentRows();
            int elementIndex = 0;

            List<CLTElement> wallElements = Building.IdentifyWallElementsAndJointTypes();

            foreach (CLTElement e in wallElements) {
                GameObject currentRow = GameObject.Instantiate(contentTemplate, contentTemplate.transform.parent);
                currentRow.name = "Row" + elementIndex.ToString();
                contentRows.Add(currentRow);

                List<string> rowData = new List<string>
                {
                "Wall " + elementIndex.ToString(),
                e.Length.ToString()+"m",
                e.Quality,    
                e.Height.ToString()+"m",
                (e.Thickness*1000).ToString()+"mm",
                e.Area.ToString()+"m²"
                };

                for (int i = 0; i < columnCount; i++) {
                    currentRow.transform.GetChild(i).GetComponentInChildren<TMPro.TMP_Text>().text = rowData[i];
                }

                elementIndex++;
            }
            contentTemplate.SetActive(false);

        }

        private void DeleteContentRows() {
            foreach (GameObject row in contentRows) {
                GameObject.Destroy(row);
            }
            contentRows = new List<GameObject>();

        }

    }
}