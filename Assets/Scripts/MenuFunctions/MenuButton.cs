using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerClickHandler
{
    // Button text
    [SerializeField]
    private TMP_Text buttonText;

    // Button text and (optionally) a text for when the button is disablec
    private string origText, disabledText;

    // Flag that will be set to true when the SetDisabledText() function has been called 
    private bool disabledTextHasBeenSet = false;

    // Double-click event
    public UnityEvent doubleClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
            doubleClicked.Invoke();
    }

    public void SetDisabledText (string text)
    {
        if (origText == null)    
            origText = buttonText.text;

        disabledText = text;
        disabledTextHasBeenSet = true;
    }

    public void SetEnabled (bool e)
    {
        gameObject.GetComponent<Button>().interactable = e;
        if (disabledTextHasBeenSet)
        {
            buttonText.text = e ? origText : disabledText;
        }
    }
}
