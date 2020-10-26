using Microsoft.Isam.Esent.Interop;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class Widget {
    public (int width, int height) Size;
    public GameObject PrefabPanel;
    public GameObject Panel;
    public GameObject Host;

    public Widget(GameObject host){
        Host = host;
    }
    public abstract void InsertInDashboard();
    public abstract GameObject DrawPanel();
    public abstract void UpdatePanel();
    public void DeletePanel() {
        Object.Destroy(Panel);
    }
}
