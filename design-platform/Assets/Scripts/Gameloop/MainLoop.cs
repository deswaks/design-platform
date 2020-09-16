//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MainLoop : MonoBehaviour
//{
//    public BuildMode buildMode  = new BuildMode();
//    public ModifyMode modifyMode = new ModifyMode();
//    public Mode currentMode;

//    void Start() {
//        setMode(modifyMode);
//    }

//    void Update() {
//        currentMode.Tick();
//    }

//    public void setMode(Mode mode)
//    {
//        if (currentMode != null) {
//            currentMode.OnModePause();
//        }
//        currentMode = mode;
//        if (currentMode != null) {
//            currentMode.OnModeResume();
//        }
//    }
//}
