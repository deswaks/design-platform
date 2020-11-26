using DesignPlatform.Core;
using DesignPlatform.Utils;
using UnityEngine;

namespace DesignPlatform.Modes {
    public class MoveMode : Mode {

        public Vector3 Offset { get; set; }
        public GameObject Handle { get; private set; }
        public GameObject HandlePrefab { get; private set; }


        private static MoveMode instance;
        public static MoveMode Instance {
            get { return instance ?? (instance = new MoveMode()); }
        }

        MoveMode() {
            HandlePrefab = AssetUtil.LoadAsset<GameObject>("prefabs", "MoveHandle");
        }


        public override void Tick() {
        }
        public override void OnModeResume() {
            Room selectedRoom = SelectMode.Instance.selection;
            if (selectedRoom != null) {
                CreateHandle(selectedRoom);
                selectedRoom.State = RoomState.MOVING;
            }
        }
        public override void OnModePause() {
            Room selectedRoom = SelectMode.Instance.selection;
            if (selectedRoom != null) {
                selectedRoom.State = RoomState.STATIONARY;
            }
            RemoveHandle();
        }


        /// <summary>
        /// 
        /// </summary>
        public void CreateHandle(Room room) {
            // Remove prior handle
            RemoveHandle();

            // Create the handle
            Handle = Object.Instantiate(HandlePrefab);

            // Set position
            Handle.transform.position = room.GetTagLocation(localCoordinates: true);
            Handle.transform.SetParent(room.gameObject.transform, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        public void RemoveHandle() {
            if (Handle != null) {
                Object.Destroy(Handle);
            }
            Handle = null;
        }


    }
}