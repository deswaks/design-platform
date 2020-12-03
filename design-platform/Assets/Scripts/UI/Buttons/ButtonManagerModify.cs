using DesignPlatform.Core;
using DesignPlatform.Modes;
using TMPro;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// 
    /// </summary>
    public class ButtonManagerModify : MonoBehaviour {
        public void Move() {
            SelectMode.Instance.SetMode(MoveMode.Instance);
        }
        public void Rotate() {
            if (SelectMode.Instance.Selection != null) {
                SelectMode.Instance.Selection.Rotate();
            }
        }
        public void Modify() {
            SelectMode.Instance.SetMode(ExtrudeMode.Instance);
        }
        public void Properties() {
            SelectMode.Instance.SetMode(null);
        }
        public void Delete() {
            if (SelectMode.Instance.Selection != null) {
                SelectMode.Instance.Selection.Delete();
            }
        }
        public void PublishSpaceType(int spaceType) {
            SelectMode.Instance.Selection.Function = (SpaceFunction)spaceType;
        }
        public void PublishSpaceNote() {
            //GameObject myInputGO = GameObject.Find("InputField Room Note");
            TMP_InputField myInputIF = GameObject.Find("InputProperty").GetComponent<TMP_InputField>();
            SelectMode.Instance.Selection.CustomNote = myInputIF.text;
            myInputIF.text = "";
            Debug.Log(SelectMode.Instance.Selection.CustomNote);
        }


    }
}