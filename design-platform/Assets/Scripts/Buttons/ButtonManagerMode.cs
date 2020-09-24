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

    public void StartBuildMode(int buildShape)
    {
        main.buildMode.SetSelectedShape((RoomShape)buildShape);
        main.setMode(main.buildMode);
    }

}
