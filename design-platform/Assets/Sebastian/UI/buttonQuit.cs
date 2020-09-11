using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonQuit : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ButtonFunction);
    }

    private void ButtonFunction()
    {
        Application.Quit();
    }
}
