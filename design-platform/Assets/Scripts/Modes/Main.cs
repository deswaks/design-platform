using DesignPlatform.UI;
using DesignPlatform.Utils;
using StructuralAnalysis;
using UnityEngine;

namespace DesignPlatform.Modes {
    public class Main : MonoBehaviour {

        private static Main instance;
        private Mode currentMode;

        public static Main Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new GameObject("MainLoop").AddComponent<Main>()); }
        }

        void Start() {
            instance = this;
            Core.Grid.size = 1.0f;
            SetMode(SelectMode.Instance);

            // Pre load assets
            Utils.AssetUtil.LoadBundle("materials");
            Utils.AssetUtil.LoadBundle("prefabs");

            // WIDGET TEST
            SpaceScheduleWidget widgey = new SpaceScheduleWidget();
            widgey.RequestDraw();
            WallElementScheduleWidget wallElementwidget = new WallElementScheduleWidget();
            wallElementwidget.RequestDraw();            
            OpeningsScheduleWidget openingsWidget = new OpeningsScheduleWidget();
            openingsWidget.RequestDraw();
            StructuralWidget strucWidget = new StructuralWidget();
            strucWidget.RequestDraw();
            Widget_CircleDiagram circle = new Widget_CircleDiagram();
            circle.RequestDraw();
            // WIDGET TEST - SLUT

            ModuleLoader.LoadModules();
        }

        void Update() {
            currentMode.Tick();
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
    }
}