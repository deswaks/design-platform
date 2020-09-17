using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerModify : MonoBehaviour
{
    public MainLoop mainLoop;

    public void Move()
    {
        Debug.Log("Move function is not implemented");
    }
    public void Rotate()
    {
        mainLoop.modifyMode.selectedRoom.Rotate();
    }
    public void Modify()
    {
        Debug.Log("Modify function is not implemented");
    }
    public void Properties()
    {
        Debug.Log("Properties function is not implemented");
    }
    public void Delete()
    {
        mainLoop.modifyMode.selectedRoom.Delete();
    }
}
