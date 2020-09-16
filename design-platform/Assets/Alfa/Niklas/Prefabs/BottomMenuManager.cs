using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class BottomMenuManager : MonoBehaviour
{
    public GameObject menu;
    private List<Button> allButtons;
    private TextMeshProUGUI currentActionText;

    void Start()
    {
        // Finds all buttons in menu
        allButtons = menu.GetComponentsInChildren(typeof(Button)).Cast<Button>().ToList();
        currentActionText = (TextMeshProUGUI)menu.GetComponentInChildren(typeof(TextMeshProUGUI));
        allButtons.ForEach(b => b.GetComponent<Button>().onClick.AddListener(button_Clicked));
    }
    void button_Clicked()
    {
        // Finds current button
        string currentName = EventSystem.current.currentSelectedGameObject.name;
        Button currentButton = allButtons.First(b => b.name == currentName);


        currentActionText.text = currentButton.name;
    }
}
