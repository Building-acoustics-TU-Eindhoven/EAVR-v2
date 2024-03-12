using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    public PlayerManager playerManager;
    private bool mouseIsUp = true;
    private bool curActive = true;

    public List<GameObject> subMenus = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform.GetChild(0))
        {
            subMenus.Add (child.gameObject);
        }

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
            subMenus[i].SetActive (i == idx);
    }

    public void OpenCloseMenu (bool shouldBeActive)
    {
        curActive = shouldBeActive;
        transform.GetChild(0).gameObject.SetActive(curActive);
        Cursor.lockState = curActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = true;
        playerManager.SetCanvasActive (curActive);
    }
}
