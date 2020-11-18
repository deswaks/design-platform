using DesignPlatform.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Core {
    public class ExtrudeMode : Mode {

        private static ExtrudeMode instance;
        public List<ExtrudeHandle> Handles { get; private set; }
        public GameObject HandlePrefab { get; private set; }


        public static ExtrudeMode Instance {
            get { return instance ?? (instance = new ExtrudeMode()); }
        }


        ExtrudeMode() {
            HandlePrefab = AssetUtil.LoadAsset<GameObject>("prefabs", "ExtrudeHandle");
            Handles = new List<ExtrudeHandle>();
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
            RemoveHandles();
            Room selectedRoom = SelectMode.Instance.selection;
            if (selectedRoom != null) {
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

                ExtrudeHandle handle = HandleGO.GetComponent<ExtrudeHandle>();
                handle.InitHandle(i);
                Handles.Add(handle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        public void RemoveHandles() {
            if (Handles.Count > 0) {
                foreach (ExtrudeHandle handle in Handles) {
                    Object.Destroy(handle.gameObject);
                }
                Handles.Clear();
            }
        }


    }
}