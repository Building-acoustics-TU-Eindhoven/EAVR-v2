using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SteamAudio;
using Vector3 = UnityEngine.Vector3;

public class SourcePanelManager : MonoBehaviour
{

    public AudioSourceManager sourceManager;
    public GameObject root;
    public RoomSizeManager roomSizeManager;

    public TMP_Dropdown m_dropdown;

    public GameObject sourceSelector;
    private TMP_Text sourceIdxText;

    public GameObject gainPanel;
    private TMP_Text gainText;

    public GameObject sourcePosition;
    private TMP_InputField xText, yText, zText;

    private string activeSourceNumber = "1";

    private List<TMP_Dropdown.OptionData> m_NewDataList_source = new List<TMP_Dropdown.OptionData>();
    private List<TMP_Dropdown.OptionData> m_Messages_s = new List<TMP_Dropdown.OptionData>();

    private Slider xSlider, ySlider, zSlider;
    private float prevSourceX, prevSourceY, prevSourceZ;
    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");

        m_dropdown.ClearOptions();

        sourceIdxText = sourceSelector.transform.GetChild(0).GetComponent<TMP_Text>();
        gainText = gainPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        xText = GlobalFunctions.GetChildWithName(sourcePosition, "xValue").GetComponent<TMP_InputField>();
        yText = GlobalFunctions.GetChildWithName(sourcePosition, "yValue").GetComponent<TMP_InputField>();
        zText = GlobalFunctions.GetChildWithName(sourcePosition, "zValue").GetComponent<TMP_InputField>();

        xText.characterLimit = 4;
        yText.characterLimit = 4;
        zText.characterLimit = 4;

        xSlider = GlobalFunctions.GetChildWithName(sourcePosition, "xSlider").GetComponent<Slider>();
        ySlider = GlobalFunctions.GetChildWithName(sourcePosition, "ySlider").GetComponent<Slider>();
        zSlider = GlobalFunctions.GetChildWithName(sourcePosition, "zSlider").GetComponent<Slider>();

        sourceManager.AddLocalFilesToClipList();

        CreateDropdownList();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateDropdownList()
    {

        int i = 0;
        foreach (AudioClip clip in sourceManager.clipList)
        {
            m_NewDataList_source.Add(new TMP_Dropdown.OptionData());
            m_NewDataList_source[i].text = clip.name;
            //m_Messages_s.Add(m_NewDataList_source[i]);
            m_dropdown.options.Add(m_NewDataList_source[i]);

            i++;
        }

        //Take each entry in the message List
        //foreach (TMP_Dropdown.OptionData message in m_Messages_s)
        ////foreach (TMP_Dropdown.OptionData message in m_Messages_s)
        //{
        //    //Add each entry to the Dropdown
        //    m_dropdown.options.Add(message);
        //}

        m_dropdown.GetComponentInChildren<TMP_Text>().text = "Select Audio..";


    }

    public void SetGainText(float dB)
    {
        gainText.text = "Gain = " + Mathf.Round(dB).ToString() + " dB";
    }

    public void RefreshTextAndUIElements()
    {
        sourceIdxText.text = activeSourceNumber.ToString();

        GameObject curSource = sourceManager.GetCurSource();
        // Vector3 test = sourceManager.GetSourceRatioPositionAt (sourceManager.sourceIdx);
        // float xVal = (curSource.transform.position.x - root.transform.position.x) / (roomSizeManager.roomWidth - sourceManager.sourceColliderDiameter) + 0.5f;
        // float yVal = (curSource.transform.position.y - root.transform.position.y) / (roomSizeManager.roomHeight - sourceManager.sourceColliderHeight);
        // float zVal = (curSource.transform.position.z - root.transform.position.z) / (roomSizeManager.roomDepth - sourceManager.sourceColliderDiameter) + 0.5f;

        // SetPositionThroughSlidersVals(test.x, test.y, test.z);

        SetGainText (20 * Mathf.Log10 (curSource.GetComponent<SteamAudioSource>().directMixLevel));

        m_dropdown.SetValueWithoutNotify(curSource.GetComponent<SourceController>().activeSourceIdx);
    }

    public void SetPositionThroughSliderVals(float x, float y, float z)
    {
        Debug.Log(x + " " + y + " " + z);
        xSlider.SetValueWithoutNotify(x);
        SetX(x);
        ySlider.SetValueWithoutNotify(y);
        SetY(y);
        zSlider.SetValueWithoutNotify(z);
        SetZ(z);
    }

    public void ChangeSourceIdx(int i)
    {
        sourceManager.ChangeSourceIdx(i);
        activeSourceNumber = (i + 1).ToString();
        RefreshTextAndUIElements();
    }

    public void AddSource()
    {
        sourceManager.AddSource();
        ChangeSourceIdx(sourceManager.totNumSources - 1);
    }

    public void RemoveSource()
    {
        if (sourceManager.RemoveSource())
            ChangeSourceIdx(sourceManager.sourceIdx);
    }

    public void NextSource()
    {
        ChangeSourceIdx((sourceManager.sourceIdx + 1) % sourceManager.totNumSources);
    }

    public void PrevSource()
    {
        ChangeSourceIdx((sourceManager.sourceIdx + sourceManager.totNumSources - 1) % sourceManager.totNumSources);
    }

    public void IncreaseGain3dB()
    {

        SetGainText(sourceManager.IncreaseGain3dB());
    }

    public void DecreaseGain3dB()
    {
        SetGainText(sourceManager.DecreaseGain3dB());
    }
    public void SetXFromString(string x)
    {
        float res;
        if (float.TryParse(x, out res))
        {
            res = Mathf.Clamp(res, 0, 1);
            xSlider.SetValueWithoutNotify(res);
            SetX(res);
        }
        else
        {
            xText.text = prevSourceX.ToString("0.0");
        }
    }
    public void SetYFromString(string y)
    {
        float res;
        if (float.TryParse(y, out res))
        {
            res = Mathf.Clamp(res, 0, 1);
            ySlider.SetValueWithoutNotify(res);
            SetY(res);
        }
        else
        {
            yText.text = prevSourceY.ToString("0.0");
        }
    }
    public void SetZFromString(string z)
    {
        float res;
        if (float.TryParse(z, out res))
        {
            res = Mathf.Clamp(res, 0, 1);
            zSlider.SetValueWithoutNotify(res);
            SetZ(res);
        }
        else
        {
            zText.text = prevSourceZ.ToString("0.0");
        }
    }


    public void SetX(float x)
    {
        sourceManager.SetSourceX (x);
        xText.text = x.ToString("0.0");
        prevSourceX = x;
    }

    public void SetY(float y)
    {
        sourceManager.SetSourceY(y);
        yText.text = y.ToString("0.0");
        prevSourceY = y;
    }

    public void SetZ(float z)
    {
        sourceManager.SetSourceZ(z);
        zText.text = z.ToString("0.0");
        prevSourceZ = z;
    }
}
