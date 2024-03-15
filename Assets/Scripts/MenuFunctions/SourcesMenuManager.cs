using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SourcesMenuManager : SubMenu
{
    // Manager of the audio sources
    public AudioSourceManager sourceManager;

    public IncDecButton sourceSelectionButton;

    private int activeSourceNumber = 1;
    public int totNumSources = 0;
    public TMP_Dropdown m_dropdown;
    private List<TMP_Dropdown.OptionData> m_NewDataList_source = new List<TMP_Dropdown.OptionData>();
    private List<TMP_Dropdown.OptionData> m_Messages_s = new List<TMP_Dropdown.OptionData>();

    private bool dropDownListCreated = false;

    [SerializeField]
    private KnobButton xKnob, yKnob, zKnob, gainKnob;

    [SerializeField]
    private Toggle loopButton, pauseButton;
    
    // Acts like Start() but is called from the menu manager
    public override void PrepareSubMenu()
    {
        m_dropdown.ClearOptions(); 
        CreateDropdownList();

        gainKnob.Start();
    }

    void OnEnable()
    {
        if (!dropDownListCreated)
        {
            m_dropdown.ClearOptions();  
            CreateDropdownList();
        }

        if (sourceManager.IsPrepared())
            RefreshTextAndUIElements (sourceManager.GetCurSource());
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

        dropDownListCreated = true;
        // m_dropdown.GetComponentInChildren<TMP_Text>().text = "Select Audio...";


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSourceIdxFromButton (IncDecButton button)
    {
        ChangeSourceIdx (button.GetCurrentValue(), false);
    }

    public void ChangeSourceIdx(int i, bool zeroBased)
    {
        int zeroBasedVal = zeroBased ? i : i - 1;
        activeSourceNumber = zeroBasedVal + 1;
        int validSourceNumber = sourceSelectionButton.SnapToValidValue (activeSourceNumber);
        if (activeSourceNumber != validSourceNumber)
            ChangeSourceIdx (validSourceNumber, false);
        else 
            sourceManager.ChangeSourceIdx(zeroBasedVal);

        RefreshTextAndUIElements (sourceManager.GetCurSource());
    }


    public void AddSource()
    {
        sourceManager.AddSource();
        ++totNumSources;

        sourceSelectionButton.SetMaxValue (totNumSources);
        ChangeSourceIdx(totNumSources, false);
    }

    public void RemoveSource()
    {
        if (sourceManager.RemoveSource())
        {
            --totNumSources;
            sourceSelectionButton.SetMaxValue (totNumSources);
            ChangeSourceIdx(activeSourceNumber == 1 ? activeSourceNumber : (activeSourceNumber-1), false);
        }
    }

    public void RefreshTextAndUIElements (SourceController currentSource)
    {
        if (currentSource == null)
            m_dropdown.GetComponentInChildren<TMP_Text>().text = "Select Audio..";
        else 
        {
            m_dropdown.SetValueWithoutNotify(currentSource.GetActiveClipIdx());
        }
        SetKnobValuesFromSource (currentSource);

        // Set the loop button state
        loopButton.isOn = currentSource.GetShouldLoop();

        // Set the pause button state
        pauseButton.isOn = !currentSource.GetShouldPause();

        sourceSelectionButton.SetCurrentValue (activeSourceNumber);

    }

    private void SetKnobValuesFromSource (SourceController source)
    {
        Vector3 ratioVec = source.GetRatioVec();

        xKnob.SetNormalisedValue (ratioVec.x, false);
        yKnob.SetNormalisedValue (ratioVec.y, false);
        zKnob.SetNormalisedValue (ratioVec.z, false);

        gainKnob.SetNonNormalisedValue (source.GetGainDb(), false);
    }

    public void SetDefaultKnobValues (Vector3 defaults)
    {
        xKnob.SetDefaultValue (defaults.x);
        yKnob.SetDefaultValue (defaults.y);
        zKnob.SetDefaultValue (defaults.z);
    }
}
