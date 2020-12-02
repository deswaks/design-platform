using DesignPlatform.Core;
using DesignPlatform.Modes;
using TMPro;
using UnityEngine;

namespace DesignPlatform.UI {
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
        public void PublishSpaceType(int spaceType) {
            SelectMode.Instance.selection.Function = (SpaceFunction)spaceType;
        }
        public void PublishSpaceNote() {
            //GameObject myInputGO = GameObject.Find("InputField Room Note");
            TMP_InputField myInputIF = GameObject.Find("InputProperty").GetComponent<TMP_InputField>();
            SelectMode.Instance.selection.CustomNote = myInputIF.text;
            myInputIF.text = "";
            Debug.Log(SelectMode.Instance.selection.CustomNote);
        }


    }
}