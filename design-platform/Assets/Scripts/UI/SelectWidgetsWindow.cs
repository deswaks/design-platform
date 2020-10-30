﻿using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Dashboard {
    public class SelectWidgetsWindow : MonoBehaviour {

        GameObject rowTemplate;
        Dictionary<Widget, bool> widgets = new Dictionary<Widget, bool>();
        List<GameObject> widgetToggles = new List<GameObject>();

        void Start() {
            rowTemplate = gameObject.transform.Find("rowTemplate").gameObject;
            rowTemplate.SetActive(false);
            gameObject.SetActive(false);

        }

        // Sets up list of widgets and toggles them all
        void OnEnable() {

            // Finds all widgets and puts them in dictionary
            widgets = Dashboard.Instance.widgets;

            widgetToggles.ForEach(t => GameObject.Destroy(t));
            widgetToggles = new List<GameObject>();

            rowTemplate.SetActive(true);

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

        public void ToggleWidget(Toggle toggle) {
            
            Widget widget = widgets.First(w => w.Key.Name == toggle.gameObject.name).Key;
            widgets[widget] = toggle.gameObject.GetComponent<Toggle>().isOn;
            
            Dashboard.Instance.SetAllWidgetsAndToggles(widgets);
        }
    }
}
