using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ButtonManagerModify : MonoBehaviour {

    public void Move()
    {
        ModifyMode.Instance.SetModifyMode(ModifyMode.ModifyModeType.Move);
        //main.modifyMode.selectedRoom.SetIsInMoveMode(isInMoveMode: true);
    }

    public void Rotate()
    {
        ModifyMode.Instance.SetModifyMode(ModifyMode.ModifyModeType.Rotate);
        //if (main.modifyMode.selectedRoom != null)
        //{
        //    main.modifyMode.selectedRoom.Rotate();
        //}
    }

    public void Modify()
    {
        ModifyMode.Instance.SetModifyMode(ModifyMode.ModifyModeType.Edit);
    }

    public void Properties() {

        
    }

    public void Delete()
    {
        ModifyMode.Instance.SetModifyMode(ModifyMode.ModifyModeType.Delete);
        //if (main.modifyMode.selectedRoom != null)
        //{
        //    main.modifyMode.selectedRoom.Delete();
        //}
    }
}
