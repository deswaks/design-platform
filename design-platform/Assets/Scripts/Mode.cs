using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mode
{
    public abstract void Tick();

    public virtual void OnModeResume() { }

    public virtual void OnModePause() { }
}
