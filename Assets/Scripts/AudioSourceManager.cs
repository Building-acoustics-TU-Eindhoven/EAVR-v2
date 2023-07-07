/**
 * @author [Silvin Willemsen]
 * @email [s.willemsen@tue.nl]
 * @desc [A class to manage the audio sources]
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.Networking;
using SteamAudio;
using Vector3 = UnityEngine.Vector3;
public class AudioSourceManager : MonoBehaviour
{

    [HideInInspector]
    public int sourceIdx;

    [HideInInspector]
    public int totNumSources = 0;
    public GameObject initSource;
    public GameObject root;
    
    private GameObject curSource;

    List<GameObject> allSources;

    public List<AudioClip> clipList;
    public SourcePanelManager sourcePanelManager;
    private bool stopButtonPressedLast = false;

    private List<Vector3> ratioVec = new List<Vector3>();

    public float sourceColliderDiameter;
    public float sourceColliderHeight;

    private float origRoomWidth, origRoomHeight, origRoomDepth;
    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");

        allSources = new List<GameObject>();        

        // Used to be a call to AddSource() here, now waiting for room to be initialised and call to this function happens in SetOriginalRoomDimensions()

        sourceColliderDiameter = 2.0f * initSource.GetComponent<CapsuleCollider>().radius;
        sourceColliderHeight = 2.0f * initSource.GetComponent<CapsuleCollider>().height;

        curSource = initSource;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator AddLocalFilesToClipList()
    {
        string path = Path.Combine(Application.persistentDataPath, "Audios").Replace("\\", "/");

        if (Directory.Exists(path))
        {
            DirectoryInfo info = new DirectoryInfo(path);

            foreach (FileInfo item in info.GetFiles("*.wav"))
            {

                string address = Path.Combine(path, item.Name).Replace("\\", "/");

                UnityWebRequest AudioFiles = UnityWebRequestMultimedia.GetAudioClip(address, AudioType.WAV);

                yield return AudioFiles.SendWebRequest();
                if (AudioFiles.isNetworkError)
                {
                    // Debug.Log(AudioFiles.error);
                    // Debug.Log(path + string.Format("{0}", audioname[i]));
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(AudioFiles);
                    clipList.Add(clip);
                }

            }

        }
    }

    public GameObject GetCurSource()
    {
        return curSource;
    }

    public List<GameObject> GetAllSources()
    {
        return allSources;
    }

    public void PauseAudio()
    {
        curSource.GetComponent<SourceController>().PauseAudio();
        stopButtonPressedLast = false;
    }

    public void StopAudio()
    {
        if (stopButtonPressedLast)
        {
            foreach (GameObject source in allSources)
            {
                source.GetComponent<SourceController>().StopAudio();
            }
            stopButtonPressedLast = false;
        }
        else
        {
            curSource.GetComponent<SourceController>().StopAudio();
            stopButtonPressedLast = true;
        }
    }

    public void LoopAudio()
    {
        curSource.GetComponent<SourceController>().LoopAudio (true);
        stopButtonPressedLast = false;
    }

    public void PlayAudio()
    {
        curSource.GetComponent<SourceController>().PlayFromStart();
        stopButtonPressedLast = false;
    }

    public void GetDropdownChoice (int dropDownValue)
    {
        AudioClip selectedClip = clipList[dropDownValue];

        curSource.GetComponent<SourceController>().activeClipIdx = dropDownValue;
        curSource.GetComponent<SourceController>().SwitchClipAndPlay(clipList[dropDownValue]);

        string clipName = selectedClip.name;

        Debug.Log("my choice is " + clipName);

    }

    public void ChangeSourceIdx (int i)
    {
        sourceIdx = i;
        curSource = allSources[sourceIdx];
        sourcePanelManager.RefreshTextAndUIElements();
    }

    public void AddSource(bool init = false)
    {
        totNumSources++;
        // Duplicate currently selected source
        if (init)
        {
            
            allSources.Add(Instantiate(initSource, this.transform));
            ratioVec.Add (new Vector3 (
                Mathf.Clamp01((initSource.transform.position.x - root.transform.position.x) / (origRoomWidth * root.transform.localScale.x - sourceColliderDiameter) + 0.5f),
                Mathf.Clamp01((initSource.transform.position.y - 0.5f * sourceColliderHeight - root.transform.position.y) / (origRoomHeight * root.transform.localScale.y - sourceColliderHeight * 1.0f)),
                Mathf.Clamp01((initSource.transform.position.z - root.transform.position.z) / (origRoomDepth * root.transform.localScale.z - sourceColliderDiameter) + 0.5f)));
            Debug.Log(ratioVec[0].y);
            initSource.SetActive(false);
            allSources[0].SetActive(true);
        }
        else
        {
            ratioVec.Add(ratioVec[sourceIdx]);
            allSources.Add(Instantiate(allSources[sourceIdx], this.transform));
        }

        // GlobalFunctions.GetChildWithName(allSources[totNumSources - 1], "Source").
        // allSources[totNumSources - 1].GetComponent<SourceController>().activeSourceIdx =
            // curSource.GetComponent<SourceController>().activeSourceIdx;
        ChangeSourceIdx (totNumSources-1);

    }

    public void RemoveAllSources()
    {
        sourceIdx = totNumSources - 1;
        int i = totNumSources;
        while (i > 0)
        {
            RemoveSource (true);
            i = totNumSources;
        }
    }

    public bool RemoveSource (bool loadingNewObservation = false)
    {
        if (totNumSources == 1 && !loadingNewObservation)
        {
            Debug.Log("Can't have 0 sources!");
            return false;
        }
        --totNumSources;
        Destroy (allSources[sourceIdx]);
        ratioVec.RemoveAt(sourceIdx);
        allSources.RemoveAt(sourceIdx);

        if (sourceIdx == totNumSources)
            --sourceIdx;

        return true;
    }

    public float ConvertTodB (float linGain)
    {
        return 20 * Mathf.Log10 (linGain);
    }

    public float ConvertFromdB (float dBgain)
    {
        return Mathf.Pow(10.0f, dBgain / 20.0f);
    }

    public float IncreaseGain3dB()
    {
        SteamAudioSource source = curSource.GetComponent<SteamAudioSource>();
        float curGain = ConvertTodB (source.directMixLevel);
        float nextGain = curGain + 3.0f;
        source.directMixLevel = ConvertFromdB (nextGain);
        source.reflectionsMixLevel = source.directMixLevel;
        curSource.GetComponent<SourceController>().SetGainDb (nextGain);
        return nextGain;
    }

    public float DecreaseGain3dB()
    {
        SteamAudioSource source = curSource.GetComponent<SteamAudioSource>();
        float curGain = ConvertTodB (source.directMixLevel);
        float nextGain = curGain - 3.0f;
        source.directMixLevel = ConvertFromdB (nextGain);
        source.reflectionsMixLevel = source.directMixLevel;
        curSource.GetComponent<SourceController>().SetGainDb (nextGain);
        return nextGain;
    }

    public void SetPositionForAllSources (Vector3 vec)
    {
        foreach (Transform source in this.transform)
        {
            SetSourcePosition(vec, source);
        }

    }

    public void SetNormalisedSourcePosition (Vector3 vec)
    {
        SetSourceX(vec.x);
        SetSourceY(vec.y);
        SetSourceZ(vec.z);
    }


    public void SetSourcePosition (Vector3 vec, Transform source = null)
    {
        if (source == null)
            allSources[sourceIdx].transform.position = vec;
        else
            source.position = vec;
    }

    public void SetSourceX(float x)
    {
        ratioVec[sourceIdx] = new Vector3 (x, ratioVec[sourceIdx].y, ratioVec[sourceIdx].z);
        //ratioVec[sourceIdx] = new Vector3 (x, ratioVec[sourceIdx].y, ratioVec[sourceIdx].z);
        SetSourcePosition(new Vector3((x - 0.5f) * (origRoomWidth * root.transform.localScale.x - sourceColliderDiameter) + root.transform.position.x,
                                    allSources[sourceIdx].transform.position.y,
                                    allSources[sourceIdx].transform.position.z));

    }

    public void SetSourcesFromRoomSizeX(float roomSize)
    {
        for (int i = 0; i < totNumSources; ++i)
            SetSourcePosition(new Vector3((ratioVec[i].x - 0.5f) * (roomSize - sourceColliderDiameter) + root.transform.position.x,
                             allSources[i].transform.position.y,
                             allSources[i].transform.position.z), allSources[i].transform);

    }

    public void SetSourceY(float y)
    {
        ratioVec[sourceIdx] = new Vector3(ratioVec[sourceIdx].x, y, ratioVec[sourceIdx].z);
        SetSourcePosition(new Vector3(allSources[sourceIdx].transform.position.x,
                                    y * (origRoomHeight * root.transform.localScale.y - sourceColliderHeight) + root.transform.position.y + sourceColliderHeight * 0.5f,
                                    allSources[sourceIdx].transform.position.z));
    }

    public void SetSourcesFromRoomSizeY(float roomSize)
    {
        for (int i = 0; i < totNumSources; ++i)
            SetSourcePosition(new Vector3(allSources[i].transform.position.x,
                            ratioVec[i].y * (roomSize - sourceColliderHeight) + root.transform.position.y + sourceColliderHeight * 0.5f,
                             allSources[i].transform.position.z), allSources[i].transform);
    }


    public void SetSourceZ(float z)
    {
        ratioVec[sourceIdx] = new Vector3(ratioVec[sourceIdx].x, ratioVec[sourceIdx].y, z);
        Debug.Log (z);
        SetSourcePosition(new Vector3(allSources[sourceIdx].transform.position.x,
                            allSources[sourceIdx].transform.position.y,
                            (z - 0.5f) * (origRoomDepth * root.transform.localScale.z - sourceColliderDiameter) + root.transform.position.z));
    }

    public void SetSourcesFromRoomSizeZ(float roomSize)
    {
        Debug.Log (ratioVec[0].z);

        for (int i = 0; i < totNumSources; ++i)
            SetSourcePosition(new Vector3(allSources[i].transform.position.x,
                            allSources[i].transform.position.y,
                            (ratioVec[i].z - 0.5f) * (roomSize - sourceColliderDiameter) + root.transform.position.z), allSources[i].transform);
    }

    public void SetOriginalRoomDimensions (float roomWidth, float roomHeight, float roomDepth)
    {
        origRoomWidth = roomWidth;
        origRoomHeight = roomHeight;
        origRoomDepth = roomDepth;
        
        AddSource(true);
    }

    // Returns the 3D position of an audio source 
    public Vector3 GetSourceRatioPositionAt (int idx)
    {
        return ratioVec[idx];
    }

}
