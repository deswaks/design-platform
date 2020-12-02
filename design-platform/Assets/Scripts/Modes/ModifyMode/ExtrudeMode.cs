using DesignPlatform.Core;
using DesignPlatform.UI;
using DesignPlatform.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Modes {

    /// <summary>
    /// The mode wherein the walls/faces of spaces can be modified by extrusion.
    /// </summary>
    public class ExtrudeMode : Mode {

        /// <summary>The handles that are currentl active in this mode.</summary>
        public List<ExtrudeHandle> Handles { get; private set; }
        
        /// <summary>The single instance that exists of this singleton class.</summary>
        public static ExtrudeMode Instance {
            get { return instance ?? (instance = new ExtrudeMode()); }
        }



        /// <summary>The single instance that exists of this singleton class.</summary>
        private static ExtrudeMode instance;

        /// <summary>Prefab for new handles.</summary>
        private readonly GameObject HandlePrefab;



        /// <summary>Default constructor.</summary>
        ExtrudeMode() {
            HandlePrefab = AssetUtil.LoadAsset<GameObject>("prefabs", "ExtrudeHandle");
            Handles = new List<ExtrudeHandle>();
        }




        /// <summary>Defines the actions to take at every frame where this mode is active.</summary>
        public override void Tick() {
        }

        /// <summary>Defines the actions to take when changing into this mode.</summary>
        public override void OnModeResume() {
            Core.Space selectedRoom = SelectMode.Instance.Selection;
            if (selectedRoom != null) {
                CreateHandles(selectedRoom);
            }
        }

        /// <summary>Defines the actions to take when changing out of this mode.</summary>
        public override void OnModePause() {
            RemoveHandles();
            Core.Space selectedRoom = SelectMode.Instance.Selection;
            if (selectedRoom != null) {
                selectedRoom.ResetOrigin();
            }
        }

        /// <summary>
        /// Sppawns handles for extruding the walls of a space.
        /// </summary>
        /// <param name="space">Space to spawn handles for.</param>
        public void CreateHandles(Core.Space space) {
            for (int i = 0; i < space.GetControlPoints().Count; i++) {
                GameObject HandleGO = Object.Instantiate(HandlePrefab);
                HandleGO.transform.SetParent(space.gameObject.transform, true);

                ExtrudeHandle handle = HandleGO.GetComponent<ExtrudeHandle>();
                handle.InitHandle(i);
                Handles.Add(handle);
            }
        }

        /// <summary>
        /// Removes the current handles for the mode.
        /// </summary>
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