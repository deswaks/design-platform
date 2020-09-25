using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerMode : MonoBehaviour
{
    public Main main;

    public void StartModifyMode()
    {
        Main.Instance.setMode(ModifyMode.Instance);
    }

    public void StartBuildMode(int buildShape)
    {
        BuildMode.Instance.SetSelectedShape((RoomShape)buildShape);
        Main.Instance.setMode(BuildMode.Instance);
    }

}
