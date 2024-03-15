using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using TMPro;
using System.Threading;

[RequireComponent(typeof(Button))]
public class KnobButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    
    public string parameterName;
    public string unit;
    public float minVal = 0.0f;

    public float maxVal = 1.0f;
    public float normalisedValue = 0.5f;

    public float defaultValue = 0.5f;
    public float step = 0.01f;

    public UnityEvent onValueChanged;

    public float speed = 1.0f;

    private bool dragged = false;

    private float yStart = 0;
    private float normValStart = 0;

    private float maxAngle = 150.0f;

    private bool dragging = false;

    private bool isCoroutineRunning = false;

    private Coroutine TimerCoroutine;
    private string formatString = "{0:0.00}";
    // Start is called before the first frame update
    public void Start()
    {
        GetValueFormattingFromStep();
    }

    private void GetValueFormattingFromStep()
    {
        // Reset formatstring
        formatString = "";
    
        // Get decimal places of the step variable to determine the string formatting
        decimal decimalStep = ((decimal)step);

        // Remove zeropadding
        string decimalStepString = decimalStep.ToString();
        while (decimalStepString.EndsWith("0"))
        {
            decimalStepString = decimalStepString.Substring(0,decimalStepString.Length-1);
        }
        decimalStep = decimal.Parse(decimalStepString);
    
        // Count decimals
        int count = BitConverter.GetBytes(decimal.GetBits(decimalStep)[3])[2];

        // Build formatString
        // 0 decimals: "{0:0}"
        // 1 decimal: "{0:0.0}"
        // 2 decimals: "(0:0.00}"
        formatString = "{0:0";
        if (count != 0)
            formatString += ".";
        for (int i = 0; i < count; ++i)
        {
            formatString += "0";
        }
        formatString += "}";
    }

    public void OnEnable()
    {
        // Stop any existing timers
        if (TimerCoroutine != null)
            StopCoroutine (TimerCoroutine);

        ShowValueFor (1.0f);
    }

    public void SetDefaultValue (float value)
    {
        defaultValue = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Get the current mouse y position
        yStart = eventData.position.y;

        // Get the current normalised value
        normValStart = normalisedValue;
    }
 
    public void OnDrag(PointerEventData eventData)
    {
        // Get the difference in the y position of the mouse cursor.. 
        float yDiff = speed * (eventData.position.y - yStart);

        // And apply it to the knob
        SetNormalisedValue (normValStart + yDiff * 0.001f);
    }

    // Set value based on a non-normalised input
    public void SetNonNormalisedValue (float val, bool sendChangeMessage = true)
    {
        SetNormalisedValue (ConvertTo0to1 (val), sendChangeMessage);
    }

    // Set a value  Don't send change message if another source is selected and the values are simply applied to the knob
    public void SetNormalisedValue (float val, bool sendChangeMessage = true)
    {

        // Make sure that the normalisedValue is a valid value (both between 0 and 1, and based on step)
        normalisedValue = SnapToValidValue (val, 0, 1);

        // Change angle of the knob
        float knobAngle = (2 * maxAngle * normalisedValue) - maxAngle;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -knobAngle);

        // Rotate the child of the knob (the text) the other way so it stays upright
        transform.GetChild(0).localRotation = Quaternion.Euler(0.0f, 0.0f, knobAngle);

        // Set the text of the knob
        transform.GetChild(0).GetComponent<TMP_Text>().SetText(String.Format(formatString, GetValue()) + " " + unit);

        if (gameObject.activeInHierarchy)
            ShowValueFor (1.0f);

        if (sendChangeMessage)
            onValueChanged.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {        
        // A click should show the value
        ShowValueFor (1.0f);
        
        // Double click will set the knob to its default value
        if (pointerEventData.clickCount % 2 == 0)
        {
            SetNonNormalisedValue (defaultValue);
        }
    }

    public void ShowValueFor (float sec)
    {
        // Show value for 
        transform.GetChild(0).GetComponent<TMP_Text>().SetText(String.Format(formatString, GetValue()) + " " + unit);

        // Stop any existing timers
        if (TimerCoroutine != null)
            StopCoroutine (TimerCoroutine);

        // Start the timer to show the parameter name after one second
        TimerCoroutine = StartCoroutine(Timer (sec, 
        ()=>{
            transform.GetChild(0).GetComponent<TMP_Text>().SetText(parameterName);        
        }));
    }

    public IEnumerator Timer (float sec, Action callback)
    {
        // Wait for sec seconds and call the callback
        yield return new WaitForSeconds (sec);
        callback();
    }

    public float SnapToValidValue (float val, float min, float max)
    {
        if (val >= max)
            return max;
        else if (val <= min)
            return min;
        else 
            return Mathf.Round (val * (maxVal - minVal) / step) * step / (maxVal - minVal);
    }

    public float GetValue()
    {
        float val = (maxVal - minVal) * normalisedValue + minVal;
        return val;
        // float test1 =  Mathf.Pow(10, decimalPlaces);
        // float test2 =  Mathf.Pow(10, 1.0f / decimalPlaces);
        // return Mathf.Round(val * Mathf.Pow(10, decimalPlaces)) * 1.0f / Mathf.Pow(10, decimalPlaces);

    }

    public float ConvertTo0to1 (float notNormalisedValue)
    {
        return (notNormalisedValue - minVal) / (maxVal - minVal);
    }

}
