using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using JetBrains.Annotations;
using TMPro;

namespace DesignPlatform.Core {
    public class ButtonManagerModify : MonoBehaviour {
        public void Move() {
            SelectMode.Instance.SetMode(MoveMode.Instance);
        }
        public void Rotate() {
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
            if (SelectMode.Instance.selection != null) {
                SelectMode.Instance.selection.Delete();
            }
        }
        public void PublishRoomType(int buildType) {
            SelectMode.Instance.selection.SetRoomType((RoomType)buildType);
        }
        public void PublishRoomNote() {
            //GameObject myInputGO = GameObject.Find("InputField Room Note");
            TMP_InputField myInputIF = GameObject.Find("InputProperty").GetComponent<TMP_InputField>();
            SelectMode.Instance.selection.SetRoomNote(myInputIF.text);
            myInputIF.text = "";
            Debug.Log(SelectMode.Instance.selection.customProperty);
        }


    }
}