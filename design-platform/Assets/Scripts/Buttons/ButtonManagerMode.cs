using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerMode : MonoBehaviour
{
    public MainLoop mainLoop;

    public void StartModifyMode()
    {
        mainLoop.setMode(mainLoop.modifyMode);

    }

    public void StartBuildMode(int buildingShape)
    {
        mainLoop.setMode(mainLoop.buildMode);
        mainLoop.buildMode.SetSelectedShape(buildingShape);

    }

}
