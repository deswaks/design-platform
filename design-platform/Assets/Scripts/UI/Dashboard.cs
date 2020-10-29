using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Dashboard {


    public class Dashboard {
        public static Dashboard instance;
        
        private List<Widget> widgets = new List<Widget>();

        private GameObject DashboardGameObject;
        private GameObject WidgetArea;
        private GameObject TemplateRow;

        public Dashboard() {
            DashboardGameObject = GameObject.Find("DashboardWindow");
            WidgetArea = DashboardGameObject.transform.Find("ScrollArea").Find("WidgetArea").gameObject;
            TemplateRow = WidgetArea.transform.Find("TemplateRow").gameObject;
        }
        public static Dashboard Instance {
            get { return instance ?? (instance = new Dashboard()); }
        }

        public void InsertWidgets() {
            GameObject dualRow = null;
            TemplateRow.SetActive(true);

            foreach (Widget widget in widgets){

                // If widget is full width, a new row is created and the widget is inserted into it
                if (widget.Size.width == 2) {
                    GameObject hostRow = GameObject.Instantiate( TemplateRow, WidgetArea.transform );
                    widget.Draw(hostRow);
                    // Forces rebuild of layout to update position in layout groups
                    LayoutRebuilder.ForceRebuildLayoutImmediate(TemplateRow.transform as RectTransform);
                }

                else if (widget.Size.width == 1) {
                    // If no dualRow is available
                    if(dualRow == null) {
                        dualRow = GameObject.Instantiate(TemplateRow, WidgetArea.transform);
                        widget.Draw(dualRow);
                    }
                    // If a dualRow already exists (meaning one place remains)
                    else {
                        widget.Draw(dualRow);
                        // Forces rebuild of layout to update position in layout groups
                        LayoutRebuilder.ForceRebuildLayoutImmediate(dualRow.transform as RectTransform);
                        dualRow = null;
                    }
                }
            }
            TemplateRow.SetActive(false);
        }

        public void AddWidgetToList(Widget widget) {
            widgets.Add(widget);
        }

        public void UpdateCurrentWidgets() {

        }

    }
}

