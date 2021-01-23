using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DesignPlatform.UI {

    /// <summary>
    /// Contains functions for controlling the 'Select Widgets' window
    /// </summary>
    public class SelectWidgetsWindow : MonoBehaviour {

        GameObject rowTemplate = null;
        Dictionary<Widget, bool> widgets = new Dictionary<Widget, bool>();
        List<GameObject> widgetToggles = new List<GameObject>();
        /// <summary>
        /// 
        /// </summary>
        void Start() {
            rowTemplate = gameObject.transform.Find("rowTemplate").gameObject;
            gameObject.SetActive(false);

        }

        
        /// <summary>
        /// Sets up list of widgets and toggles them all
        /// </summary>
        void OnEnable() {
            if (rowTemplate == null) {
                rowTemplate = gameObject.transform.Find("rowTemplate").gameObject;
            }
            rowTemplate.SetActive(true);

            // Finds all widgets and puts them in dictionary
            widgets = Dashboard.Instance.widgets;

            widgetToggles.ForEach(t => Destroy(t));
            widgetToggles = new List<GameObject>();


            foreach (var w in widgets) {
                GameObject currentRow = Instantiate(rowTemplate, rowTemplate.transform.parent);
                currentRow.name = w.Key.Name;
                currentRow.GetComponentInChildren<TMPro.TMP_Text>().text = w.Key.Name;
                Toggle toggle = currentRow.GetComponent<Toggle>();
                toggle.isOn = w.Value;
                toggle.onValueChanged.AddListener(delegate { ToggleWidget(toggle); });
                widgetToggles.Add(currentRow);
            }

            //Dashboard.Instance.SetAllWidgetsAndToggles(widgets);

            rowTemplate.SetActive(false);

        }

        /// <summary>
        /// Function that enables the toggling Widgets (switching them on/off)
        /// </summary>
        /// <param name="toggle"></param>
        public void ToggleWidget(Toggle toggle) {

            Widget widget = widgets.First(w => w.Key.Name == toggle.gameObject.name).Key;
            widgets[widget] = toggle.gameObject.GetComponent<Toggle>().isOn;

            Dashboard.Instance.SetAllWidgetsAndToggles(widgets);
        }
    }
}
