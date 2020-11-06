using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.Core {
    public class SelectMode : Mode {

        private static SelectMode instance;
        private Mode currentMode;
        public Room selection;

        public static SelectMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new SelectMode()); }
        }

        /// <summary>
        /// Gameloop tick is run every frame the mode is active
        /// </summary>
        public override void Tick() {

            if (Input.GetMouseButtonDown(0)) {
                Select();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(2)) {
                Deselect();
                SetMode(null);

            }

            if (Input.GetKeyDown(KeyCode.Delete)) {
                if (selection != null) {selection.Delete(); Deselect(); }
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                if (selection != null) SetMode(ExtrudeMode.Instance);
            }

            if (Input.GetKeyDown(KeyCode.M)) {
                if (selection != null) SetMode(MoveMode.Instance);
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                if (selection != null) selection.Rotate();
            }

            if (currentMode != null) {
                currentMode.Tick();
            }
        }
        public override void OnModeResume() {
            SetMode(null);
        }
        public override void OnModePause() {
            Deselect();
            SetMode(null);
        }

        public void SetMode(Mode mode) {
            if (currentMode != null) {
                currentMode.OnModePause();
            }
            currentMode = mode;
            if (currentMode != null) {
                currentMode.OnModeResume();
            }
        }

        private void Select() {
            // Abort if UI object is under cursor
            bool overGO = EventSystem.current.IsPointerOverGameObject();
            if (overGO) {
                return;
            }

            // Set new room selection
            Room clickedRoom = GetClickedRoom();
            if (clickedRoom != null) {
                if (clickedRoom == selection) {
                    return;
                }
                else {
                    Deselect();
                    selection = clickedRoom;
                    selection.UpdateRender2D(highlighted: true);
                    SetMode(null);
                }
            }
            else {
                Deselect();
                SetMode(null);
            }
        }

        private void Deselect() {

            if (selection != null) {
                selection.UpdateRender2D(highlighted: false);
                selection.State = RoomState.STATIONARY;
            }
            selection = null;

        }

        private Room GetClickedRoom() {
            Room clickedRoom = null;
            Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hitInfo)) {
                // if the hit game object is a room (ie. it is on layer 8)
                if (hitInfo.collider.gameObject.layer == 8) {
                    clickedRoom = hitInfo.collider.gameObject.GetComponent<Room>();
                }
                else {
                    clickedRoom = hitInfo.collider.gameObject.GetComponentInParent<Room>();
                }
            }
            return clickedRoom;
        }

    }
}