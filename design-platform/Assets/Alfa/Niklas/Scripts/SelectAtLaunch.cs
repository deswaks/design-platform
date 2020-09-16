using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAtLaunch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button thisButton = (Button)this.GetComponent(typeof(Button));
        thisButton.Select();
    }

}
