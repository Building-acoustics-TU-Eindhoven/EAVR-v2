using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSteamAudio : MonoBehaviour
{
    public PlayerManager playerManager;
    private bool mouseIsUp = true;
    public GameObject root = null;
    // Start is called before the first frame update

    private bool curActive = true;
    void Start()
    {
        root = GameObject.FindGameObjectWithTag("Model"); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && mouseIsUp)
        {
            curActive = !transform.GetChild(0).gameObject.activeSelf;
            transform.GetChild(0).gameObject.SetActive(curActive);
            Cursor.lockState = curActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = true;
            playerManager.SetCanvasActive (curActive);

            mouseIsUp = false;
        }
    
        if (Input.GetMouseButtonUp(1) && !mouseIsUp)
        {
            mouseIsUp = true;
        }
    }

    public void ChangeModelSize (float scale)
    {
        root.transform.localScale = new Vector3 (scale, scale, scale);
    }

    public bool IsCanvasActive()
    {
        return curActive;
    }
}