using DesignPlatform.UI;
using DesignPlatform.Utils;
using StructuralAnalysis;
using UnityEngine;

namespace DesignPlatform.Modes {
    
    /// <summary>
    /// The main gameloop of the design platform that calls every update for the custom
    /// classes and delegates  user input to the sub-modes.
    /// </summary>
    public class Main : MonoBehaviour {

        /// <summary>The single instance that exists of this singleton class.</summary>
        private static Main instance;

        /// <summary>The mode currently running.</summary>
        private Mode currentMode;

        /// <summary>The single instance that exists of this singleton class.</summary>
        public static Main Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new GameObject("MainLoop").AddComponent<Main>()); }
        }

        /// <summary>Main initializer to run when the program starts.</summary>
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

        /// <summary>Update function calls each of the individual modes when they are active.</summary>
        void Update() {
            currentMode.Tick();
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
        
    }
}