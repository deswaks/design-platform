using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Core {
    public class MoveMode : Mode {

        private static MoveMode instance;
        public static MoveMode Instance {
            get { return instance ?? (instance = new MoveMode()); }
        }

        public override void Tick() {
        }

        public override void OnModeResume() {
            if (SelectMode.Instance.selection != null) {
                SelectMode.Instance.selection.SetIsInMoveMode(true);
            }
        }

        public override void OnModePause() {
            if (SelectMode.Instance.selection != null) {
                SelectMode.Instance.selection.SetIsInMoveMode(false);
            }
        }


    }
}