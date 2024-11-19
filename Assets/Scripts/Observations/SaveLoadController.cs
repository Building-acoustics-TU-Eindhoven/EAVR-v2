using System.Collections;
using System.Collections.Generic;

using System;
using UnityEngine;
using UnityEngine.UI;
using SteamAudio;
using Vector3 = UnityEngine.Vector3;
using TMPro;
public class SaveLoadController : MonoBehaviour
{

    [SerializeField] private AssessmentData _AssessmentData;
    public GameObject root;
    public GameObject playerGO;
    public GameObject xrOrigin;
    public string recordName;
    public Transform viewportContent;        
    public GameObject observationItem;
    public List<GameObject> assessmentList;

    private SteamAudioManager steamAudioManager;
    public SourcePanelManager sourcePanelManager;
    public AudioSourceManager audioSourceManager;
    public RoomSizeManager roomSizeManager;
    public GeometryManager geometryManager;
    bool observationLoaded = false;
    // Start is called before the first frame update
    void Start()
    {
        steamAudioManager = GameObject.Find("Steam Audio Manager").GetComponent<SteamAudioManager>();
        root = GameObject.Find("root");

        DateTime dt = DateTime.Now;
        // = dt.ToString("yyyy-MM-dd") + "_Test_" + "SceneLevel";
        recordName = dt.ToString("yyyy-MM-dd") + "_Test_" + "SceneLevel";

        _AssessmentData.assessment_name = "Observation_Record";
        string path = Application.persistentDataPath + "/AssessmentData_" + _AssessmentData.assessment_name + ".json";

        AssessmentData _assessments = JsonUtility.FromJson<AssessmentData>(System.IO.File.ReadAllText(path));

        _AssessmentData.observations = _assessments.observations;
        Debug.Log(Application.persistentDataPath);

        UpdateButtons();
    }
    public void ConfirmButton()
    {
        SaveIntoAssessment();
        SaveIntoJson();
        UpdateButtons();
    }

    public void SaveIntoAssessment()
    {

        float timestamp = Time.time;
        DateTime dt = DateTime.Now;
        _AssessmentData.assessment_name = "Observation_Record";

        _AssessmentData.currentObservation = new Observation();
        if (_AssessmentData.observations.Count == 0)
            _AssessmentData.currentObservation.name = "0_" + recordName;
        else
        {
            string lastObsName = _AssessmentData.observations[_AssessmentData.observations.Count-1].name;
            _AssessmentData.currentObservation.name = (int.Parse(lastObsName.Substring(0, 1)) + 1).ToString() + "_" + recordName;
        }

        _AssessmentData.currentObservation.observationTime = timestamp;
        _AssessmentData.currentObservation.roomOrigSize = roomSizeManager.originalSize;
        _AssessmentData.currentObservation.roomScaling = root.transform.localScale;
        _AssessmentData.currentObservation.playerXYZ = roomSizeManager.GetNormalisedPlayerPos();
        if (playerGO.activeSelf)
            _AssessmentData.currentObservation.rotation = Camera.main.transform.rotation;
        else
            _AssessmentData.currentObservation.rotation = xrOrigin.transform.rotation;
        Debug.Log("Pos save: " + roomSizeManager.GetNormalisedPlayerPos());

        int sourceId = 0;
        foreach (SourceController source in audioSourceManager.GetAllSources())
        {
            // SourceData tmpSourceData = new SourceData();

            // Set source ID
            // tmpSourceData.ID = sourceId;

            // Set audio clip
            _AssessmentData.currentObservation.sourceClipNames.Add(source.gameObject.GetComponent<AudioSource>().clip.name);
            // tmpSourceData.audioFileName = source.GetComponent<AudioSource>().clip.name;

            // Set gaindB
            _AssessmentData.currentObservation.sourceGaindBs.Add(audioSourceManager.ConvertTodB(source.gameObject.GetComponent<SteamAudioSource>().directMixLevel));

            _AssessmentData.currentObservation.sourcePositions.Add (audioSourceManager.GetAllSources()[sourceId].GetRatioVec());

            ++sourceId;
        }
        // Add to the list of sources in the current observation
        List<int> matList = new List<int>();
        foreach (Transform child in root.transform.GetChild(0))
        {
            if (child.GetComponent<WallManager>() != null)
                matList.Add(child.GetComponent<WallManager>().GetActiveMaterialIdx());
        }

        _AssessmentData.currentObservation.matList = matList;    
        //TODO make sure that we are not leaving data out

    }

    // Initialise the assessment data
    public void InitData()
    {
        _AssessmentData = ReadRoomFromJson();
    }

    public static AssessmentData ReadRoomFromJson()
    {

        AssessmentData _assessment_buffer = new AssessmentData();

        string buffer = "Observation_Record";
        string path = Application.persistentDataPath + "/AssessmentData_" + buffer + ".json";

        string json;
        if (System.IO.File.Exists(path))
        {
            json = System.IO.File.ReadAllText(path);
            _assessment_buffer = JsonUtility.FromJson<AssessmentData>(json);
        }
        else
        {
            Debug.Log("missing info, save first");
        }

        return _assessment_buffer;
    }
    public void SaveIntoJson()
    {

        //AssessmentData _assessments = new AssessmentData();
        Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/AssessmentData_" + _AssessmentData.assessment_name + ".json";
        string json;

        // if the file does not exist
        if (!System.IO.File.Exists(path))
        {

            //if it does not exist, we create a new string with the current data from _AssessmentData
            // store the data
            _AssessmentData.observations.Add(_AssessmentData.currentObservation);

            //Convert the file
            json = JsonUtility.ToJson(_AssessmentData);

            System.IO.File.WriteAllText(path, json);

            Debug.Log("saved new assessment: " + "sessionName");
        }
        else
        {

            json = System.IO.File.ReadAllText(path);

            AssessmentData _assessments = JsonUtility.FromJson<AssessmentData>(json);

            _assessments.observations.Add(_AssessmentData.currentObservation);
            _AssessmentData.observations = _assessments.observations;

            json = JsonUtility.ToJson(_AssessmentData);

            System.IO.File.WriteAllText(path, json);
            Debug.Log("saved assessment in old file: " + _AssessmentData.assessment_name);
        }
    }

    public void UpdateButtons()
    {
        viewportContent.GetChild(0).gameObject.SetActive(true);
        for (int childIdx = 1; childIdx < viewportContent.childCount; ++childIdx)
        {
            //GameObject childToRemove = viewportContent.GetChild(childIdx).gameObject;
            //childToRemove.transform.SetParent(viewportContent.parent); // as desctruction doesn't update childCount, we change the parent before destruction
            Destroy(viewportContent.GetChild(childIdx).gameObject);
        }

        foreach (GameObject element in assessmentList)
        {
            if (element != null)
            {
                Destroy(element);
            }
        }
        assessmentList.Clear();
        int obsIndex = 0;
        int startIdx = viewportContent.childCount;
        //assessmentList.Clear();
        foreach (var obs in _AssessmentData.observations)
        {

            if (obs.name != "")
            {

                //bool isInList = check.Contains(obs.name);
                //if (!currentlyShownObservationNames.Contains(obs.name))
                //{
                GameObject newItem = Instantiate(observationItem, viewportContent);
                // newItem.transform.position = new Vector3 (newItem.transform.position.x, newItem.transform.position.y - (obsIndex + 1) * 120.0f, newItem.transform.position.z);
                // newItem.GetComponent<RectTransform>().offsetMax = new Vector2 (
                    // newItem.GetComponent<RectTransform>().position.x, 
                    // - obsIndex * 45.0f);

                // newItem.transform.SetParent(transform, false);

                //newDeleteButton.transform.SetParent(transform, false);
                //if (count == 0)
                //newItem.GetComponent<RectTransform>().position = new Vector3(observationItem.transform.position.x,
                //observationItem.transform.position.y - obsIndex * ySpacing, 
                //observationItem.transform.position.z);
                //newDeleteButton.transform.localPosition = new Vector3(40, -10 - obsIndex * ySpacing, 0);
                //else if (count >0)
                // newButton.transform.localPosition = new Vector3 (46,-10 - obsIndex*10,0);
                Button observationButton = GlobalFunctions.GetChildWithName(newItem, "ObservationButton").GetComponent<Button>();
                observationButton.GetComponentInChildren<TextMeshProUGUI>().text = obs.name; //.Remove(obs.name.Length - 7);
                observationButton.onClick.AddListener(delegate { LoadObservation(obs); });
                assessmentList.Add(observationButton.gameObject);


                //Text deleteButtonText = newDeleteButton.GetComponent<Button>().GetComponentInChildren<Text>();
                Button deleteObservationbutton = GlobalFunctions.GetChildWithName(newItem, "DeleteObservationButton").GetComponent<Button>();
                deleteObservationbutton.onClick.AddListener(delegate { DeleteObservation(obs); });
                assessmentList.Add(deleteObservationbutton.gameObject);

                //buttonObjects.Add(newButton);
                //CreateButton(AssessmentPanel.transform, obsIndex, obs.name, LogButtonPressed);
                //Debug.Log(obs.name);
                obsIndex++;
                //} else
                //{
                //    //
                //}
            }
        }
        //obsIndex = 0;
        viewportContent.GetChild(0).gameObject.SetActive(false);
        // viewportContent.GetChild(1).gameObject.SetActive(false);
        ScrollViewCallback(startIdx);

    }

    public void LoadObservation(Observation obs)
    {
        observationLoaded = false;
        steamAudioManager.enabled = false;

        foreach (SourceController source in audioSourceManager.GetAllSources())
        {
            source.gameObject.SetActive(false);
        }

        // Load room scaling (via sliders)
        roomSizeManager.SetRoomSizeSliders(obs.roomScaling);

        // Load sources //
        audioSourceManager.RemoveAllSources();
        bool clipIsFound = false;
        for (int i = 0; i < obs.sourceClipNames.Count; ++i)
        {
            audioSourceManager.AddSource();

            // Apply gaindB
            audioSourceManager.GetCurSource().GetComponent<SteamAudioSource>().directMixLevel = audioSourceManager.ConvertFromdB(obs.sourceGaindBs[i]);
            audioSourceManager.GetCurSource().GetComponent<SteamAudioSource>().reflectionsMixLevel = audioSourceManager.ConvertFromdB(obs.sourceGaindBs[i]);

            for (int ii = 0; ii < audioSourceManager.clipList.Count; ++ii)
            {
                if (audioSourceManager.clipList[ii].name == obs.sourceClipNames[i])
                {
                    audioSourceManager.GetCurSource().GetComponent<AudioSource>().clip = audioSourceManager.clipList[ii];
                    audioSourceManager.GetCurSource().GetComponent<SourceController>().SetActiveClipIdx (ii);
                    clipIsFound = true;
                    break;
                }
            }
            if (!clipIsFound)
            {
                Debug.Log("Saved clip " + obs.sourceClipNames[i] + " can't be found!");
            }

            // Set position
            sourcePanelManager.SetPositionThroughSliderVals(obs.sourcePositions[i].x, obs.sourcePositions[i].y, obs.sourcePositions[i].z);

        }
        sourcePanelManager.ChangeSourceIdx(obs.sourceClipNames.Count - 1);

        foreach (SourceController source in audioSourceManager.GetAllSources())
            source.gameObject.SetActive(true);

        // Apply player position
        if (playerGO.activeSelf)
        {
            playerGO.GetComponent<PlayerManager>().PreparePlayerPosAndRoomDimensions(obs.playerXYZ, Vector3.Scale(obs.roomOrigSize, obs.roomScaling));
            playerGO.GetComponent<PlayerManager>().ApplyPlayerPosAndRoomDimensions();
            playerGO.GetComponent<PlayerManager>().mainCamera.transform.localRotation = obs.rotation;
            playerGO.GetComponent<PlayerManager>().SetRotationAndPosition();
        } 
        else
        {
            xrOrigin.transform.localRotation = obs.rotation;
            xrOrigin.GetComponent<XRoriginManager>().TeleportToLocation (obs.playerXYZ, Vector3.Scale(obs.roomOrigSize, obs.roomScaling));
        }

        //if (root.transform.GetChild(0).childCount != obs.matList.Count)
        //{
        //    Debug.Log ("Observation doesn't match loaded model.");
        //} else {
        int iii = 0;
        foreach (Transform child in root.transform.GetChild(0))
        {
            if (child.GetComponent<WallManager>() != null)
            {
                child.GetComponent<WallManager>().SetActiveMaterial(obs.matList[iii]);
                ++iii;
            }
        }
        //}

        observationLoaded = true;
    }

    void LateUpdate() 
    {
        if (observationLoaded)
        {
            steamAudioManager.enabled = true;
            observationLoaded = false;
        }
    }
    public void DeleteObservation (Observation obs)
    {
        _AssessmentData.observations.Remove(obs);
        Debug.Log("removed observation: " + obs.name);
        string path = Application.persistentDataPath + "/AssessmentData_" + _AssessmentData.assessment_name + ".json";
        string json;
        json = JsonUtility.ToJson(_AssessmentData);
        System.IO.File.WriteAllText(path, json);
        Debug.Log("deleted record and saved into json");
        UpdateButtons();
    }
    public void ScrollViewCallback(int startIdx)
    {
        float itemHeight = viewportContent.GetChild(0).GetComponent<RectTransform>().rect.height * 1.5f;
        viewportContent.GetComponent<RectTransform>().sizeDelta = new Vector2(viewportContent.GetComponent<RectTransform>().rect.width, (viewportContent.childCount - startIdx) * itemHeight);
        int i = 0;
        for (int childIdx = startIdx; childIdx < viewportContent.childCount; ++childIdx)
        {
            Transform child = viewportContent.GetChild(childIdx);
            child.GetComponent<RectTransform>().localPosition = new Vector3(child.GetComponent<RectTransform>().localPosition.x,
                                                                             -(i) * itemHeight,
                                                                             child.GetComponent<RectTransform>().localPosition.z);
            ++i;
        }
    }


}
