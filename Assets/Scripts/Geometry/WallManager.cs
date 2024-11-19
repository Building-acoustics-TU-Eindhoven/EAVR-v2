using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetActiveMaterial()
    {
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf)
                return child; 

        Debug.Log("NO ACTIVE WALL!");
        return null;
    }

    public int GetActiveMaterialIdx()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
                return i; 
            ++i;
        }
        Debug.Log("NO ACTIVE WALL!");
        return -1;
    }

    public string GetActiveMaterialName()
    {
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf)
                return child.name.Split('_')[1]; 

        Debug.Log("NO ACTIVE WALL!");
        return "";
    }

    public void SetActiveMaterial (int curMaterial)
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            if (i == curMaterial)
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);
            ++i;
        }
    }
}
