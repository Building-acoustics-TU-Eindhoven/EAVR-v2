using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    public GeometryManager geometryManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setGeometryManagerToggleState(int wallIdx)
    {
        geometryManager.SetShouldChangeWallMaterial (wallIdx, transform.GetChild(wallIdx).GetComponent<Toggle>().isOn);
    }

    public void SelectAll (bool shouldBeSelected)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "WallToggle")
            {
                child.GetComponent<Toggle>().isOn = shouldBeSelected;
            }
        }
    }


}
