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
        // (Midlertidigt Sebastian) Test af structural modul
        foreach (Room room in Building.Instance.GetRooms()) {
            Dictionary<int, List<Structural.Load>> loadTable = Structural.LoadDistribution.AreaLoad(room);
            foreach (int indexWall in loadTable.Keys) {
                foreach (Structural.Load load in loadTable[indexWall]) {
                    Debug.Log("Load on wall: " + indexWall + "  Start: " + load.pStart + "  End: " + load.pEnd + "  Magnitude: " + load.magnitude);
                }
            }
         }
        
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
