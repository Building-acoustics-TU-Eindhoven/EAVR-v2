using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcesMenuManager : MonoBehaviour
{
    // Manager of the audio sources
    public AudioSourceManager sourceManager;

    public IncDecButton sourceSelectionButton;

    private int activeSourceNumber = 1;
    public int totNumSources = 0;
    public int maxSources = 10;
    // Start is called before the first frame update
    void Start()
    {

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

        RefreshTextAndUIElements();
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

    public void RefreshTextAndUIElements()
    {
        sourceSelectionButton.SetCurrentValue (activeSourceNumber);
    }
}
