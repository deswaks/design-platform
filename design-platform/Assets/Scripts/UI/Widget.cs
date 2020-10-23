using Microsoft.Isam.Esent.Interop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Widget
{
    public (int width, int height) Size;
    public GameObject Panel { get; private set; }

    public Widget(GameObject panel) {
        Panel = panel;
        GameObject prefabPanel = (GameObject)Resources.Load("Scripts/Modules/Structural Analysis/Widget/Widget");
        Panel = GameObject.Instantiate(prefabPanel, panel.transform.position, panel.transform.rotation);
        
    }

    public abstract void Update();


}
