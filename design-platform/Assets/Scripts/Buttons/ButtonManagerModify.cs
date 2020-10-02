using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using JetBrains.Annotations;

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
        Debug.Log("You just turned on the properties dropdown menu, dope!");
    }

    public void StructuralAnalysis() {
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

    public void PublishRoomType(int buildType) {
        ModifyMode.Instance.selectedRoom.SetRoomType((RoomType)buildType);
    }

    public void PublishRoomNote() {
        GameObject myInputGO = GameObject.Find("InputField Room Note");
        InputField myInputIF = myInputGO.GetComponent<InputField>();
        ModifyMode.Instance.selectedRoom.SetRoomNote(myInputIF.text);
        myInputIF.text = "";
    }

}
