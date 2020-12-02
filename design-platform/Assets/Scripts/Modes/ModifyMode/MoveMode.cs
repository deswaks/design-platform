using DesignPlatform.Core;
using DesignPlatform.Geometry;
using DesignPlatform.Utils;
using UnityEngine;


namespace DesignPlatform.Modes {

    /// <summary>
    /// The mode wherein spaces can be moved by dragging them around.
    /// </summary>
    public class MoveMode : Mode {

        /// <summary>The handle that are currently active in this mode.</summary>
        public GameObject Handle { get; private set; }

        /// <summary>The single instance that exists of this singleton class.</summary>
        public static MoveMode Instance {
            get { return instance ?? (instance = new MoveMode()); }
        }



        /// <summary>The single instance that exists of this singleton class.</summary>
        private static MoveMode instance;

        /// <summary>Prefab for new handles.</summary>
        private readonly GameObject HandlePrefab;



        /// <summary>Default constructor.</summary>
        MoveMode() {
            HandlePrefab = AssetUtil.LoadAsset<GameObject>("prefabs", "MoveHandle");
        }



        /// <summary>
        /// Defines the actions to take at every frame where this mode is active.
        /// </summary>
        public override void Tick() {
        }

        /// <summary>
        /// Defines the actions to take when changing into this mode.
        /// </summary>
        public override void OnModeResume() {
            Core.Space selectedRoom = SelectMode.Instance.Selection;
            if (selectedRoom != null) {
                CreateHandle(selectedRoom);
                selectedRoom.MoveState = MoveState.MOVING;
            }
        }

        /// <summary>
        /// Defines the actions to take when changing out of this mode.
        /// </summary>
        public override void OnModePause() {
            Core.Space selectedRoom = SelectMode.Instance.Selection;
            if (selectedRoom != null) {
                selectedRoom.MoveState = MoveState.STATIONARY;
            }
            RemoveHandle();
        }

        /// <summary>
        /// Creates a move handle for the given space.
        /// </summary>
        /// <param name="space"></param>
        public void CreateHandle(Core.Space space) {
            // Remove prior handle
            RemoveHandle();

            // Create the handle
            Handle = Object.Instantiate(HandlePrefab);

            // Set position
            Handle.transform.position = space.GetTagLocation(localCoordinates: true);
            Handle.transform.SetParent(space.gameObject.transform, false);
        }

        /// <summary>
        /// Removes the currently active handle.
        /// </summary>
        public void RemoveHandle() {
            if (Handle != null) {
                Object.Destroy(Handle);
            }
            Handle = null;
        }


    }
}