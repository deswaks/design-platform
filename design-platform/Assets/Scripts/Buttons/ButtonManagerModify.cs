using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerModify : MonoBehaviour
{
    public Main main;

    public void Move()
    {
        Debug.Log("Move function is not implemented");
    }
    public void Rotate()
    {
        if (main.modifyMode.selectedRoom != null)
        {
            main.modifyMode.selectedRoom.Rotate();
        }
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
        if (main.modifyMode.selectedRoom != null)
        {
            main.modifyMode.selectedRoom.Delete();
        }
    }
}
