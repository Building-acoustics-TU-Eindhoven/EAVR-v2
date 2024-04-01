/**
 * @author [Alessia Milo]
 * @email [a.milo@tue.nl]
 * @create date 2021-04-27 14:54:46
 * @modify date 2021-04-27 14:54:46
 * @desc [Needs to be attached to the SpeakerSource GameObject, together with SwitchSource]
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SourceController : MonoBehaviour
{
    [SerializeField]
    private MenuManager menuManager;

    [SerializeField]
    private AudioSourceSelectionHandler audioSourceSelectionHandler;

    private AudioSource audioSource;

    [SerializeField]
    private int activeClipIdx = 0;  

    private float prevRMSval = 0.0f;
    private float LPcoeff = 0.95f;
    
    private Vector3 ratioVec = new Vector3(0.0f, 0.0f, 0.0f);

    private float gainDb = 0.0f;

    private bool shouldLoop = true;
    private bool shouldPause = false;

    private bool wasJustHighlighted = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSourceSelectionHandler = FindObjectOfType<AudioSourceSelectionHandler>();

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.loop = shouldLoop;
        if (shouldPause)
            audioSource.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (!menuManager.IsMenuActive())
        {
            // Convert mouse position to ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the ray hit..
            if (Physics.Raycast(ray, out hit, 100))
            {
                // .. the audioSourceRayCast (walls start with a number)..
                if (hit.transform.gameObject == audioSourceSelectionHandler.gameObject)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        audioSourceSelectionHandler.SetPositionBasedOnHit (hit);
                        wasJustHighlighted = true;
                    }
                    else if (!wasJustHighlighted)
                    {
                        audioSourceSelectionHandler.Hover (true);
                    }
                } else {
                    wasJustHighlighted = false;
                    audioSourceSelectionHandler.Hover (false);
                }
            }
        }



    }

    private void OnAudioFilterRead(float[] data, int channels)
    {

        // float audioSum = 0.0f;
        // for (int i = 0; i < data.Length; ++i)
        //     audioSum += Mathf.Sqrt(data[i] * data[i]);
        // audioSum /= data.Length;
        // float curVal = (1.0f - LPcoeff) * audioSum * Mathf.Pow(10, gainDB / 20.0f) + LPcoeff * prevRMSval;

        // prevRMSval = curVal;

    }

    public void PlayFromStart()
    {
        audioSource.Stop();
        audioSource.Play();
    }

    public void LoopAudio (bool l)
    {
        shouldLoop = l;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();


        if (!audioSource.isPlaying)
            PlayFromStart();
        audioSource.loop = l;
    }

    public void PauseAudio (bool p)
    {
        shouldPause = p;

        if (audioSource.isPlaying && p)
        {
            audioSource.Pause();
        }
        else if (!audioSource.isPlaying && !p)
        {
            audioSource.UnPause();
        }

    }
    public void StopAudio()
    {
        audioSource.Stop();
        audioSource.loop = false;
    }


    public void SwitchClipAndPlay (AudioClip sel_clip){
   
        audioSource.Stop();
        audioSource.clip = sel_clip;
        audioSource.Play();
    }

    public int GetActiveClipIdx()
    {
        return activeClipIdx;
    }

    public void SetActiveClipIdx (int i)
    {
        activeClipIdx = i;
    }

    public void SetRatioVec (Vector3 newRatioVec)
    {
        ratioVec = newRatioVec;
    }

    public Vector3 GetRatioVec() { return ratioVec; }  

    public bool GetShouldLoop() { return shouldLoop; }
    public bool GetShouldPause() { return shouldPause; }

    public float GetGainDb() { return gainDb; }
    public void SetGainDb (float dBIn) { gainDb = dBIn; }

}
