﻿using DesignPlatform.Core;
using DesignPlatform.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Name = "CLT Element Schedule";
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

            return Panel;
        }

        public override void UpdatePanel() {
            contentTemplate.SetActive(true);

            DeleteContentRows();
            int elementIndex = 0;

            List<CLTElement> wallElements = Building.Instance.CLTElements;
            List<List<string>> allDataRows = new List<List<string>>();

            //foreach (CLTElement e in wallElements) {
            wallElements.ForEach(e =>allDataRows.Add( new List<string>{
                    "", // For count
                    e.Quality.ToString(),    
                    e.Length.ToString()+" m",
                    e.Height.ToString()+" m",
                    (e.Thickness*1000).ToString()+" mm",
                    e.Area.ToString()+" m²"
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
                currentRow.transform.GetChild(0).GetComponentInChildren<TMPro.TMP_Text>().text = count.ToString();
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