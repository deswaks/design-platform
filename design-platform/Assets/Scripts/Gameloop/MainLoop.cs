using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoop : MonoBehaviour
{
    public BuildMode buildMode;
    public ModifyMode modifyMode;
    public Mode currentMode;

    public Building building = new Building();

    void Start()
    {
        buildMode = new BuildMode(this, building);
        modifyMode = new ModifyMode(this, building);
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
