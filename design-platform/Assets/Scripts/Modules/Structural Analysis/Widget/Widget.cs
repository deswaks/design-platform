using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structural {
    public class StructuralWidget : global::Widget {

        private readonly GameObject PrefabPanel;
        private readonly GameObject PrefabRow;

        public StructuralWidget(GameObject host) : base(host) {
            PrefabPanel = (GameObject)Resources.Load("Scripts/Modules/Structural Analysis/Widget/Structural Widget");
            PrefabRow = (GameObject)Resources.Load("Scripts/Modules/Structural Analysis/Widget/Loads Overview Row");
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