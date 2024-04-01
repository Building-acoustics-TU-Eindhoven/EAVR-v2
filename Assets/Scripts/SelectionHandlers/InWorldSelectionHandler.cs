using System.Collections;
using System.Collections.Generic;
using SteamAudio;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

// This class handles in world selections. 
// Meaning that if one object (such as a wall or audio source) is selected, all other objects should be deselected

public class InWorldSelectionHandler : MonoBehaviour
{
    private List<SelectionHandler> selectionHandlers = new List<SelectionHandler>();


    [SerializeField]
    private List<InWorldSubMenu> subMenus = new List<InWorldSubMenu>();

    // Start is called before the first frame update
    void Start()
    {
        Object[] test = FindObjectsOfType(typeof (SelectionHandler), false);
        foreach (Object objec in test)
        {
            selectionHandlers.Add (objec.GetComponent<SelectionHandler>()); 
        }

        foreach (Transform child in transform)
        {
            if (child.GetComponent<InWorldSubMenu>())
                subMenus.Add (child.GetComponent<InWorldSubMenu>());
        }
        
        foreach (SelectionHandler s in selectionHandlers)
             s.SetInWorldSelectionHandler (this);

        // Disable in-world UI
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateThisSelection (SelectionHandler handlerThatGotActivated)
    {
        int j = 0; 
        for (int i = 0; i < subMenus.Count; ++i)
        {
            if (selectionHandlers[i] != handlerThatGotActivated)
            {
                selectionHandlers[i].Deselect();
                subMenus[i].gameObject.SetActive (false);

            } else {
                subMenus[i].gameObject.SetActive (true);
            }
            ++j;
        }
    }
}
