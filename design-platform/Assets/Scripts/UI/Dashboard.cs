using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DesignPlatform.Core {

    public class Dashboard {
        private static Dashboard instance;
        public Dictionary<Widget, bool> widgets { get; private set; } = new Dictionary<Widget, bool>();

        private GameObject DashboardGameObject;
        private GameObject WidgetArea;
        private GameObject TemplateRow;

        private List<GameObject> widgetRows = new List<GameObject>();

        public Dashboard() {
            DashboardGameObject = GameObject.Find("DashboardWindow");
            WidgetArea = DashboardGameObject.transform.Find("ScrollArea").Find("WidgetArea").gameObject;
            TemplateRow = WidgetArea.transform.Find("TemplateRow").gameObject;
        }
        public static Dashboard Instance {
            get { return instance ?? (instance = new Dashboard()); }
        }

        public void InsertWidgets() {
            ClearAllWidgets();
            GameObject dualRow = null;
            TemplateRow.SetActive(true);

            foreach(var widgetKeyValue in widgets){
                if (!widgetKeyValue.Value) {
                    continue;
                }
                Widget widget = widgetKeyValue.Key;

                // If widget is full width, a new row is created and the widget is inserted into it
                if (widget.Size.width == 2) {
                    GameObject hostRow = GameObject.Instantiate( TemplateRow, WidgetArea.transform );
                    widgetRows.Add(hostRow);
                    widget.Draw(hostRow);
                    // Forces rebuild of layout to update position in layout groups
                    LayoutRebuilder.ForceRebuildLayoutImmediate(TemplateRow.transform as RectTransform);
                }

                else if (widget.Size.width == 1) {
                    // If no dualRow is available
                    if(dualRow == null) {
                        dualRow = GameObject.Instantiate(TemplateRow, WidgetArea.transform);
                        widgetRows.Add(dualRow);
                        widget.Draw(dualRow);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(dualRow.transform as RectTransform);

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
            widgets.Add(widget,true);
        }

        public void UpdateCurrentWidgets() {
            foreach(var widget in widgets) {
                if (!widget.Value) {
                    continue;
                }
                widget.Key.UpdatePanel();
            }
        }

        private void ClearAllWidgets() {
            foreach(GameObject row in widgetRows) {
                GameObject.Destroy(row);
            }
            widgetRows = new List<GameObject>();
        }

        internal void SetAllWidgetsAndToggles(Dictionary<Widget,bool> widgets) {
            this.widgets = widgets;

            ClearAllWidgets();
            InsertWidgets();
        }

    }
}

