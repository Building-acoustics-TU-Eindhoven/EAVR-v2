/**
 * @author [Alessia Milo]
 * @email [a.milo@tue.nl]
 * @create date 2021-04-27 15:12:05
 * @modify date 2021-04-27 15:12:05
 * @desc [Fundamental class to generate observation data]
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public class Observation{

    public string name;
    public float observationTime;
    public Vector3 roomOrigSize;
    public Vector3 roomScaling;

    // public float RT_low;
    // public float RT_med;
    // public float RT_high;
    public string modelName;
    public List<string> acouMatList;
    public Vector3 playerXYZ;
    public Quaternion rotation;

    public List<Vector3> sourcePositions = new List<Vector3>();
    public List<string> sourceClipNames = new List<string>();
    public List<float> sourceGaindBs = new List<float>();
    public List<string> matList;
    public List<string> meshFilterNames;
    
}