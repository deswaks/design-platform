using DesignPlatform.UI;
using DesignPlatform.Utils;
using DesignPlatform.Core;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace StructuralAnalysis {

    /// <summary>
    /// The widget to display user information about the structural alanysis module
    /// </summary>
    public class StructuralWidget : Widget {

        /// <summary>The panel prefab wherein to insert data for the widget.</summary>
        private readonly GameObject PrefabPanel;

        /// <summary>The rows of data are created using this prefab,
        /// which is inserted into the widget afterwards.</summary>
        private readonly GameObject PrefabRow;

        /// <summary>Names of used GameObjects in prefab</summary>
        private static readonly string HeaderRowName = "Row_Headers";

        /// <summary></summary>
        private static readonly string ContentTemplateName = "Row_ContentTemplate";

        /// <summary></summary>
        private List<GameObject> addedRows = new List<GameObject>();

        GameObject Table;

        /// <summary>
        /// Constructor for this widget class
        /// </summary>
        public StructuralWidget() : base() {
            Name = "Structural Widget";
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("structuralanalysis", "Structural Widget");
            PrefabRow = AssetUtil.LoadAsset<GameObject>("structuralanalysis", "DataRow");
        }

        /// <summary>
        /// Returns a gameobject that visually represents this widget
        /// </summary>
        /// <returns>The panel that visually represents this widget.</returns>
        public override Object CreatePanel() {
            // Loads prefab object and instantiates Widget
            Panel = Object.Instantiate(PrefabPanel);

            Table = Panel.transform.GetChild(2).gameObject;

            return Panel;
        }

        /// <summary>
        /// Updates the gameobject that visually represents this widget
        /// </summary>
        public override void UpdatePanel() {

            // Sum elements
            float structuralWallMeters = 0;
            float nonStructuralWallMeters = 0;
            foreach (DesignPlatform.Core.Space space in Building.Instance.Spaces) {
                Dictionary<int, List<DistributedLoad>> loadTable = LoadDistributor.DistributeAreaLoads(space);
                List<int> structuralWallIndices = loadTable.Keys.ToList();
                
                // Sum the length of structural and non-structural wall elements
                for (int wallIndex = 0; wallIndex < space.ControlPoints.Count; wallIndex++) {
                    if (structuralWallIndices.Contains(wallIndex)) structuralWallMeters += space.Faces[wallIndex].LocationLine.Length;
                    else nonStructuralWallMeters += space.Faces[wallIndex].LocationLine.Length;
                }
            }

            // Build data rows
            DeleteContentRows();

            List<List<string>> dataRows = new List<List<string>> {
                new List<string> {"Structural elements", structuralWallMeters.ToString() + " m"},
                new List<string> {"Architectural elements", nonStructuralWallMeters.ToString() + " m" }
            };

            // Add data to widget
            int elementIndex = 0;

            for (int i = 0; i < dataRows.Count; i++) {
                // Create and add row to table
                GameObject dataRow = GameObject.Instantiate(PrefabRow, Table.transform);
                dataRow.name = "DataRow" + elementIndex.ToString();
                addedRows.Add(dataRow);

                // Fill row with values
                for (int j = 0; j < dataRow.transform.childCount; j++) {
                    dataRow.transform.GetChild(j).GetComponent<TMPro.TMP_Text>().text = dataRows[i][j];
                }
            }
        }

        /// <summary>
        /// Deletes the rows of information that wxists in the visual panel of this widget.
        /// </summary>
        private void DeleteContentRows() {
            foreach (GameObject row in addedRows) {
                GameObject.Destroy(row);
            }
            addedRows = new List<GameObject>();
        }
    }

}