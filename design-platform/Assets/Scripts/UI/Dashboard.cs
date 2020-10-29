using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dashboard {


    public class Dashboard {
        public static Dashboard instance;

        private List<Widget> widgets = new List<Widget>();

        public GameObject WidgetArea;
        public GameObject RowTemplate;

        public static Dashboard Instance {
            get { return instance ?? (instance = new Dashboard()); }
        }

        public void InsertWidgets() {
            
            foreach(Widget widget in widgets){






                
                widget.Draw(host);
                
            }
        }

        public void AddWidgetToList(Widget widget) {
            widgets.Add(widget);
        }

        public void UpdateCurrentWidgets() {

        }

    }
}

