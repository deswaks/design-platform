using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPlatform.Modes;
using DesignPlatform.Core;

public class SettingsManager : MonoBehaviour
{
    public void SetAllowHotkeys(bool allow) {
        DesignPlatform.Modes.Settings.allowHotkeys = allow;
    }
}
