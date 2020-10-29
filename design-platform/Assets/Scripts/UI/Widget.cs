using Boo.Lang;
using Microsoft.Isam.Esent.Interop;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace DesignPlatform.Core {
    public abstract class Widget {
        public (int width, int height) Size;
        public GameObject Panel;
        public GameObject Host;
        public string Name;


        public Widget() {
            Size = (1, 1);
            Name = "Widget";
        }

        /// <summary>
        /// Returns a gameobject that visually represents this widget
        /// </summary>
        /// <returns></returns>
        public abstract Object CreatePanel();

        /// <summary>
        /// Updates the gameobject that visually represents this widget
        /// </summary>
        public abstract void UpdatePanel();

        /// <summary>
        /// Asks the dashboard to draw this widget
        /// </summary>
        public void RequestDraw() {
            try {
                Dashboard.Instance.AddWidgetToList(this);
            }
            catch {
                Debug.Log("Failed to find dashboard when adding widget.");
            }
        }

        /// <summary>
        /// Creates a panel for the widget
        /// </summary>
        public void Draw(GameObject host) {
            Host = host;
            if (Panel != null) Delete();
            Panel = (GameObject)CreatePanel();
            Panel.transform.parent = host.transform;
            Panel.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            Panel.transform.localRotation = Quaternion.identity;
            AddLayoutElementComponent();
            UpdatePanel();
        }

        /// <summary>
        /// Deletes the widget panel but not the widget itself so it can be drawn again
        /// </summary>
        public void Delete() {
            Object.Destroy(Panel);
        }

        public void AddLayoutElementComponent() {
            if (Panel.GetComponent<LayoutElement>() == null) {
                LayoutElement layoutElement = Panel.AddComponent<LayoutElement>();
                layoutElement.minHeight = Size.height * 500;
                layoutElement.minWidth = Size.width * 500;
                layoutElement.preferredHeight = Size.height * 500;
                layoutElement.preferredWidth = Size.width * 500;
            }
        }
    }
}