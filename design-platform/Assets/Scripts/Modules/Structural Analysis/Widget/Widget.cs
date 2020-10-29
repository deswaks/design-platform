using DesignPlatform.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StructuralAnalysis {
    public class StructuralWidget : Widget {

        private readonly GameObject PrefabPanel;
        private readonly GameObject PrefabRow;

        public StructuralWidget() : base() {
            //PrefabPanel = (GameObject)Resources.Load("Scripts/Modules/Structural Analysis/Widget/Structural Widget.prefab");
            PrefabPanel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Modules/Structural Analysis/Widget/Structural Widget.prefab");
            //PrefabRow = (GameObject)Resources.Load("Scripts/Modules/Structural Analysis/Widget/Loads Overview Row.prefab");
            PrefabRow = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Modules/Structural Analysis/Widget/Loads Overview Row.prefab");

        }

        public override Object CreatePanel() {
            // This widget only uses a prefab
            return Object.Instantiate(PrefabPanel);
        }

        public override void UpdatePanel() {
            // An update method will usually go through relevant information and insert in the correct places
        }
    }

}