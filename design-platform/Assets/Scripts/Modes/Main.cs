using StructuralAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Core {
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
            Grid.size = 1.0f;
            SetMode(SelectMode.Instance);


    
            // Pre load assets
            Utils.AssetUtil.LoadBundle("materials");
            Utils.AssetUtil.LoadBundle("prefabs");

            ///////////////////////////////////////////////////////////////// WIDGET TEST ///////////////////////////
            StructuralWidget strucWidget = new StructuralWidget();
            strucWidget.RequestDraw();
            RoomScheduleWidget widgey = new RoomScheduleWidget();
            widgey.RequestDraw();
            Widget_CircleDiagram circle = new Widget_CircleDiagram();
            circle.RequestDraw();
            ///////////////////////////////////////////////////////////////// WIDGET TEST - SLUT ///////////////////////////



            ModuleLoader.LoadModules();

        }

        void Update() {
            //Debug.Log(currentMode.ToString());
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