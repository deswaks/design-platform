﻿using StructuralAnalysis;
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
            ModuleLoader.LoadModules();
            instance = this;
            Grid.size = 1.0f;
            SetMode(SelectMode.Instance);

            ///////////////////////////////////////////////////////////////// WIDGET TEST ///////////////////////////
            StructuralWidget strucWidget = new StructuralWidget();
            strucWidget.RequestDraw();
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