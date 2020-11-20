using System.Linq;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

namespace DesignPlatform.Core {
    public class POVMode : Mode {

        private static POVMode instance;

        public float mouseSensitivity = 50f;

        public static GameObject player;
        private GameObject notificationObject = null;
        public Camera PlanCamera;
        public Camera POVCamera;
        private GameObject ui;
        private bool ShowWallLines;
        private bool ShowOpeningLines;

        float xRotation = 0f;

        public enum ModeType {
            POV,
            MENU
        }

        private ModeType currentModeType = ModeType.POV;

        public static POVMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new POVMode()); }
        }

        POVMode() {
            ui = Object.FindObjectsOfType<Canvas>().Where(o => o.gameObject.name == "UI").ToList()[0].gameObject;
        }

        public override void Tick() {
            TickModeType();

            if (Input.GetKeyDown(KeyCode.Escape)) {
                SetModeType(ModeType.MENU);
                Main.Instance.SetMode(SelectMode.Instance);
            }

        }
        public override void OnModeResume() { 
            ui.SetActive(false);
            player = GameObject.Find("First person player");
            PlanCamera = GameObject.Find("Plan Camera").GetComponent<Camera>();
            POVCamera = player.GetComponentInChildren<Camera>(true);

            GlobalSettings.ShowWallLines = false;
            GlobalSettings.ShowOpeningLines = false;
            
            foreach (Room room in Building.Instance.Rooms) {
                room.UpdateRender2D();
            }
            foreach (Opening opening in Building.Instance.Openings) {
                opening.UpdateRender2D();
            }

            POVCamera.gameObject.SetActive(true);
            PlanCamera.gameObject.SetActive(false);

            // Delete preexisting interfaces and walls build new ones
            Building.Instance.RebuildPOVElements();

            // Generates notification in corner of screen
            GameObject notificationParent = POVCamera.gameObject.GetComponentsInChildren<RectTransform>().Where(t => t.gameObject.name == "UIPanel3D").First().gameObject;
            string notificationText = "You can exit POV mode at any time by pressing the escape button.";
            string notificationTitle = "POV Mode";
            notificationObject = NotificationHandler.GenerateNotification(notificationText, notificationTitle, new Vector3(10, -10, 0), notificationParent);

            currentModeType = ModeType.POV;
            SetModeType(ModeType.POV);
            OnModeTypeResume();
        }
        public override void OnModePause() {
            ui.SetActive(true);
            POVCamera.gameObject.SetActive(false);
            PlanCamera.gameObject.SetActive(true);

            GlobalSettings.ShowWallLines = true;
            GlobalSettings.ShowOpeningLines = true;

            foreach (Room room in Building.Instance.Rooms) {
                room.UpdateRender2D();
            }
            foreach (Opening opening in Building.Instance.Openings) {
                opening.UpdateRender2D();
            }

            NotificationHandler.DestroyNotification(notificationObject);
            notificationObject = null;
        }

        /// <summary>
        /// Set mode type
        /// </summary>
        /// <param name="modeType"></param>
        public void SetModeType(ModeType modeType) {
            if (modeType != currentModeType) {
                OnModeTypePause();
                currentModeType = modeType;
                OnModeTypeResume();
            }
        }
        public void TickModeType() {
            switch (currentModeType) {
                case ModeType.POV:
                    UpdatePOVCamera();
                    break;

                case ModeType.MENU:
                    break;
            }
        }
        public void OnModeTypeResume() {
            switch (currentModeType) {
                case ModeType.POV:
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    UnityEngine.Cursor.visible = false; 
                    break;

                case ModeType.MENU:
                    break;
            }
        }
        public void OnModeTypePause() {
            switch (currentModeType) {
                case ModeType.POV:
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    UnityEngine.Cursor.visible = true;
                    break;

                case ModeType.MENU:
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void UpdatePOVCamera() {
            float mouseX = Input.GetAxis("Mouse X"); //* mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y"); //* mouseSensitivity * Time.deltaTime;

            // Look Up/Down 
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            POVCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Se til siden
            player.transform.Rotate(Vector3.up * mouseX);
        }

    }
}