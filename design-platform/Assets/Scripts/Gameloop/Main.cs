using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public BuildMode buildMode;
    public ModifyMode modifyMode;
    public Mode currentMode;

    public GameObject buildingGameObject;
    public Building building;

    void Start()
    {
        building = buildingGameObject.GetComponent<Building>();
        buildMode = new BuildMode(this);
        modifyMode = new ModifyMode(this);
        setMode(modifyMode);
    }

    void Update()
    {
        currentMode.Tick();
    }

    public void setMode(Mode mode)
    {
        if (currentMode != null)
        {
            currentMode.OnModePause();
        }
        currentMode = mode;
        if (currentMode != null)
        {
            currentMode.OnModeResume();
        }
    }
}
