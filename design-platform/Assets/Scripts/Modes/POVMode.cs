using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using DesignPlatform.UI;
using DesignPlatform.Core;
using DesignPlatform.Database;
using DesignPlatform.Geometry;

namespace DesignPlatform.Modes {

    /// <summary>
    /// The mode wherein the user can inspect and interact with their design three dimensionally.
    /// </summary>
    public class POVMode : Mode {

        /// <summary>The single instance that exists of this singleton class.</summary>
        public static POVMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new POVMode()); }
        }



        /// <summary>The single instance that exists of this singleton class.</summary>
        private static POVMode instance;
        /// <summary>The player object. Used to carry the camera.</summary>
        private static GameObject player;
        /// <summary>The forward vector of the player camera.</summary>
        private float xRotation = 0f;

        /// <summary>Reference to the plan camera object. These are used to disable them and enable again.</summary>
        private Camera PlanCamera;
        /// <summary>Reference to POV camera objects. These are used to disable them and enable again.</summary>
        private Camera POVCamera;
        /// <summary>Reference to UI object. These are used to disable them and enable again.</summary>
        private GameObject ui;
        /// <summary>Object to display notification on top of the view.</summary>
        private GameObject notificationObject = null;



        /// <summary>Default constructor.</summary>
        POVMode() {
            ui = Object.FindObjectsOfType<Canvas>().Where(o => o.gameObject.name == "UI").ToList()[0].gameObject;
        }



        /// <summary>
        /// Defines the actions to take at every frame where this mode is active.
        /// </summary>
        public override void Tick() {
            UpdatePOVCamera();

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Main.Instance.SetMode(SelectMode.Instance);
            }

        }

        /// <summary>
        /// Defines the actions to take when changing into this mode.
        /// </summary>
        public override void OnModeResume() {
            ui.SetActive(false);
            player = GameObject.Find("First person player");
            PlanCamera = GameObject.Find("Plan Camera").GetComponent<Camera>();
            POVCamera = player.GetComponentInChildren<Camera>(true);

            UI.Settings.ShowWallLines = false;
            UI.Settings.ShowOpeningLines = false;

            foreach (Core.Space room in Building.Instance.Spaces) {
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
        }

        /// <summary>
        /// Defines the actions to take when changing out of this mode.
        /// </summary>
        public override void OnModePause() {
            ui.SetActive(true);
            POVCamera.gameObject.SetActive(false);
            PlanCamera.gameObject.SetActive(true);

            UI.Settings.ShowWallLines = true;
            UI.Settings.ShowOpeningLines = true;

            foreach (Core.Space room in Building.Instance.Spaces) {
                room.UpdateRender2D();
            }
            foreach (Opening opening in Building.Instance.Openings) {
                opening.UpdateRender2D();
            }

            NotificationHandler.DestroyNotification(notificationObject);
            notificationObject = null;
        }

        /// <summary>
        /// Set the view of this camera according to mouse position.
        /// </summary>
        public void UpdatePOVCamera() {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Look Up/Down 
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            POVCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Se til siden
            player.transform.Rotate(Vector3.up * mouseX);
        }

    }
}