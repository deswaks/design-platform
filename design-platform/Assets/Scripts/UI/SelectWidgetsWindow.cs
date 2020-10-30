using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Dashboard {
    public class SelectWidgetsWindow : MonoBehaviour {

        GameObject rowTemplate;
        Dictionary<Widget, bool> widgets = new Dictionary<Widget, bool>();
        List<GameObject> widgetToggles = new List<GameObject>();

        // Sets up list of widgets and toggles them all
        void Start() {
            rowTemplate = gameObject.transform.Find("rowTemplate").gameObject;

            // Finds all widgets and puts them in dictionary
            widgets = Dashboard.Instance.widgets;

            foreach (var w in widgets) {
                GameObject currentRow = Instantiate(rowTemplate, rowTemplate.transform.parent);
                currentRow.name = w.Key.Name;
                currentRow.GetComponentInChildren<TMPro.TMP_Text>().text = w.Key.Name;
                Toggle toggle = currentRow.GetComponent<Toggle>();
                toggle.isOn = true;
                toggle.onValueChanged.AddListener(delegate { ToggleWidget(toggle); });
                widgetToggles.Add(currentRow);
            }

            Dashboard.Instance.SetAllWidgetsAndToggles(widgets);

            gameObject.SetActive(false);
        }

        public void ToggleWidget(Toggle toggle) {
            
            Widget widget = widgets.First(w => w.Key.Name == toggle.gameObject.name).Key;
            widgets[widget] = toggle.gameObject.GetComponent<Toggle>().isOn;
            
            Dashboard.Instance.SetAllWidgetsAndToggles(widgets);
        }
    }
}
