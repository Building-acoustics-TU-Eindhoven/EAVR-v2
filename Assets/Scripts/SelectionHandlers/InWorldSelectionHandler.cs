using System.Collections;
using System.Collections.Generic;
using SteamAudio;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;

// This class handles in world selections. 
// Meaning that if one object (such as a wall or audio source) is selected, all other objects should be deselected

public class InWorldSelectionHandler : MonoBehaviour
{
    private List<SelectionHandler> selectionHandlers = new List<SelectionHandler>();


    [SerializeField]
    private GameObject sourceMenu;

    [SerializeField]
    private GameObject wallMenu; 

    // Start is called before the first frame update
    void Start()
    {
        UpdateSelectionHandlers();

        // Disable in-world UI
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSelectionHandlers()
    {
        selectionHandlers.Clear();
        Object[] selectionHandlersInScene = FindObjectsOfType(typeof (SelectionHandler), false);
        foreach (Object objec in selectionHandlersInScene)
        {
            selectionHandlers.Add (objec.GetComponent<SelectionHandler>()); 
        }

        foreach (SelectionHandler s in selectionHandlers)
        {
            s.SetInWorldSelectionHandler (this);

            // Assign the correct menus to the selection handlers
            if (s.GetType() == typeof(AudioSourceSelectionHandler))
                s.SetInWorldSubMenu (ref sourceMenu);
            else if (s.GetType() == typeof(WallSelectionHandler))
                s.SetInWorldSubMenu (ref wallMenu);
        }
    }

    public void ActivateThisSelection (SelectionHandler handlerThatGotActivated)
    {
        int j = 0; 
        for (int i = 0; i < selectionHandlers.Count; ++i)
        {
            if (selectionHandlers[i] != handlerThatGotActivated)
            {
                selectionHandlers[i].Deselect();
                selectionHandlers[i].GetInWorldSubMenu().SetActive(false);
            }
        }
        handlerThatGotActivated.GetInWorldSubMenu().SetActive(true);
    }
}
