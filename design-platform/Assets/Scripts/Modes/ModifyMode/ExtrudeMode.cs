using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPlatform.Utils;

namespace DesignPlatform.Core {
    public class ExtrudeMode : Mode {

        private static ExtrudeMode instance;
        public List<EditHandle> Handles { get; private set; }
        public GameObject HandlePrefab { get; private set; }


        public static ExtrudeMode Instance {
            get { return instance ?? (instance = new ExtrudeMode()); }
        }


        ExtrudeMode() {
            HandlePrefab = AssetUtil.LoadGameObject("prefabs", "edit_handle");
        }


        public override void Tick() {
        }
        public override void OnModeResume() {
            Room selectedRoom = SelectMode.Instance.selection;
            if (selectedRoom != null) {
                CreateHandles(selectedRoom);
            }
        }
        public override void OnModePause() {
            Room selectedRoom = SelectMode.Instance.selection;
            if (selectedRoom != null) {
                RemoveHandles(selectedRoom);
                selectedRoom.ResetOrigin();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        public void CreateHandles(Room room) {
            for (int i = 0; i < room.GetControlPoints().Count; i++) {
                GameObject HandleGO = Object.Instantiate(HandlePrefab);
                HandleGO.transform.SetParent(room.gameObject.transform, true);

                EditHandle Handle = HandleGO.GetComponent<EditHandle>();
                Handle.InitializeHandle(i);
                Handles.Add(Handle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        public void RemoveHandles(Room room) {
            if (Handles != null) {
                foreach (EditHandle handle in Handles) {
                    Object.Destroy(handle.gameObject);
                }
            }
        }


    }
}