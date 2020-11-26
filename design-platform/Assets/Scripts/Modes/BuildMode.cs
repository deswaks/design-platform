using DesignPlatform.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignPlatform.Modes {
    public class BuildMode : Mode {

        private static BuildMode instance;

        private RoomShape selectedShape = RoomShape.RECTANGLE;
        public RoomShape SelectedShape {
            get {
                return selectedShape;
            }
            set {
                selectedShape = value;
                RebuildPreview(selectedShape);
            }
        }

        // Set at runtime
        public Room previewRoom;


        public static BuildMode Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new BuildMode()); }
        }

        public BuildMode() {
            SelectedShape = RoomShape.RECTANGLE;
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
                    SelectedShape = (RoomShape)(int)SelectedShape + 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                previewRoom.Rotate(); //remember to tell this to the user when implementing tooltips
            }

            if (Input.GetKeyDown(KeyCode.L)) {
                SelectedShape = RoomShape.LSHAPE;
            }

            if (Input.GetKeyDown(KeyCode.U)) {
                SelectedShape = RoomShape.USHAPE;
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                SelectedShape = RoomShape.SSHAPE;
            }

            if (Input.GetKeyDown(KeyCode.T)) {
                SelectedShape = RoomShape.TSHAPE;
            }

            if (Input.GetKeyDown(KeyCode.D)) {
                OpeningMode.Instance.SelectedShape = OpeningShape.DOOR;
                Main.Instance.SetMode(OpeningMode.Instance);
            }

            if (Input.GetKeyDown(KeyCode.W)) {
                OpeningMode.Instance.SelectedShape = OpeningShape.WINDOW;
                Main.Instance.SetMode(OpeningMode.Instance);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Main.Instance.SetMode(SelectMode.Instance);
            }

            if (previewRoom) { UpdatePreviewLocation(); }
        }

        public override void OnModeResume() {
            if (previewRoom == null) {
                previewRoom = Building.Instance.BuildRoom(buildShape: SelectedShape, preview: true);
            }
        }

        public override void OnModePause() {
            if (previewRoom != null) previewRoom.Delete();
            previewRoom = null;
        }

        public void RebuildPreview(RoomShape SelectedShape = RoomShape.RECTANGLE) {
            if (previewRoom != null) previewRoom.Delete();
            previewRoom = Building.Instance.BuildRoom(buildShape: SelectedShape, preview: true);
        }

        // Actually build the thing
        public void Build() {
            Room builtRoom = Building.Instance.BuildRoom(buildShape: SelectedShape, templateRoom: previewRoom);
            builtRoom.State = RoomState.STATIONARY;

        }

        // Moves Preview room with the mouse
        public void UpdatePreviewLocation() {
            Plane basePlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //simple ray cast from the main camera. Notice there is no range

            float distance;
            if (basePlane.Raycast(ray, out distance)) {
                Vector3 hitPoint = ray.GetPoint(distance);
                previewRoom.Move(hitPoint);
            }
            //Nyttig funktion: ElementSelection.GetPerimeterEdges()
        }

        public void SetSelectedShape(RoomShape shape = RoomShape.RECTANGLE) {
            SelectedShape = shape;
        }
    }
}