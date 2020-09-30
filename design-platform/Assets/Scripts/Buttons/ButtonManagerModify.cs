using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerModify : MonoBehaviour
{
    public Main main;

    public void Move()
    {
        main.modifyMode.SetModifyMode(ModifyMode.ModifyModeTypes.Move);
    }
    public void Rotate()
    {
        main.modifyMode.SetModifyMode(ModifyMode.ModifyModeTypes.Rotate);
    }
    public void Modify()
    {
        main.modifyMode.SetModifyMode(ModifyMode.ModifyModeTypes.Edit);
    }
    public void Properties()
    {
        Debug.Log("Properties function is not implemented");
    }
    public void Delete()
    {
        main.modifyMode.SetModifyMode(ModifyMode.ModifyModeTypes.Delete);
    }
    public void Neo4jStuff() {
        //Neo4jClientSample.Program prog = new Neo4jClientSample.Program();
        Neo4jClientSample.Program.Muhmuh();
        //Vector3 a = new Vector3(1, 2, 3);

    }
}
