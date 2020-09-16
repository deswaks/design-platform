﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonModeModify : MonoBehaviour
{
    public MainLoop mainLoop;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ButtonFunction);
    }

    public void ButtonFunction()
    {
        mainLoop.setMode(mainLoop.modifyMode);
    }
}
