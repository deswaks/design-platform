using DesignPlatform.Core;
using DesignPlatform.Geometry;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.Modes {

    /// <summary>
    /// The mode wherein new spaces may be built.
    /// </summary>
    public class BuildMode : Mode {

        /// <summary>The currently selected shape. New rooms will be built in this shape.</summary>
        public SpaceShape SelectedShape {
            get { return selectedShape; }
            set { selectedShape = value; RebuildPreview(selectedShape);}
        }

        /// <summary>The space object used for preview purposes.</summary>
        public Core.Space previewSpace;

        /// <summary>The single instance that exists of this singleton class.</summary>
        public static BuildMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new BuildMode()); }
        }



        /// <summary>Internal variable to save the selected shape.</summary>
        private SpaceShape selectedShape = SpaceShape.RECTANGLE;

        /// <summary>The single instance that exists of this singleton class.</summary>
        private static BuildMode instance;



        /// <summary>Default constructor.</summary>
        public BuildMode() {
            SelectedShape = SpaceShape.RECTANGLE;
        }



        /// <summary>Defines the actions to take at every frame where this mode is active.</summary>
        public override void Tick() {
            if (Input.GetMouseButtonDown(0)) {
                if (EventSystem.current.IsPointerOverGameObject()) {
                    Main.Instance.SetMode(SelectMode.Instance);
                }
                else {
                    Build();
                }
            }

            if (Input.GetMouseButtonDown(1)) {
                if ((int)SelectedShape == 4) {
                    SelectedShape = 0;
                }
                else {
                    SelectedShape = (SpaceShape)(int)SelectedShape + 1;
                }
            }

            if (Settings.allowHotkeys) {
                if (Input.GetKeyDown(KeyCode.R)) {
                    previewSpace.Rotate(); //remember to tell this to the user when implementing tooltips
                }

                if (Input.GetKeyDown(KeyCode.L)) {
                    SelectedShape = SpaceShape.LSHAPE;
                }

                if (Input.GetKeyDown(KeyCode.U)) {
                    SelectedShape = SpaceShape.USHAPE;
                }

                if (Input.GetKeyDown(KeyCode.S)) {
                    SelectedShape = SpaceShape.SSHAPE;
                }

                if (Input.GetKeyDown(KeyCode.T)) {
                    SelectedShape = SpaceShape.TSHAPE;
                }

                if (Input.GetKeyDown(KeyCode.D)) {
                    OpeningMode.Instance.SelectedFunction = OpeningFunction.DOOR;
                    Main.Instance.SetMode(OpeningMode.Instance);
                }

                if (Input.GetKeyDown(KeyCode.W)) {
                    OpeningMode.Instance.SelectedFunction = OpeningFunction.WINDOW;
                    Main.Instance.SetMode(OpeningMode.Instance);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Main.Instance.SetMode(SelectMode.Instance);
            }

            if (previewSpace) { UpdatePreviewLocation(); }
        }

        /// <summary>
        /// Defines the actions to take when changing into this mode.
        /// </summary>
        public override void OnModeResume() {
            Settings.allowHotkeys = true;
            if (previewSpace == null) {
                previewSpace = Building.Instance.BuildSpace(buildShape: SelectedShape, preview: true);
            }
        }

        /// <summary>
        /// Defines the actions to take when changing out of this mode.
        /// </summary>
        public override void OnModePause() {
            Settings.allowHotkeys = false;
            if (previewSpace != null) previewSpace.Delete();
            previewSpace = null;
        }
        
        /// <summary>
        /// Revuild the preview space object using the given space shape.
        /// </summary>
        /// <param name="SelectedShape">Shape of new preview object.</param>
        public void RebuildPreview(SpaceShape SelectedShape = SpaceShape.RECTANGLE) {
            if (previewSpace != null) previewSpace.Delete();
            previewSpace = Building.Instance.BuildSpace(buildShape: SelectedShape, preview: true);
        }

        /// <summary>
        /// Build the room and insert it into the model.
        /// </summary>
        public void Build() {
            Core.Space builtRoom = Building.Instance.BuildSpace(buildShape: SelectedShape, templateSpace: previewSpace);
            builtRoom.MoveState = MoveState.STATIONARY;

        }

        /// <summary>
        /// Move the preview space to the location of the cursor.
        /// </summary>
        public void UpdatePreviewLocation() {
            Plane basePlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //simple ray cast from the main camera. Notice there is no range

            float distance;
            if (basePlane.Raycast(ray, out distance)) {
                Vector3 hitPoint = ray.GetPoint(distance);
                previewSpace.Move(hitPoint);
            }
        }

    }
}