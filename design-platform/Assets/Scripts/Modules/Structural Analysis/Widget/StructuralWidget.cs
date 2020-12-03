using DesignPlatform.UI;
using DesignPlatform.Utils;
using UnityEngine;

namespace StructuralAnalysis {

    /// <summary>
    /// The widget to display user information about the structural alanysis module
    /// </summary>
    public class StructuralWidget : Widget {

        /// <summary>The panel prefab wherein to insert data for the widget.</summary>
        private readonly GameObject PrefabPanel;

        /// <summary>The rows of data are created using this prefab,
        /// which is inserted into the widget afterwards.</summary>
        private readonly GameObject PrefabRow;

        /// <summary>
        /// Constructor for this widget class
        /// </summary>
        public StructuralWidget() : base() {
            Name = "Structural Widget";
            PrefabPanel = AssetUtil.LoadAsset<GameObject>("structuralanalysis", "Structural Widget");
            PrefabRow = AssetUtil.LoadAsset<GameObject>("structuralanalysis", "Loads Overview Row");
        }

        /// <summary>
        /// Returns a gameobject that visually represents this widget
        /// </summary>
        /// <returns>The panel that visually represents this widget.</returns>
        public override Object CreatePanel() {
            // This widget only uses a prefab
            return Object.Instantiate(PrefabPanel);
        }

        /// <summary>
        /// Updates the gameobject that visually represents this widget
        /// </summary>
        public override void UpdatePanel() {
            // An update method will usually go through relevant information and insert in the correct places
        }
    }

}