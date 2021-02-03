using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonManagerSideMenu : MonoBehaviour
{
    List<UnityEngine.UI.Button> buttons;

    Color selectedColor = Color.white;
    Color normalColor;
    ColorBlock normalColorBlock;
    ColorBlock selectedColorBlock;

    // Start is called before the first frame update
    void Start()
    {
        buttons = gameObject.GetComponentsInChildren<Button>().ToList();

        ColorUtility.TryParseHtmlString("#0E091F", out normalColor);
        
        normalColorBlock = ColorBlock.defaultColorBlock;
        normalColorBlock.normalColor = normalColor;
        normalColorBlock.disabledColor = normalColor;
        
        selectedColorBlock = ColorBlock.defaultColorBlock;


        // Saves all buttons in side menu bar to list
        buttons.ForEach(bt => bt.onClick.AddListener(ChangeButtonColor));
        buttons.ForEach(bt => bt.colors = normalColorBlock);
        buttons[0].colors = selectedColorBlock;
    }

    public void ChangeButtonColor() {
        // Finds name of button that has just been clicked
        string currentName = EventSystem.current.currentSelectedGameObject.name;

        buttons.ForEach(bt => bt.colors = normalColorBlock);
        buttons.First(bt => bt.gameObject.name == currentName).colors = selectedColorBlock;
    }

    public void ChangeButtonColorByIndex(int index) {

        buttons.ForEach(bt => bt.colors = normalColorBlock);
        buttons[index].colors = selectedColorBlock;
    }
}
