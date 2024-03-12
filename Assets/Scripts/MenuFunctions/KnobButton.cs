using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using TMPro;

public class KnobButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public string parameterName;
    public string unit;
    public float minVal = -30.0f;

    public float maxVal = 24.0f;
    public float normalisedValue = 0.5f;

    public float defaultValue = 0.0f;
    public float step = 1.0f;

    public UnityEvent onValueChanged;

    public float speed = 1.0f;

    private bool dragged = false;

    private float yStart = 0;
    private float normValStart = 0;

    private float maxAngle = 150.0f;

    // Start is called before the first frame update
    void Start()
    {
        normalisedValue = ConvertTo0to1 (defaultValue);
        ApplyNormalisedValue();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButton(0))
        // {
        //     if (!mouseDown)
        //     {
        //         yStart = Input.GetAxis("Mouse Y");
        //         mouseDown = true;
            
        //     } else {
        //         float yDiff = Input.GetAxis("Mouse Y") - yStart;
        //         Debug.Log(yStart);
        //     }
        // } 
        // else if (Input.GetMouseButtonUp(0))      
        // {
        //     mouseDown = false;
        //     Debug.Log ("mouse up");
        // }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        print("begin dragging");
        yStart = eventData.position.y;
        normValStart = normalisedValue;
        
    }
 
    public void OnDrag(PointerEventData eventData)
    {
        print("Dragging");
        float yDiff = speed * (eventData.position.y - yStart);
        normalisedValue = SnapToValidValue(normValStart + yDiff * 0.001f, 0, 1);

        ApplyNormalisedValue();
    }
 
    public void ApplyNormalisedValue()
    {
        float knobAngle = (2 * maxAngle * normalisedValue) - maxAngle;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -knobAngle);
        transform.GetChild(0).localRotation = Quaternion.Euler(0.0f, 0.0f, knobAngle);

        transform.GetChild(0).GetComponent<TMP_Text>().SetText(parameterName + ":\n" + GetValue(2).ToString() + " " + unit);
        onValueChanged.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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

    public float GetValue (int decimalPlaces)
    {
        float val = (maxVal - minVal) * normalisedValue + minVal;
        float test1 =  Mathf.Pow(10, decimalPlaces);
        float test2 =  Mathf.Pow(10, 1.0f / decimalPlaces);
        return Mathf.Round(val * Mathf.Pow(10, decimalPlaces)) * 1.0f / Mathf.Pow(10, decimalPlaces);
    }

    public float ConvertTo0to1 (float notNormalisedValue)
    {
        return (notNormalisedValue - minVal) / (maxVal - minVal);
    }
}
