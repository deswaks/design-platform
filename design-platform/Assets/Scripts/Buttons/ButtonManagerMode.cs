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

    // Starts building mode with the desired room shape 
    public void StartBuildMode(int buildingShape)
    {
        mainLoop.buildMode.SetSelectedShape(buildingShape);
        mainLoop.setMode(mainLoop.buildMode);
    }

}
