using DesignPlatform.Geometry;
using UnityEngine;
using UnityEngine.UI;

namespace DesignPlatform.UI {

    /// <summary>
    /// Base class for the dashboard widgets.
    /// Contains methods to create and update the visual dahboard panel for the widget.
    /// </summary>
    public abstract class Widget {

        /// <summary></summary>
        public (int width, int height) Size;

        /// <summary>The visual panel game object of this widget.</summary>
        public GameObject Panel;

        /// <summary>The host panel wherein the panel of this widget is inserted.</summary>
        public GameObject Host;

        /// <summary>The name of this widget.</summary>
        public string Name;

        /// <summary>
        /// Main constructor can be called from all derived constructors
        /// if default settings are needed in terms of size and name.
        /// </summary>
        public Widget() {
            Size = (1, 1);
            Name = "Widget";
        }

        /// <summary>
        /// Returns a gameobject that visually represents this widget
        /// </summary>
        /// <returns>The panel that visually represents this widget.</returns>
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
            Panel.transform.SetParent(host.transform, false);
            //Panel.transform.parent = host.transform;
            Panel.transform.localPosition = Vector3.zero;
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

        /// <summary>
        /// All a layout component to the widget such that it can be placed correctly into the dahboard.
        /// </summary>
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