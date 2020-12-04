using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// 
    /// </summary>
    public class Widget_CircleDiagram : Widget {

        /// <summary>Names of used GameObjects in prefab</summary>
        private static readonly string TitleName = "txt_Title";
        /// <summary></summary>
        private static readonly string WedgeTemplateName = "wedge";
        /// <summary></summary>
        private static readonly string ListObjectTemplateName = "ListObject";
        /// <summary></summary>
        private GameObject PrefabPanel;
        /// <summary></summary>
        private GameObject titleObject;
        /// <summary>Content row template from prefab</summary>
        private GameObject wedgeTemplate;
        /// <summary>Content row template from prefab</summary>
        private GameObject listObjectTemplate;
        /// <summary></summary>
        private List<GameObject> wedges = new List<GameObject>();
        /// <summary></summary>
        private List<GameObject> listObjects = new List<GameObject>();
        /// <summary></summary>
        private Dictionary<string, float> diagramData = new Dictionary<string, float>();
        /// <summary></summary>
        private int elementsCount = 0;



        /// <summary>
        /// Constructor for this widget specifies its size and name.
        /// </summary>
        public Widget_CircleDiagram() : base() // Host skal ses som parent Dashboard-row(?)
        {
            Size = (width: 1, height: 1);
            Name = "Circle Diagram";
        }



        /// <summary>
        /// Returns a gameobject that visually represents this widget
        /// </summary>
        /// <returns>The panel that visually represents this widget.</returns>
        public override Object CreatePanel() // Initialize widget
        {
            // Loads prefab object and instantiates Widget
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("prefabs", "Widget_CircleDiagram");
            Panel = Object.Instantiate(PrefabPanel);//, parent.transform);

            // Locates primary components of widget
            titleObject = Panel.transform.Find(TitleName).gameObject;
            titleObject.GetComponent<TMPro.TMP_Text>().text = Name;

            wedgeTemplate = Panel.transform.Find("DiagramArea").Find("CircleDiagram").Find(WedgeTemplateName).gameObject;
            listObjectTemplate = Panel.transform.Find("DiagramArea").Find("List").Find(ListObjectTemplateName).gameObject;

            //UpdatePanel();

            return Panel;
        }

        /// <summary>
        /// Updates the gameobject that visually represents this widget
        /// </summary>
        public override void UpdatePanel() {
            listObjectTemplate.SetActive(true);
            wedgeTemplate.SetActive(true);

            ClearDiagram();

            // SAMPLE DATA
            diagramData.Add("Element 1", 0.21f);
            diagramData.Add("Element 2", 0.10f);
            diagramData.Add("Element 3", 0.05f);
            diagramData.Add("Element 4", 0.13f);
            diagramData.Add("Element 5", 0.08f);
            diagramData.Add("Element 6", 0.13f);
            diagramData.Add("Element 7", 0.17f);
            diagramData.Add("Element 8", 0.13f);

            elementsCount = diagramData.Count;

            float currentAngle = 0;
            int i = 0;

            foreach (var element in diagramData) {
                GameObject currentWedge = Object.Instantiate(wedgeTemplate, wedgeTemplate.transform.parent);
                GameObject currentListObject = Object.Instantiate(listObjectTemplate, listObjectTemplate.transform.parent);

                // Writes list element
                currentListObject.GetComponentInChildren<TMPro.TMP_Text>().text = element.Key + " (" + (element.Value * 100).ToString() + "%)";

                // Sets wedge fill amount and angle
                currentWedge.GetComponent<UnityEngine.UI.Image>().fillAmount = element.Value;
                currentWedge.transform.Rotate(Vector3.forward, currentAngle);
                currentAngle -= element.Value * 360;


                // Sets color of element in both diagram and list
                Color color = new Color(Random.value, Random.value, Random.value);                       // RANDOM COLOUR
                                                                                                         //Color pink = new Color(129f / 256f,   0f / 256f, 212f / 256f);
                                                                                                         //Color blue = new Color( 14f / 256f, 210f / 256f,  23f / 256f);

                currentWedge.GetComponent<UnityEngine.UI.Image>().color = color;
                currentListObject.GetComponentInChildren<UnityEngine.UI.Image>().color = color;

                wedges.Add(currentWedge);
                listObjects.Add(currentListObject);

                i++;
            }

            listObjectTemplate.SetActive(false);
            wedgeTemplate.SetActive(false);
        }

        /// <summary>
        /// Clears the data currently displayed in the diagram.
        /// </summary>
        private void ClearDiagram() {
            foreach (GameObject wedge in wedges) {
                Object.Destroy(wedge);
            }
            foreach (GameObject listObject in listObjects) {
                Object.Destroy(listObject);
            }
            wedges = new List<GameObject>();
            listObjects = new List<GameObject>();
            diagramData = null;
            diagramData = new Dictionary<string, float>();
        }
    }
}