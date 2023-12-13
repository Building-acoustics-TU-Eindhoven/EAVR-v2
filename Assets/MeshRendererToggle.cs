using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererToggle : MonoBehaviour
{
    public GameObject xrSimulator;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = xrSimulator.activeSelf;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
