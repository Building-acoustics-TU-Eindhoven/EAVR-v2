using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GeometryManager : MonoBehaviour
{
    int curMaterial = 0;
    public TMP_Dropdown dropdown;
    public GameObject audioSourceLocation;
    public AudioSource audioSource;

    int totMaterials = -1;
    
    bool[] shouldChangeWallMaterial = new bool [6] { true, true, true, true, true, true };
    float scale = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        SetActiveMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveMaterial()
    {
        int j = 0;
        foreach (Transform child in transform)
        {
            if (child.tag == "Wall" && shouldChangeWallMaterial[j])
            {
                if (totMaterials == -1)
                    totMaterials = child.childCount;
                for (int i = 0; i < totMaterials; ++i)
                {
                    if (i == curMaterial)
                        child.GetChild(i).gameObject.SetActive (true);
                    else
                        child.GetChild(i).gameObject.SetActive (false);
                }
            }
            ++j;
        }

    }

    public void SetShouldChangeWallMaterial (int wall, bool shouldChange)
    {
        shouldChangeWallMaterial[wall] = shouldChange;
    }

    public void ChangeMaterial()
    {
        curMaterial = (curMaterial + 1) % totMaterials;
        SetActiveMaterial();          
    }

    public void ChangeMaterial (int mat)
    {
        // first option is "Choose material..
        if (mat == 0)
            return;

        curMaterial = mat-1;
        SetActiveMaterial();          
    }


    public void ChangeSize (float multiplier)
    {
        scale = multiplier;
        this.transform.localScale = new Vector3 (scale, scale, scale);
        audioSource.transform.position = audioSourceLocation.transform.position;
    }

    public void RegenerateDynamicObjects()
    {
        //...?
    }
}
