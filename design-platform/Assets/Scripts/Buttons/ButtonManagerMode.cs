using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerMode : MonoBehaviour
{
    public Main main;

    public void StartModifyMode()
    {
        main.setMode(main.modifyMode);
    }

    public void StartBuildMode(int buildingShape)
    {
        main.buildMode.SetSelectedShape(buildingShape);
        main.setMode(main.buildMode);
    }

}
