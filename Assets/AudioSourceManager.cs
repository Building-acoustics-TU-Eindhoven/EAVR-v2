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
    
    private GameObject curSpeakerSource;

    List<GameObject> allSources;

    public List<AudioClip> clipList;
    public SourcePanelManager sourcePanelManager;
    private bool stopButtonPressedLast = false;

    private List<Vector3> ratioVec = new List<Vector3>();

    private float sourceColliderDiameter;
    private float sourceColliderHeight;

    private float origRoomWidth, origRoomHeight, origRoomDepth;
    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");

        allSources = new List<GameObject>();        

        // Used to be a call to AddSource() here, now waiting for room to be initialised and call to this function happens in SetOriginalRoomDimensions()

        // GlobalFunctions.GetChildWithName(allSources[sourceIdx], "JaneAvatar").SetActive(true);
        // GlobalFunctions.GetChildWithName(allSources[sourceIdx], "MaleAvatar").SetActive(false);
        // GlobalFunctions.GetChildWithName(allSources[sourceIdx], "SourceSimple").SetActive(false);

        sourceColliderDiameter = 2.0f * initSource.GetComponent<CapsuleCollider>().radius;
        sourceColliderHeight = 2.0f * initSource.GetComponent<CapsuleCollider>().height;

        curSpeakerSource = initSource;
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

                // Debug.Log(address);
                // UnityWebRequest AudioFiles = UnityWebRequestMultimedia.GetAudioClip("file://" + address, AudioType.WAV);
                UnityWebRequest AudioFiles = UnityWebRequestMultimedia.GetAudioClip(address, AudioType.WAV);
                // UnityWebRequest AudioFiles = UnityWebRequestMultimedia.GetAudioClip("/Users/amilo/Library/Application Support/DefaultCompany/Attempt/Audios/Harvard_L69_S01_5.wav", AudioType.WAV);

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

    public GameObject GetCurSpeakerSource()
    {
        return curSpeakerSource;
    }

    public void PauseAudio()
    {
        curSpeakerSource.GetComponent<SourceController>().PauseAudio();
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
            curSpeakerSource.GetComponent<SourceController>().StopAudio();
            stopButtonPressedLast = true;
        }
    }

    public void LoopAudio()
    {
        curSpeakerSource.GetComponent<SourceController>().LoopAudio (true);
        stopButtonPressedLast = false;
    }

    public void PlayAudio()
    {
        curSpeakerSource.GetComponent<SourceController>().PlayFromStart();
        stopButtonPressedLast = false;
    }

    public void GetDropdownChoice (int dropDownValue)
    {
        AudioClip selectedClip = clipList[dropDownValue];

        curSpeakerSource.GetComponent<SourceController>().activeSourceIdx = dropDownValue;
        curSpeakerSource.GetComponent<SourceController>().SwitchClipAndPlay(clipList[dropDownValue]);

        string clipName = selectedClip.name;

        Debug.Log("my choice is " + clipName);

    }

    public void ChangeSourceIdx (int i)
    {
        sourceIdx = i;
        curSpeakerSource = allSources[sourceIdx];
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
                Mathf.Clamp01((initSource.transform.position.x - root.transform.position.x) / (origRoomWidth * root.transform.localScale.x)),
                Mathf.Clamp01((initSource.transform.position.y - 0.5f * sourceColliderHeight - root.transform.position.y) / (origRoomHeight * root.transform.localScale.y - sourceColliderHeight * 1.0f)),
                Mathf.Clamp01((initSource.transform.position.z - root.transform.position.z) / (origRoomDepth * root.transform.localScale.z))));
            Debug.Log(ratioVec[0].y);
            initSource.SetActive(false);
            ChangeSourceIdx(0);

        }
        else
        {
            ratioVec.Add(ratioVec[sourceIdx]);
            allSources.Add(Instantiate(allSources[sourceIdx], this.transform));
        }

        // GlobalFunctions.GetChildWithName(allSources[totNumSources - 1], "SpeakerSource").
        Debug.Log("ActiveSourceIdx = " + allSources[totNumSources - 1].GetComponent<SourceController>().activeSourceIdx);
        // allSources[totNumSources - 1].GetComponent<SourceController>().activeSourceIdx =
            // curSpeakerSource.GetComponent<SourceController>().activeSourceIdx;
    }

    public bool RemoveSource()
    {
        if (totNumSources == 1)
        {
            Debug.Log("Can't have 0 sources!");
            return false;
        }
        --totNumSources;
        Destroy (allSources[sourceIdx]);
        allSources.RemoveAt(sourceIdx);

        if (sourceIdx == totNumSources)
            --sourceIdx;

        return true;
    }

    public float IncreaseGain3dB()
    {
        SteamAudioSource source = curSpeakerSource.GetComponent<SteamAudioSource>();
        float curGain = 20 * Mathf.Log10(source.directMixLevel);
        float nextGain = curGain + 3.0f;
        source.directMixLevel = Mathf.Pow(10.0f, nextGain / 20.0f);
        source.reflectionsMixLevel = source.directMixLevel;
        curSpeakerSource.GetComponent<SourceController>().SetGainDb (nextGain);
        return nextGain;
    }

    public float DecreaseGain3dB()
    {
        SteamAudioSource source = curSpeakerSource.GetComponent<SteamAudioSource>();
        float curGain = 20 * Mathf.Log10(source.directMixLevel);
        float nextGain = curGain - 3.0f;
        source.directMixLevel = Mathf.Pow(10.0f, nextGain / 20.0f);
        source.reflectionsMixLevel = source.directMixLevel;
        curSpeakerSource.GetComponent<SourceController>().SetGainDb (nextGain);
        return nextGain;
    }

    public void SetPositionForAllSources (Vector3 vec)
    {
        foreach (Transform source in this.transform)
        {
            SetPosition(source, vec);
        }

    }

    public void SetPosition(Transform source, Vector3 vec)
    {
        source.position = vec;
    }

    public void SetSpeakerX(float x)
    {
        ratioVec[sourceIdx] = new Vector3 (x, ratioVec[sourceIdx].y, ratioVec[sourceIdx].z);
        //ratioVec[sourceIdx] = new Vector3 (x, ratioVec[sourceIdx].y, ratioVec[sourceIdx].z);
        SetPosition(allSources[sourceIdx].transform, 
                        new Vector3((x - 0.5f) * (origRoomWidth * root.transform.localScale.x - sourceColliderDiameter) + root.transform.position.x,
                                    allSources[sourceIdx].transform.position.y,
                                    allSources[sourceIdx].transform.position.z));

    }

    public void SetSpeakersFromRoomSizeX(float roomSize)
    {
        for (int i = 0; i < totNumSources; ++i)
            SetPosition(allSources[i].transform,
                        new Vector3((ratioVec[i].x - 0.5f) * (roomSize - sourceColliderDiameter) + root.transform.position.x,
                             allSources[i].transform.position.y,
                             allSources[i].transform.position.z));

    }

    public void SetSpeakerY(float y)
    {
        ratioVec[sourceIdx] = new Vector3(ratioVec[sourceIdx].x, y, ratioVec[sourceIdx].z);
        SetPosition(allSources[sourceIdx].transform,
                        new Vector3(allSources[sourceIdx].transform.position.x,
                                    y * (origRoomHeight * root.transform.localScale.y - sourceColliderHeight) + root.transform.position.y + sourceColliderHeight * 0.5f,
                                    allSources[sourceIdx].transform.position.z));
    }

    public void SetSpeakersFromRoomSizeY(float roomSize)
    {
        for (int i = 0; i < totNumSources; ++i)
            SetPosition(allSources[i].transform,
                        new Vector3(allSources[i].transform.position.x,
                            ratioVec[i].y * (roomSize - sourceColliderHeight) + root.transform.position.y + sourceColliderHeight * 0.5f,
                             allSources[i].transform.position.z));
    }


    public void SetSpeakerZ(float z)
    {
        ratioVec[sourceIdx] = new Vector3(ratioVec[sourceIdx].x, ratioVec[sourceIdx].y, z);
        Debug.Log (z);
        SetPosition(allSources[sourceIdx].transform,
                        new Vector3(allSources[sourceIdx].transform.position.x,
                            allSources[sourceIdx].transform.position.y,
                            (z - 0.5f) * (origRoomDepth * root.transform.localScale.z - sourceColliderDiameter) + root.transform.position.z));
    }

    public void SetSpeakersFromRoomSizeZ(float roomSize)
    {
        Debug.Log (ratioVec[0].z);

        for (int i = 0; i < totNumSources; ++i)
            SetPosition(allSources[i].transform,
                        new Vector3(allSources[i].transform.position.x,
                            allSources[i].transform.position.y,
                            (ratioVec[i].z - 0.5f) * (roomSize - sourceColliderDiameter) + root.transform.position.z));
    }

    public void SetOriginalRoomDimensions (float roomWidth, float roomHeight, float roomDepth)
    {
        origRoomWidth = roomWidth;
        origRoomHeight = roomHeight;
        origRoomDepth = roomDepth;
        
        AddSource(true);
    }
}
