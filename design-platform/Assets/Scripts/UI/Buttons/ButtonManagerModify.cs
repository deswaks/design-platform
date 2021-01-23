using DesignPlatform.Core;
using DesignPlatform.Modes;
using TMPro;
using UnityEngine;

namespace DesignPlatform.UI {

    /// <summary>
    /// Contains functions for manipulating spaces in the modify UI menu
    /// </summary>
    public class ButtonManagerModify : MonoBehaviour {
        /// <summary>
        /// Activates move of currently selected Space
        /// </summary>
        public void Move() {
            SelectMode.Instance.SetMode(MoveMode.Instance);
        }
        /// <summary>
        /// Rotates currently selected Space 90 degrees
        /// </summary>
        public void Rotate() {
            if (SelectMode.Instance.Selection != null) {
                SelectMode.Instance.Selection.Rotate();
            }
        }
        /// <summary>
        /// Rotates currently selected Space 90 degrees
        /// </summary>
        public void Modify() {
            SelectMode.Instance.SetMode(ExtrudeMode.Instance);
        }
        /// <summary>
        /// Cancels current mode, as the Properties menu opens.
        /// </summary>
        public void Properties() {
            SelectMode.Instance.SetMode(null);
        }
        /// <summary>
        /// Deletes currently selected Space 
        /// </summary>
        public void Delete() {
            if (SelectMode.Instance.Selection != null) {
                SelectMode.Instance.Selection.Delete();
            }
        }
        /// <summary>
        /// Reads room type from properties window and assigns it to the currently selected Space
        /// </summary>
        public void PublishSpaceType(int spaceType) {
            SelectMode.Instance.Selection.Function = (SpaceFunction)spaceType;
        }
        /// <summary>
        /// Reads note from properties window and assigns it to the currently selected Space
        /// </summary>
        public void PublishSpaceNote() {
            //GameObject myInputGO = GameObject.Find("InputField Room Note");
            TMP_InputField myInputIF = GameObject.Find("InputProperty").GetComponent<TMP_InputField>();
            SelectMode.Instance.Selection.CustomNote = myInputIF.text;
            myInputIF.text = "";
            Debug.Log(SelectMode.Instance.Selection.CustomNote);
        }


    }
}