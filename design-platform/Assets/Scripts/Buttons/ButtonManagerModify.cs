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
    }
    public void Properties() {
        foreach (Room room in Building.Instance.GetRooms()) {
            Dictionary<int, List<Structural.Load>> loadTable = Structural.LoadDistribution.AreaLoad(room);
            for (int i = 0; i < loadTable.Count; i++) {
                for (int j = 0; j < loadTable[i].Count; j++) {
                    Structural.Load load = loadTable[i][j];
                    Debug.Log("Load on wall: "+i+"  Start: "+load.pStart+"  End: "+load.pEnd+"  Magnitude: "+load.magnitude);
                }
            }
         }
        
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
