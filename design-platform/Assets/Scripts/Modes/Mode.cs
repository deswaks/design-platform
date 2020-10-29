﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPlatform.Core {
    public abstract class Mode {
        public abstract void Tick();

        public virtual void OnModeResume() { }

        public virtual void OnModePause() { }
    }
}