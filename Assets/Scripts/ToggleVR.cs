using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ToggleVR : MonoBehaviour
{
    public enum VRsetup
    {
        ScreenBased,
        VR,
        VRSimulator
    }

    public VRsetup VRSetup;
    private VRsetup curVRsetup;

    public GameObject player;
    public GameObject XROrigin;
    public GameObject XRDeviceSimulator;

    private bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        curVRsetup = VRSetup;
        started = true;
    }

    // Update is called once per frame
    void OnValidate()
    {
        if (!started)
            return;
        if (curVRsetup != VRSetup)
        {
            ChangeVRsettings();
            curVRsetup = VRSetup;
        }
    }

    void ChangeVRsettings()
    {
        switch (VRSetup)
        {
            case VRsetup.ScreenBased:
                player.SetActive(true);
                XROrigin.SetActive(false);
                XRDeviceSimulator.SetActive(false);
                break;

            case VRsetup.VR:
                player.SetActive(false);
                XROrigin.SetActive(true);
                XRDeviceSimulator.SetActive(false);
                break;

            case VRsetup.VRSimulator:
                player.SetActive(false);
                XROrigin.SetActive(true);
                XRDeviceSimulator.SetActive(true);
                break;
        }

    }

}
