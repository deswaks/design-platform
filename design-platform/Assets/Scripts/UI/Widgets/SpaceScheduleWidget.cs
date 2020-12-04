using DesignPlatform.Core;
using DesignPlatform.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// Returns a gameobject that visually represents this widget
        /// </summary>
        /// <returns>The panel that visually represents this widget.</returns>
        public override Object CreatePanel() // Initialize widget
        {
            // Loads prefab object and instantiates Widget
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("prefabs", "Widget_SpaceSchedule");
            Panel = GameObject.Instantiate(PrefabPanel); //, parent.transform).transform.GetChild(0).gameObject;

            // Locates primary components of widget
            headerRow = Panel.transform.Find(HeaderRowName).gameObject;
            contentTemplate = Panel.transform.Find(ContentTemplateName).gameObject;

            columnCount = contentTemplate.transform.childCount;

            return Panel;
        }

        /// <summary>
        /// Updates the gameobject that visually represents this widget
        /// </summary>
        public override void UpdatePanel() {
            contentTemplate.SetActive(true);

            DeleteContentRows();
            int spaceIndex = 0;

            foreach (Core.Space space in Building.Instance.Spaces) {
                GameObject currentRow = GameObject.Instantiate(contentTemplate, contentTemplate.transform.parent);
                currentRow.name = "Row" + spaceIndex.ToString();
                contentRows.Add(currentRow);

                List<string> rowData = new List<string>{
                    $"{spaceIndex+1}",
                    $"{Core.Settings.SpaceTypeNames[space.Function].Replace("\n"," ")}",
                    $"{Core.Settings.SpaceShapeNames[space.Shape].Replace("\n"," ")}",
                    $"{space.Area} m²",
                    $"{space.CustomNote}",
                    
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