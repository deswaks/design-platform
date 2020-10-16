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
        SelectMode.Instance.SetMode(MoveMode.Instance);
    }
    public void Rotate() {
        SelectMode.Instance.SetMode(null);
        if (SelectMode.Instance.selection != null) {
            SelectMode.Instance.selection.Rotate();
        }
    }
    public void Modify() {
        SelectMode.Instance.SetMode(ExtrudeMode.Instance);
    }
    public void Properties() {
        SelectMode.Instance.SetMode(null);
    }

    public void Delete() {
        SelectMode.Instance.SetMode(null);
        if (SelectMode.Instance.selection != null) {
            SelectMode.Instance.selection.Delete();
        }
    }

    public void PublishRoomType(int buildType) {
        SelectMode.Instance.selection.SetRoomType((RoomType)buildType);
    }

    public void PublishRoomNote() {
        GameObject myInputGO = GameObject.Find("InputField Room Note");
        InputField myInputIF = myInputGO.GetComponent<InputField>();
        SelectMode.Instance.selection.SetRoomNote(myInputIF.text);
        myInputIF.text = "";
    }
    //public void TestWallBuilder() {
        //wallControlPoints = new List<Vector3> {
        //    SelectMode.Instance.selection.GetControlPoints(localCoordinates: true)[0],
        //    SelectMode.Instance.selection.GetControlPoints(localCoordinates: true)[1]
        //};
        //normal = SelectMode.Instance.selection.GetWallNormals(localCoordinates: true)[0];
        //room = SelectMode.Instance.selection;

        //Wall wall = new Wall();

        //wall.InitializeWall(wallControlPoints, normal, room);
    //}
}
