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
    //private AudioClip clip;

    private AudioSource audioSource;
    public int activeClipIdx { get; set; }

    public float gainDB = 0.0f;
    private float prevRMSval = 0.0f;
    private float LPcoeff = 0.95f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {

        float audioSum = 0.0f;
        for (int i = 0; i < data.Length; ++i)
            audioSum += Mathf.Sqrt(data[i] * data[i]);
        audioSum /= data.Length;
        float curVal = (1.0f - LPcoeff) * audioSum * Mathf.Pow(10, gainDB / 20.0f) + LPcoeff * prevRMSval;

        prevRMSval = curVal;

    }

    public void SetGainDb (float dBIn)
    {
        gainDB = dBIn;
    }

    public void PlayFromStart()
    {
        audioSource.Stop();
        audioSource.Play();
    }

    public void LoopAudio (bool val)
    {
        if (!audioSource.isPlaying)
            PlayFromStart();
        audioSource.loop = val;
    }

    public void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else if (!audioSource.isPlaying)
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
        //audio.PlayOneShot(clip);
    }
}
