using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using JetBrains.Annotations;

public class ButtonManagerModify : MonoBehaviour {
    public Main main;

    //SLET
    //private List<Vector3> wallControlPoints;
    //private Vector3 normal;
    //private Room room;
    //SLET
    public void Move() {
        ModifyMode.Instance.SetModeType(ModifyMode.ModeType.Move);
    }
    public void Rotate() {
        ModifyMode.Instance.SetModeType(ModifyMode.ModeType.Rotate);
    }
    public void Modify() {
        ModifyMode.Instance.SetModeType(ModifyMode.ModeType.Edit);
    }
    public void Properties() {
        ModifyMode.Instance.SetModeType(ModifyMode.ModeType.None);
    }

    public void Delete() {
        ModifyMode.Instance.SetModeType(ModifyMode.ModeType.Delete);
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
    //public void TestWallBuilder() {
        //wallControlPoints = new List<Vector3> {
        //    ModifyMode.Instance.selectedRoom.GetControlPoints(localCoordinates: true)[0],
        //    ModifyMode.Instance.selectedRoom.GetControlPoints(localCoordinates: true)[1]
        //};
        //normal = ModifyMode.Instance.selectedRoom.GetWallNormals(localCoordinates: true)[0];
        //room = ModifyMode.Instance.selectedRoom;

        //Wall wall = new Wall();

        //wall.InitializeWall(wallControlPoints, normal, room);
    //}
}
