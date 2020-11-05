using DesignPlatform.Core;
using DesignPlatform.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DesignPlatform {
    public class RoomScheduleWidget : Widget {
        // Names of used GameObjects in prefab
        private static readonly string HeaderRowName = "Row_Headers";
        private static readonly string ContentTemplateName = "Row_ContentTemplate";
        private GameObject PrefabPanel;

        GameObject headerRow;   // 
        GameObject contentTemplate; // Content row template from prefab
        List<GameObject> contentRows = new List<GameObject>();

        private int columnCount = 0;

        public RoomScheduleWidget() : base() {
            Size = (width: 2, height: 1);
            Name = "Room Schedule";
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
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("prefabs", "Widget_RoomSchedule");
            Panel = GameObject.Instantiate(PrefabPanel); //, parent.transform).transform.GetChild(0).gameObject;

            // Locates primary components of widget
            headerRow = Panel.transform.Find("Widget_RoomSchedule").Find(HeaderRowName).gameObject;
            contentTemplate = Panel.transform.Find("Widget_RoomSchedule").Find(ContentTemplateName).gameObject;

            columnCount = contentTemplate.transform.childCount;

            //UpdatePanel();

            return Panel;
        }

        public override void UpdatePanel() {
            contentTemplate.SetActive(true);

            DeleteContentRows();
            int roomIndex = 0;

            foreach (Room room in Building.Instance.rooms) {
                GameObject currentRow = GameObject.Instantiate(contentTemplate, contentTemplate.transform.parent);
                currentRow.name = "Row" + roomIndex.ToString();
                contentRows.Add(currentRow);

                List<string> rowData = new List<string>
                {
                "Room " + roomIndex.ToString(),                                             // Room Name
                StringUtils.StringUtils.ToTitleCase(room.Type.ToString()),              // Room Type
                StringUtils.StringUtils.ToTitleCase(room.Shape.ToString()),        // Room Shape
                room.gameObject.GetInstanceID().ToString(),                                 // Room rumber - SKAL OPDATERES!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                room.GetFloorArea().ToString() + " m²"                                      // Floor area

            };

                for (int i = 0; i < columnCount; i++) {
                    currentRow.transform.GetChild(i).GetComponentInChildren<TMPro.TMP_Text>().text = rowData[i];
                }

                roomIndex++;

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