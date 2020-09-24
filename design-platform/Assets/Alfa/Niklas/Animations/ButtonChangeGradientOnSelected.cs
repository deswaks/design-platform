using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;

public class ButtonChangeGradientOnSelected : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
{
    public Gradient defaultGradient;
    public Gradient selectionGradient;
    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was selected");
        UnityEngine.UI.Michsky.UI.ModernUIPack.UIGradient gradient = (UnityEngine.UI.Michsky.UI.ModernUIPack.UIGradient)gameObject.GetComponentInChildren(typeof(UnityEngine.UI.Michsky.UI.ModernUIPack.UIGradient));
        gradient.EffectGradient.colorKeys = selectionGradient.colorKeys;
        
    }
}
