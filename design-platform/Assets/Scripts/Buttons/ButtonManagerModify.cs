using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ButtonManagerModify : MonoBehaviour {
    public Main main;
    // Tilføjet ABC
    private List<int> verticalFace;
    private List<int> baseFace;
    private List<Vector3> baseFaceCorners;
    private Vector3 facePos;
    //public GameObject moveHandlePrefab;
    //private GameObject moveHandle;
    //private Room prefabRoom;

    public void Move()
    {
        ModifyMode.Instance.SetModifyMode(ModifyMode.ModifyModeTypes.Move);
        //main.modifyMode.selectedRoom.SetIsInMoveMode(isInMoveMode: true);
    }
    public void Rotate()
    {
        ModifyMode.Instance.SetModifyMode(ModifyMode.ModifyModeTypes.Rotate);
        //if (main.modifyMode.selectedRoom != null)
        //{
        //    main.modifyMode.selectedRoom.Rotate();
        //}
    }
    public void Modify()
    {
        ModifyMode.Instance.SetModifyMode(ModifyMode.ModifyModeTypes.Edit);

        Debug.Log("Modify function is not implemented");
    }
    public void Properties() {
        Debug.Log("Properties function is not implemented");
    }
    public void Delete()
    {
        ModifyMode.Instance.SetModifyMode(ModifyMode.ModifyModeTypes.Delete);
        //if (main.modifyMode.selectedRoom != null)
        //{
        //    main.modifyMode.selectedRoom.Delete();
        //}
    }
}
