using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SteamAudioDropdownManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Transform floor;

    public List<TMP_Dropdown.OptionData> m_NewDataList = new List<TMP_Dropdown.OptionData>();

    int numMaterials = -1;
    // Start is called before the first frame update
    void Start()
    {
        numMaterials = floor.childCount;
        InitialiseDropdown();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitialiseDropdown()
    {
        dropdown.ClearOptions();
        for (int i = 0; i < numMaterials; ++i)
        {
            m_NewDataList.Add(new TMP_Dropdown.OptionData());

            string material = floor.GetChild(i).name;
            material = material.Substring (0, material.Length - 5);
            m_NewDataList[i].text = material;
        }

        TMP_Dropdown.OptionData test = new TMP_Dropdown.OptionData();
        test.text = "Choose material..";
        dropdown.options.Add(test);
        //Take each entry in the message List
        foreach (TMP_Dropdown.OptionData message in m_NewDataList)
        {
            //Add each entry to the Dropdown
            dropdown.options.Add(message);
        }
    }

    public void ResetDropdown()
    {
        dropdown.SetValueWithoutNotify (0);
    }
}
