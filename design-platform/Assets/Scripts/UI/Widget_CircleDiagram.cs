using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Widget_CircleDiagram : Widget
{
    // Names of used GameObjects in prefab
    private static readonly string TitleName = "txt_Title";
    private static readonly string WedgeTemplateName = "wedge";
    private static readonly string ListObjectTemplateName = "ListObject";
    private static readonly string PrefabPath = "Assets/Prefabs/Widgets/Widget_CircleDiagram.prefab";
    private GameObject PrefabPanel;

    GameObject titleObject;   // 
    GameObject wedgeTemplate; // Content row template from prefab
    GameObject listObjectTemplate; // Content row template from prefab
    List<GameObject> wedges = new List<GameObject>();
    List<GameObject> listObjects = new List<GameObject>();

    Dictionary<string, float> diagramData = new Dictionary<string, float>();
    

    private int elementsCount = 0;

    public Widget_CircleDiagram()  : base() // Host skal ses som parent Dashboard-row(?)
    {
        Size = (width: 1, height: 1);
        Name = "Circle Diagram";
    }

    public void InsertInDashboard()
    {

    }

    public GameObject DrawPanel() // Initialize widget
    {
        return Host;
    }

    public override Object CreatePanel() // Initialize widget
    {
        // Loads prefab object and instantiates Widget
        PrefabPanel = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        Panel = (GameObject)GameObject.Instantiate(PrefabPanel);//, parent.transform);

        // Locates primary components of widget
        titleObject = Panel.transform.Find(TitleName).gameObject;
        titleObject.GetComponent<TMPro.TMP_Text>().text = Name;

        wedgeTemplate = Panel.transform.Find("DiagramArea").Find("CircleDiagram").Find(WedgeTemplateName).gameObject;
        listObjectTemplate = Panel.transform.Find("DiagramArea").Find("List").Find(ListObjectTemplateName).gameObject;

        //UpdatePanel();

        return Panel;
    }

    public override void UpdatePanel()
    {
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

        foreach(var element in diagramData)
        {
            GameObject currentWedge = GameObject.Instantiate(wedgeTemplate, wedgeTemplate.transform.parent);
            GameObject currentListObject = GameObject.Instantiate(listObjectTemplate, listObjectTemplate.transform.parent);

            // Writes list element
            currentListObject.GetComponentInChildren<TMPro.TMP_Text>().text = element.Key + " (" + (element.Value*100).ToString()+"%)";

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

private void ClearDiagram()
    {
        foreach (GameObject wedge in wedges)
        {
            GameObject.Destroy(wedge);
        }        
        foreach (GameObject listObject in listObjects)
        {
            GameObject.Destroy(listObject);
        }
        wedges = new List<GameObject>();
        listObjects = new List<GameObject>();
        diagramData = null;
        diagramData = new Dictionary<string, float>();
    }
}
