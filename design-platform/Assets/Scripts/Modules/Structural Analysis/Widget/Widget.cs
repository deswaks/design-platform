using DesignPlatform.Core;
using DesignPlatform.Utils;
using UnityEngine;

namespace StructuralAnalysis {
    public class StructuralWidget : Widget {

        private readonly GameObject PrefabPanel;
        private readonly GameObject PrefabRow;

        public StructuralWidget() : base() {
            Name = "Structural Widget";
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("structuralanalysis", "Structural Widget");
            PrefabRow = AssetUtil.LoadAsset<GameObject>("structuralanalysis", "Loads Overview Row");
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