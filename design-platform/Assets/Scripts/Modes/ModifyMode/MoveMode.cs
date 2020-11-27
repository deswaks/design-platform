using DesignPlatform.Core;
using DesignPlatform.Geometry;
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
            Core.Space selectedRoom = SelectMode.Instance.selection;
            if (selectedRoom != null) {
                CreateHandle(selectedRoom);
                selectedRoom.State = SpaceState.MOVING;
            }
        }
        public override void OnModePause() {
            Core.Space selectedRoom = SelectMode.Instance.selection;
            if (selectedRoom != null) {
                selectedRoom.State = SpaceState.STATIONARY;
            }
            RemoveHandle();
        }


        /// <summary>
        /// 
        /// </summary>
        public void CreateHandle(Core.Space room) {
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