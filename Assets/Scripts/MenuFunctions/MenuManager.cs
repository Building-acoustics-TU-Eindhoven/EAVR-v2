using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    private PlayerManager playerManager;

    [SerializeField]
    private MenuButton wallButton;

    private bool mouseIsUp = true;
    private bool curActive = true;

    [Space]

    [SerializeField]
    private List<SubMenu> subMenus = new List<SubMenu>();

    private int prevActiveMenuIdx = 0;
    private bool calledFromActivateWall = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.GetComponent<SubMenu>())
                subMenus.Add (child.GetComponent<SubMenu>());
        }
        
        foreach (SubMenu subMenu in subMenus)
            subMenu.PrepareSubMenu();

        // Set the disabled text for the wall button
        
        wallButton.SetDisabledText ("No wall\nselected");
        // As no walls can be selected on startup, disable the Wall button
        wallButton.SetEnabled (false);

        // Set the "Main Menu" as active menu
        SetActiveMenu (0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && mouseIsUp)
        {
            OpenCloseMenu (!transform.GetChild(0).gameObject.activeSelf);
            mouseIsUp = false;
        }
    
        if (Input.GetMouseButtonUp(1) && !mouseIsUp)
        {
            mouseIsUp = true;
        }
    }

    public void SetActiveMenu (int idx)
    {
        for (int i = 0; i < subMenus.Count; ++i)
            subMenus[i].gameObject.SetActive (i == idx);
        
        if (!calledFromActivateWall)
            prevActiveMenuIdx = idx;

        calledFromActivateWall = false;
    }

    // Triggered on right-click, or the Main Menu Close button
    public void OpenCloseMenu (bool shouldBeActive)
    {
        curActive = shouldBeActive;
        transform.GetChild(0).gameObject.SetActive(curActive);
        Cursor.lockState = curActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = true;
        playerManager.SetCanvasActive (curActive);
    }

    public void HasSelectedWalls (bool h)
    {
        wallButton.SetEnabled (h);

        calledFromActivateWall = true;
        SetActiveMenu (h ? 3 : prevActiveMenuIdx);
    }
}
