/**
 * @author [Silvin Willemsen]
 * @email [s.willemsen@tue.nl]
 * @desc [A class to manage the audio sources]
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    private SourceController curSource;

    List<SourceController> allSources = new List<SourceController>();

    public List<AudioClip> clipList;
    public SourcesMenuManager sourcesMenuManager;

    public float sourceColliderDiameter;
    // Determines what to subtract from the roomheight when calculating the ratio for Y
    public float sourceColliderHeight;

    // The origin of the source (determines what is 0 height)
    public float sourceColliderY;

    private float origRoomWidth, origRoomHeight, origRoomDepth;

    private int audioSourceIdxName = 0;

    private bool prepared = false;

    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");

        // Used to be a call to AddSource() here, now waiting for room to be initialised and call to this function happens in SetOriginalRoomDimensions()

        sourceColliderDiameter = 2.0f * initSource.GetComponent<CapsuleCollider>().radius;
        sourceColliderHeight = initSource.GetComponent<CapsuleCollider>().height;
        sourceColliderY = initSource.transform.position.y;

        curSource = initSource.GetComponent<SourceController>();
        GetDropdownChoice (5);

        prepared = true;

        // This is where we add the first source to the application!
        AddSource();
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

    public SourceController GetCurSource()
    {
        return curSource;
    }

    public List<SourceController> GetAllSources()
    {
        return allSources;
    }

    public void PauseAudioFromToggle (Toggle toggle)
    {
        curSource.PauseAudio (!toggle.isOn);
    }

    public void StopAudio()
    {
 
        curSource.StopAudio();
    }

    public void StopAllAudio()
    {
        foreach (SourceController source in allSources)
        {
            source.StopAudio();
        }
    }

    public void LoopAudioFromToggle (Toggle toggle)
    {
        curSource.LoopAudio (toggle.isOn);
    }

    public void PlayAudio()
    {
        curSource.PlayFromStart();
    }

    public void GetDropdownChoice (int dropDownValue)
    {
        AudioClip selectedClip = clipList[dropDownValue];

        curSource.SetActiveClipIdx (dropDownValue);
        curSource.SwitchClipAndPlay(clipList[dropDownValue]);

        string clipName = selectedClip.name;

        Debug.Log("my choice is " + clipName);

    }

    public void ChangeSourceIdx (int i)
    {
        sourceIdx = i;
        curSource = allSources[sourceIdx];
        sourcesMenuManager.RefreshTextAndUIElements (curSource);
    }

    public void AddSource()
    {
        if (!prepared)
            return;

        totNumSources++;
        // Duplicate currently selected source
        if (allSources.Count == 0)
        {
            
            allSources.Add(Instantiate(initSource, this.transform).GetComponent<SourceController>());
            allSources[0].SetRatioVec (new Vector3 (
                Mathf.Clamp01((initSource.transform.position.x - root.transform.position.x) / (origRoomWidth * root.transform.localScale.x - sourceColliderDiameter) + 0.5f),
                Mathf.Clamp01((initSource.transform.position.y - sourceColliderY - root.transform.position.y) / (origRoomHeight * root.transform.localScale.y - sourceColliderHeight)),
                Mathf.Clamp01((initSource.transform.position.z - root.transform.position.z) / (origRoomDepth * root.transform.localScale.z - sourceColliderDiameter) + 0.5f)));

            sourcesMenuManager.SetDefaultKnobValues (allSources[0].GetRatioVec());
            initSource.SetActive(false);
            allSources[0].gameObject.SetActive(true);
            allSources[0].gameObject.name = "Audio Source 0";
        }
        else
        {
            allSources.Add(Instantiate(allSources[sourceIdx], this.transform));
            allSources[allSources.Count-1].LoopAudio (allSources[sourceIdx].GetShouldLoop());
            allSources[allSources.Count-1].PauseAudio (allSources[sourceIdx].GetShouldPause());
            allSources[allSources.Count-1].SetRatioVec (allSources[sourceIdx].GetRatioVec());
            allSources[allSources.Count-1].name = "Audio Source " + audioSourceIdxName.ToString();
        }
        ++audioSourceIdxName;

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
        StartCoroutine (SafelyRemoveSource (sourceIdx));

        if (sourceIdx == totNumSources)
            --sourceIdx;

        return true;
    }

    private IEnumerator SafelyRemoveSource (int idx)
    {
        allSources[idx].gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        while (idx >= allSources.Count)
        {
            --idx;
        }

        Destroy (allSources[idx].gameObject);
        allSources.RemoveAt(idx);
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
        SteamAudioSource source = curSource.gameObject.GetComponent<SteamAudioSource>();
        float curGain = ConvertTodB (source.directMixLevel);
        float nextGain = curGain + 3.0f;
        source.directMixLevel = ConvertFromdB (nextGain);
        source.reflectionsMixLevel = source.directMixLevel;
        // curSource.SetGainDb (nextGain);
        return nextGain;
    }

    public float DecreaseGain3dB()
    {
        SteamAudioSource source = curSource.GetComponent<SteamAudioSource>();
        float curGain = ConvertTodB (source.directMixLevel);
        float nextGain = curGain - 3.0f;
        source.directMixLevel = ConvertFromdB (nextGain);
        source.reflectionsMixLevel = source.directMixLevel;
        // curSource.SetGainDb (nextGain);
        return nextGain;
    }

    public void SetGainDbFromKnob (KnobButton knob)
    {
        SteamAudioSource source = curSource.gameObject.GetComponent<SteamAudioSource>();
        source.directMixLevel = ConvertFromdB (knob.GetValue());
        source.reflectionsMixLevel = source.directMixLevel;
        // curSource.SetGainDb (knob.GetValue());
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

    public void SetSourceXfromKnob (KnobButton knob)
    {
        if (!prepared)
            return;

        SetSourceX (knob.GetValue());
    }

    public void SetSourceYfromKnob (KnobButton knob)
    {
        if (!prepared)
            return;

        SetSourceY (knob.GetValue());
    }

    public void SetSourceZfromKnob (KnobButton knob)
    {
        if (!prepared)
            return;

        SetSourceZ (knob.GetValue());
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
        // Get current ratio vector
        Vector3 ratioVec = allSources[sourceIdx].GetRatioVec();

        // Set the ratio of the source based on the new X value
        allSources[sourceIdx].SetRatioVec(new Vector3 (x, ratioVec.y, ratioVec.z));

        SetSourcePosition(new Vector3((x - 0.5f) * (origRoomWidth * root.transform.localScale.x - sourceColliderDiameter) + root.transform.position.x,
                                    allSources[sourceIdx].transform.position.y,
                                    allSources[sourceIdx].transform.position.z));

    }

    public void SetSourcesFromRoomSizeX(float roomSize)
    {

        for (int i = 0; i < totNumSources; ++i)
            SetSourcePosition(new Vector3((allSources[i].GetRatioVec().x - 0.5f) * (roomSize - sourceColliderDiameter) + root.transform.position.x,
                             allSources[i].transform.position.y,
                             allSources[i].transform.position.z), allSources[i].transform);

    }

    public void SetSourceY(float y)
    {
        // Get current ratio vector
        Vector3 ratioVec = allSources[sourceIdx].GetRatioVec();

        // Set the ratio of the source based on the new Y value
        allSources[sourceIdx].SetRatioVec(new Vector3 (ratioVec.x, y, ratioVec.z));

        SetSourcePosition(new Vector3(allSources[sourceIdx].transform.position.x,
                                    y * (origRoomHeight * root.transform.localScale.y - sourceColliderHeight) + root.transform.position.y + sourceColliderY,
                                    allSources[sourceIdx].transform.position.z));
    }

    public void SetSourcesFromRoomSizeY(float roomSize)
    {
        for (int i = 0; i < totNumSources; ++i)
            SetSourcePosition(new Vector3(allSources[i].transform.position.x,
                            allSources[i].GetRatioVec().y * (roomSize - sourceColliderHeight) + root.transform.position.y + sourceColliderY,
                             allSources[i].transform.position.z), allSources[i].transform);
    }


    public void SetSourceZ(float z)
    {
        // Get current ratio vector
        Vector3 ratioVec = allSources[sourceIdx].GetRatioVec();

        // Set the ratio of the source based on the new X value
        allSources[sourceIdx].SetRatioVec(new Vector3 (ratioVec.x, ratioVec.y, z));

        SetSourcePosition(new Vector3(allSources[sourceIdx].transform.position.x,
                            allSources[sourceIdx].transform.position.y,
                            (z - 0.5f) * (origRoomDepth * root.transform.localScale.z - sourceColliderDiameter) + root.transform.position.z));
    }
 
    public void SetSourcesFromRoomSizeZ(float roomSize)
    {
        for (int i = 0; i < totNumSources; ++i)
            SetSourcePosition(new Vector3(allSources[i].transform.position.x,
                            allSources[i].transform.position.y,
                            (allSources[i].GetRatioVec().z - 0.5f) * (roomSize - sourceColliderDiameter) + root.transform.position.z), allSources[i].transform);
    }

    public void SetOriginalRoomDimensions (Vector3 roomSize)
    {
        origRoomWidth = roomSize.x;
        origRoomHeight = roomSize.y;
        origRoomDepth = roomSize.z;
    }

    public bool IsPrepared() { return prepared; }
}
