using DesignPlatform.Core;
using DesignPlatform.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// 
    /// </summary>
    public class OpeningsScheduleWidget : Widget {

        /// <summary>Names of used GameObjects in prefab</summary>
        private static readonly string HeaderRowName = "Row_Headers";
        /// <summary></summary>
        private static readonly string ContentTemplateName = "Row_ContentTemplate";
        /// <summary></summary>
        private GameObject PrefabPanel;
        /// <summary></summary>
        private GameObject headerRow;
        /// <summary>Content row template from prefab</summary>
        private GameObject contentTemplate;
        /// <summary></summary>
        private List<GameObject> contentRows = new List<GameObject>();
        /// <summary></summary>
        private int columnCount = 0;



        /// <summary>
        /// Constructor for this widget specifies its size and name.
        /// </summary>
        public OpeningsScheduleWidget() : base() {
            Size = (width: 2, height: 1);
            Name = "Openings Schedule";
        }



        /// <summary>
        /// Returns a gameobject that visually represents this widget
        /// </summary>
        /// <returns>The panel that visually represents this widget.</returns>
        public override Object CreatePanel() // Initialize widget
        {
            // Loads prefab object and instantiates Widget
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("prefabs", "Widget_OpeningsSchedule");
            Panel = GameObject.Instantiate(PrefabPanel); 

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
            int elementIndex = 0;

            List<Opening> openings = Building.Instance.Openings;
            List<List<string>> allDataRows = new List<List<string>>();

            //foreach (CLTElement e in wallElements) {
            openings.ForEach(o =>allDataRows.Add( new List<string>{
                     StringUtils.ToTitleCase(o.Function.ToString()),    
                    (o.Height*1000).ToString()+" mm",
                    (o.Width*1000).ToString()+" mm",
                    "", // For count
            }));

            List<List<string>> addedRows = new List<List<string>>();

            foreach (List<string> rowData in allDataRows) {
                // Checks if an equal row has already been added
                if (addedRows.Count(row => row.SequenceEqual(rowData)) > 0) continue;

                addedRows.Add(rowData);

                GameObject currentRow = GameObject.Instantiate(contentTemplate, contentTemplate.transform.parent);
                currentRow.name = "Row" + elementIndex.ToString();
                contentRows.Add(currentRow);
                elementIndex++;

                for (int i = 0; i < columnCount; i++) {
                    currentRow.transform.GetChild(i).GetComponentInChildren<TMPro.TMP_Text>().text = rowData[i];
                }
                int count = allDataRows.Count(row => row.SequenceEqual(rowData));
                currentRow.transform.GetChild(columnCount-1).GetComponentInChildren<TMPro.TMP_Text>().text = count.ToString();
            }

            contentTemplate.SetActive(false);
        }

        /// <summary>
        /// Deletes the rows of information that wxists in the visual panel of this widget.
        /// </summary>
        private void DeleteContentRows() {
            foreach (GameObject row in contentRows) {
                GameObject.Destroy(row);
            }
            contentRows = new List<GameObject>();

        }

    }
}