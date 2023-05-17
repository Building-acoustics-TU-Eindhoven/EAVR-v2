using System.Collections;
using System.Collections.Generic;

using System;
using UnityEngine;
using SteamAudio;
using Vector3 = UnityEngine.Vector3;

public class SaveLoadController : MonoBehaviour
{

    [SerializeField] private AssessmentData _AssessmentData;
    public GameObject root;
    public GameObject playerGO;
    public string recordName;

    public AudioSourceManager audioSourceManager;
    public RoomSizeManager roomSizeManager;

    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");

        DateTime dt = DateTime.Now;
        // = dt.ToString("yyyy-MM-dd") + "_Test_" + "SceneLevel";
        recordName = dt.ToString("yyyy-MM-dd") + "_Test_" + "SceneLevel";

    }
    public void ConfirmButton()
    {
        SaveIntoAssessment();
        SaveIntoJson();
        // UpdateButtons();
    }

    public void SaveIntoAssessment()
    {

        float timestamp = Time.time;
        DateTime dt = DateTime.Now;
        _AssessmentData.assessment_name = "Observation_Record";

        _AssessmentData.currentObservation.observationTime = timestamp;
        _AssessmentData.currentObservation.roomScaling = root.transform.localScale;
        _AssessmentData.currentObservation.playerXYZ = roomSizeManager.GetNormalisedPlayerPos();
        _AssessmentData.currentObservation.rotation = Camera.main.transform.rotation;

        int sourceId = 0;
        foreach (GameObject source in audioSourceManager.GetAllSpeakerSources())
        {
            // SourceData tmpSourceData = new SourceData();

            // Set source ID
            // tmpSourceData.ID = sourceId;

            // Set audio clip
            _AssessmentData.currentObservation.sourceClipNames.Add(source.GetComponent<AudioSource>().clip.name);
            // tmpSourceData.audioFileName = source.GetComponent<AudioSource>().clip.name;

            // Set gaindB
            _AssessmentData.currentObservation.sourceGaindBs.Add (audioSourceManager.ConvertTodB(source.GetComponent<SteamAudioSource>().directMixLevel));

            ++sourceId;
        }
        // Add to the list of sources in the current observation
        _AssessmentData.currentObservation.sourcePositions = audioSourceManager.GetSourceRatioPositions();

        Debug.Log("test " + _AssessmentData.observations.Count);

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


            //Console.WriteLine("file exists");
            // we read it (as a string)

            json = System.IO.File.ReadAllText(path);

            AssessmentData _assessments = new AssessmentData();

            _assessments = JsonUtility.FromJson<AssessmentData>(json);

            //   for(int i = 0; i < _assessments.observations.Count; i++){
            // _AssessmentData.observations[i] = _assessments.observations[i];
            //   }

            _assessments.observations.Add(_AssessmentData.currentObservation);
            _AssessmentData.observations = _assessments.observations;


            //json = JsonUtility.ToJson(_assessments);
            json = JsonUtility.ToJson(_AssessmentData);
            //commented
            //string assessment = JsonUtility.ToJson(_assessments);



            System.IO.File.WriteAllText(path, json);
            Debug.Log("saved assessment in old file: " + _AssessmentData.assessment_name);
        }
    }

}
