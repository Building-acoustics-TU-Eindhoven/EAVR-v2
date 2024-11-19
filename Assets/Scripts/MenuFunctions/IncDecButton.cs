using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class IncDecButton : MonoBehaviour
{    
    // Name of the parameter that will be shown in text before the value
    public string parameterName;

    [SerializeField]
    private const int defaultValue = 1;
    [SerializeField]
    private int minValue = 1;
    [SerializeField]
    private int maxValue = 1;
    private int currentValue = 1;  
    public int step = 1;

    // When the current value is changed
    public UnityEvent onValueChanged;
    
    // Start is called before the first frame update
    void Start()
    {
        // At the start, set the current value to the default value
        SetCurrentValue (defaultValue);
    }

    // Set the current value and update the text. Return whether the value that went in got changed by the snap to valid value function
    public bool SetCurrentValue (int val)
    {
        bool valueChanged = currentValue != val;

        currentValue = SnapToValidValue (val);
        foreach (Transform child in transform)
            if (child.GetComponent<TMP_Text>())
                child.GetComponent<TMP_Text>().SetText(parameterName + ": " + currentValue);
        

        if (valueChanged)      
            onValueChanged.Invoke();

        return currentValue == val;
    }

    // Decrement value. Called when the left half button is pressed.
    public void DecrementValue()
    {
        SetCurrentValue (currentValue - step);
    }

    // Increment value. Called when the right half button is pressed.
    public void IncrementValue()
    {
        SetCurrentValue (currentValue + step);
    }

    public void SetMaxValue (int val)
    {
        maxValue = val;
    }

    // When we want to reduce the range of the possible values.
    public bool DecrementMaxValue()
    {
        if (maxValue == minValue)
            return false;

        SetMaxValue (maxValue - 1);
        return true;
    }

    // When we want to increase the range of the possible values.
    public void IncrementMaxValue()
    {
        SetMaxValue (maxValue + 1);
    }

    // Make sure that the value we want to apply is a valid value. Includes wrapping around when going outside the range 
    public int SnapToValidValue (int val)
    {
        if (val > maxValue)
            return minValue;
        else if (val < minValue)
            return maxValue;
        else 
            return val;
    }

    public int GetCurrentValue()
    {
        return currentValue;
    }
}
