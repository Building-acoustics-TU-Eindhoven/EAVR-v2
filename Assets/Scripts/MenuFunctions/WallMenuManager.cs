using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallMenuManager : SubMenu
{
    // The dropdown component
    public TMP_Dropdown dropdown;
    public List<TMP_Dropdown.OptionData> m_NewDataList = new List<TMP_Dropdown.OptionData>();
    private GameObject root;

    int numMaterials = -1;

    string roomName;

    // Acts like Start() but is called from the menu manager
    public override void PrepareSubMenu()
    {
        root = GameObject.Find("root");
        roomName = root.transform.GetChild(0).name;
        // numMaterials = floor.childCount;
        if (root.transform.GetChild(0).name == roomName)
        {
            numMaterials = root.transform.GetChild(0).GetChild(0).transform.childCount;
        } else {
            numMaterials = root.transform.GetChild(0).transform.childCount;
        }

        InitialiseDropdown();

    }

    private void InitialiseDropdown()
    {
        dropdown.ClearOptions();
        for (int i = 0; i < numMaterials; ++i)
        {
            m_NewDataList.Add(new TMP_Dropdown.OptionData());

            string material;
            if (root.transform.GetChild(0).name == roomName)
            {
                material = root.transform.GetChild(0).GetChild(0).GetChild(i).name;
            } else {
                material = root.transform.GetChild(0).GetChild(i).name;
            }
            material = material.Substring (material.LastIndexOf('_') + 1);
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
