using DesignPlatform.Core;
using DesignPlatform.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.UI {
    public class SpaceScheduleWidget : Widget {
        // Names of used GameObjects in prefab
        private static readonly string HeaderRowName = "Row_Headers";
        private static readonly string ContentTemplateName = "Row_ContentTemplate";
        private GameObject PrefabPanel;

        GameObject headerRow;   // 
        GameObject contentTemplate; // Content row template from prefab
        List<GameObject> contentRows = new List<GameObject>();

        private int columnCount = 0;

        public SpaceScheduleWidget() : base() {
            Size = (width: 2, height: 1);
            Name = "Space Schedule";
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
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("prefabs", "Widget_SpaceSchedule");
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
            int spaceIndex = 0;

            foreach (Core.Space space in Building.Instance.Spaces) {
                GameObject currentRow = GameObject.Instantiate(contentTemplate, contentTemplate.transform.parent);
                currentRow.name = "Row" + spaceIndex.ToString();
                contentRows.Add(currentRow);

                List<string> rowData = new List<string>
                {
                "Space " + spaceIndex.ToString(),                 // Space Name
                StringUtils.ToTitleCase(space.Function.ToString()),  // Space Type
                StringUtils.ToTitleCase(space.Shape.ToString()), // Space Shape
                space.gameObject.GetInstanceID().ToString(),     // Space rumber - SKAL OPDATERES!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                space.Area.ToString() + " m²"          // Floor area

            };

                for (int i = 0; i < columnCount; i++) {
                    currentRow.transform.GetChild(i).GetComponentInChildren<TMPro.TMP_Text>().text = rowData[i];
                }

                spaceIndex++;

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