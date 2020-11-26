using DesignPlatform.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.Modes {
    public class BuildMode : Mode {

        private static BuildMode instance;

        private SpaceShape selectedShape = SpaceShape.RECTANGLE;
        public SpaceShape SelectedShape {
            get {
                return selectedShape;
            }
            set {
                selectedShape = value;
                RebuildPreview(selectedShape);
            }
        }

        // Set at runtime
        public Core.Space previewSpace;


        public static BuildMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new BuildMode()); }
        }

        public BuildMode() {
            SelectedShape = SpaceShape.RECTANGLE;
        }

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
                OpeningMode.Instance.SelectedShape = OpeningFunction.DOOR;
                Main.Instance.SetMode(OpeningMode.Instance);
            }

            if (Input.GetKeyDown(KeyCode.W)) {
                OpeningMode.Instance.SelectedShape = OpeningFunction.WINDOW;
                Main.Instance.SetMode(OpeningMode.Instance);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Main.Instance.SetMode(SelectMode.Instance);
            }

            if (previewSpace) { UpdatePreviewLocation(); }
        }

        public override void OnModeResume() {
            if (previewSpace == null) {
                previewSpace = Building.Instance.BuildSpace(buildShape: SelectedShape, preview: true);
            }
        }

        public override void OnModePause() {
            if (previewSpace != null) previewSpace.Delete();
            previewSpace = null;
        }

        public void RebuildPreview(SpaceShape SelectedShape = SpaceShape.RECTANGLE) {
            if (previewSpace != null) previewSpace.Delete();
            previewSpace = Building.Instance.BuildSpace(buildShape: SelectedShape, preview: true);
        }

        // Actually build the thing
        public void Build() {
            Core.Space builtRoom = Building.Instance.BuildSpace(buildShape: SelectedShape, templateSpace: previewSpace);
            builtRoom.State = SpaceState.STATIONARY;

        }

        // Moves Preview room with the mouse
        public void UpdatePreviewLocation() {
            Plane basePlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //simple ray cast from the main camera. Notice there is no range

            float distance;
            if (basePlane.Raycast(ray, out distance)) {
                Vector3 hitPoint = ray.GetPoint(distance);
                previewSpace.Move(hitPoint);
            }
            //Nyttig funktion: ElementSelection.GetPerimeterEdges()
        }

        public void SetSelectedShape(SpaceShape shape = SpaceShape.RECTANGLE) {
            SelectedShape = shape;
        }
    }
}