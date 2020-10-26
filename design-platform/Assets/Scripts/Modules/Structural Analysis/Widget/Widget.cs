using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Structural {
    public class Widget : global::Widget {

        private readonly GameObject PrefabRow;

        public Widget(GameObject host) : base(host) {
            Size = (1, 1);
            PrefabRow = (GameObject)Resources.Load("Scripts/Modules/Structural Analysis/Widget/Loads Overview Row");
            PrefabPanel = (GameObject)Resources.Load("Scripts/Modules/Structural Analysis/Widget/Structural Widget");
        }
        public override void InsertInDashboard() {
            // Skriv sig selv ind i dashboard
        }
        public override GameObject DrawPanel() {
            Panel = Object.Instantiate(PrefabPanel, Host.transform.position, Host.transform.rotation);
            // Insert something into the rows
            return Panel;
        }
        public override void UpdatePanel() {
            //Go through the information and insert in the correct places
        }
    }
}