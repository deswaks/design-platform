using DesignPlatform.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.Modes {

    /// <summary>
    /// The mode wherein spaces may be selected.
    /// </summary>
    public class SelectMode : Mode {

        /// <summary>The currenty selected space.</summary>
        public Core.Space Selection { get; private set; }

        /// <summary>The single instance that exists of this singleton class.</summary>
        public static SelectMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new SelectMode()); }
        }



        /// <summary>The single instance that exists of this singleton class.</summary>
        private static SelectMode instance;
        /// <summary>The currently active sub-mode.</summary>
        private Mode currentMode;



        /// <summary>Defines the actions to take at every frame where this mode is active.</summary>
        public override void Tick() {

            if (Input.GetMouseButtonDown(0)) {
                Select();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2)) {
                Deselect();
                SetMode(null);
            }

            if (Input.GetKeyDown(KeyCode.Delete)) {
                if (Selection != null) { Selection.Delete(); Deselect(); }
            }

            if (Settings.allowHotkeys) {
                if (Input.GetKeyDown(KeyCode.E)) {
                    if (Selection != null) SetMode(ExtrudeMode.Instance);
                }

                if (Input.GetKeyDown(KeyCode.M)) {
                    if (Selection != null) SetMode(MoveMode.Instance);
                }

                if (Input.GetKeyDown(KeyCode.R)) {
                    if (Selection != null) Selection.Rotate();
                }

                // Check if any number key was pressed
                if (Input.GetKeyDown(KeyCode.Alpha0)) {
                    if (Selection != null) Selection.Function = SpaceFunction.DEFAULT;
                    Selection.UpdateRender2D();
                }
                for (int i = (int)KeyCode.Alpha1; i < (int)KeyCode.Alpha9; i++) {
                    if (Input.GetKeyDown((KeyCode)i)) {
                        if (Selection != null) {
                            Selection.Function = (SpaceFunction)i - (int)KeyCode.Alpha1 + 10;
                            Selection.UpdateRender2D();
                        }
                    }
                }
            }
            

            if (currentMode != null) {
                currentMode.Tick();
            }
        }

        /// <summary>Defines the actions to take when changing into this mode.</summary>
        public override void OnModeResume() {
            Settings.allowHotkeys = true;
            SetMode(null);
        }

        /// <summary>Defines the actions to take when changing out of this mode.</summary>
        public override void OnModePause() {
            Settings.allowHotkeys = false;
            Deselect();
            SetMode(null);
        }

        /// <summary>
        /// Change the mode that is updates from this mode.
        /// </summary>
        /// <param name="mode">Mode to change to.</param>
        public void SetMode(Mode mode) {
            if (currentMode != null) {
                currentMode.OnModePause();
            }
            currentMode = mode;
            if (currentMode != null) {
                currentMode.OnModeResume();
            }
        }

        /// <summary>
        /// Set the space under the pointer as the currently selected space.
        /// </summary>
        private void Select() {
            // Abort if UI object is under cursor
            bool overGO = EventSystem.current.IsPointerOverGameObject();
            if (overGO) {
                return;
            }

            // Set new space selection
            Core.Space clickedRoom = GetClickedSpace();
            if (clickedRoom != null) {
                if (clickedRoom == Selection) {
                    return;
                }
                else {
                    Deselect();
                    Selection = clickedRoom;
                    Selection.UpdateRender2D(highlighted: true);
                    SetMode(null);
                }
            }
            else {
                Deselect();
                SetMode(null);
            }
        }

        /// <summary>
        /// Delesect the currently selected room (this means no space is selcted afterwards).
        /// </summary>
        private void Deselect() {

            if (Selection != null) {
                Selection.UpdateRender2D( highlighted: false);
                Selection.MoveState = MoveState.STATIONARY;
            }
            Selection = null;

        }

        /// <summary>
        /// Finds the room underneath the pointer.
        /// </summary>
        /// <returns>The room underneath the pointer.</returns>
        private Core.Space GetClickedSpace() {
            Core.Space clickedRoom = null;
            Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            int spaceMask = 1 << 8 | 1 << 9;
            if (Physics.Raycast(ray: ray, maxDistance: 1000f, hitInfo: out RaycastHit hitInfo, layerMask: spaceMask)) {
                // if the hit game object is a space (ie. it is on layer 8)
                if (hitInfo.collider.gameObject.layer == 8) {
                    clickedRoom = hitInfo.collider.gameObject.GetComponent<Core.Space>();
                }
                else {
                    clickedRoom = hitInfo.collider.gameObject.GetComponentInParent<Core.Space>();
                }
            }
            return clickedRoom;
        }

    }
}