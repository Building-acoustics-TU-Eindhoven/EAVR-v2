using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherits from the interface in PlayerManager to allow for bidirectional communication between the classes
public class RoomMenuManager : SubMenu, iRoomMenuManager 
{
    // The GameObject holding the room geometry 
    public GameObject root;

    // Original size of the room geometry
    public Vector3 originalSize;

    // Current room dimensions
    public Vector3 curRoomSize;

    // Player position normalised between -0.5 and 0.5 for X and Z and between 0 and 1 for Y.
    private Vector3 normalisedPlayerPos = new Vector3 (0.5f, 0.2f, 0.5f);

    // Non normalised player position (for use in PlayerManager Update() to check if this class has the latest player position
    private Vector3 nonNormalisedPlayerPos = new Vector3 (0.0f, 1.0f, 0.0f);

    // The player
    public GameObject playerGO;
    private PlayerManager playerManager;

    // The XR origin
    public GameObject xrOrigin;
    private XRoriginManager xrOriginManager;

    private bool playerActive = false;

    // Audio source manager
    public AudioSourceManager audioSourceManager;

    private float playerColliderRadius = 0.5f;

    private bool prepared = false;

    [SerializeField]
    private KnobButton xKnob, yKnob, zKnob;

    // Acts like Start() but is called from the menu manager
    public override void PrepareSubMenu()
    {
        // Obtain room geometry 
        root = GameObject.Find("root");

        // Obtain the dimensions of the room geometry
        Bounds b = getBounds(root);

        originalSize = b.size;
        curRoomSize = originalSize;

        audioSourceManager.SetOriginalRoomDimensions (originalSize);

        playerActive = playerGO.activeSelf;
        if (playerActive)
        {
            playerManager = playerGO.GetComponent<PlayerManager>();
            RefreshInternalPlayerPos(false);
            playerManager.PreparePlayerPosAndRoomDimensions (normalisedPlayerPos, originalSize);
        }
        else {
            xrOriginManager = xrOrigin.GetComponent<XRoriginManager>();
            RefreshInternalPlayerPos(true);

        }

        RefreshTextAndUIElements();

        prepared = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetNormalisedPlayerPos()
    {
        return normalisedPlayerPos;
    }

    // Doesn't change the player position, simply retrieves its position as a ratio of the room width and height
    public void RefreshInternalPlayerPos (bool isXRorigin)
    {
        if (!prepared)
            return;
        nonNormalisedPlayerPos = playerActive ? playerGO.transform.position : xrOrigin.transform.position;

        normalisedPlayerPos = new Vector3 ((nonNormalisedPlayerPos.x - root.transform.localPosition.x) / (curRoomSize.x - playerColliderRadius) + 0.5f, // Subtract radius so that the normalised player position all the way at the edge is 0 or 1
                                           ((nonNormalisedPlayerPos.y - (isXRorigin ? 0.0f : 1.0f)) - root.transform.localPosition.y) / curRoomSize.y,                // Subtract 1.0f as this is half the height of the player collider
                                           (nonNormalisedPlayerPos.z - root.transform.localPosition.z) / (curRoomSize.z - playerColliderRadius) + 0.5f); // Subtract radius so that the normalised player position all the way at the edge is 0 or 1

        //Debug.Log("Normalised pos: " + normalisedPlayerPos);
    }

    public void SetRoomXfromKnob (KnobButton knob)
    {
        // Obtain value from the knob
        float xScaling = knob.GetValue();

        // Set the width of the room geometry
        root.transform.localScale = new Vector3(xScaling, root.transform.localScale.y, root.transform.localScale.z);

        // Set the internal room width variable by scaling the original width 
        curRoomSize = new Vector3 (xScaling * originalSize.x, curRoomSize.y, curRoomSize.z);
        
        // Update the source location
        audioSourceManager.SetSourcesFromRoomSizeX (curRoomSize.x);

        // Update the player X location
        if (playerActive)
            playerManager.SetPlayerX(normalisedPlayerPos.x, curRoomSize.x);
        else
            xrOriginManager.SetPlayerX(normalisedPlayerPos.x, curRoomSize.x);

    }

    public void SetRoomYfromKnob (KnobButton knob)
    {
        // Obtain value from the knob
        float yScaling = knob.GetValue();
                
        // Set the height of the room geometry
        root.transform.localScale = new Vector3(root.transform.localScale.x, yScaling, root.transform.localScale.z);

        // Set the internal room width variable by scaling the original width 
        curRoomSize = new Vector3 (curRoomSize.x, yScaling * originalSize.y, curRoomSize.z);

        // Update the source location
        audioSourceManager.SetSourcesFromRoomSizeY (curRoomSize.y);

        // Update the player Y location
        if (playerActive)
            playerManager.SetPlayerY(normalisedPlayerPos.y, curRoomSize.y);
        else
            xrOrigin.GetComponent<XRoriginManager>().SetPlayerY(normalisedPlayerPos.y, curRoomSize.y);

    }

    public void SetRoomZfromKnob (KnobButton knob)
    {
        // Obtain value from the knob
        float zScaling = knob.GetValue();
        
        // Set the depth of the room geometry
        root.transform.localScale = new Vector3(root.transform.localScale.x, root.transform.localScale.y, zScaling);

        // Set the internal room width variable by scaling the original width 
        curRoomSize = new Vector3 (curRoomSize.x, curRoomSize.y, zScaling * originalSize.z);

        // Update the source location
        audioSourceManager.SetSourcesFromRoomSizeZ (curRoomSize.z);
       
        // Update the player Z location
        if (playerActive)
            playerManager.SetPlayerZ(normalisedPlayerPos.z, curRoomSize.z);
        else
            xrOriginManager.SetPlayerZ(normalisedPlayerPos.z, curRoomSize.z);

    }

    public void SetKnobValuesFromObservation(Vector3 obs)
    {
        xKnob.SetNonNormalisedValue(obs.x);
        yKnob.SetNonNormalisedValue(obs.y);
        zKnob.SetNonNormalisedValue(obs.z);
    }

    public void RefreshTextAndUIElements()
    {
        SetKnobValuesFromCurrentRoomDimensions();
    }

    private void SetKnobValuesFromCurrentRoomDimensions()
    {
        // As the scaling is not normalised (not between 0 and 1) use the SetNonNormalisedValue() function
        xKnob.SetNonNormalisedValue (curRoomSize.x / originalSize.x, false);
        yKnob.SetNonNormalisedValue (curRoomSize.y / originalSize.y, false);
        zKnob.SetNonNormalisedValue (curRoomSize.z / originalSize.z, false);
    }

    public Vector3 GetNormalisedPlayerPosition() { return normalisedPlayerPos; }
    public Vector3 GetNonNormalisedPlayerPosition() { return nonNormalisedPlayerPos; }

    // Functions found online to get the bounds of a game object
    Bounds getRenderBounds (GameObject go)
    {
        Bounds bounds = new  Bounds(Vector3.zero,Vector3.zero);
        Renderer render = go.GetComponent<Renderer>();
        if(render!=null){
            return render.bounds;
        }
        return bounds;
    }

    public Bounds getBounds (GameObject go)
    {
        Bounds bounds;
        Renderer childRender;
        bounds = getRenderBounds(go);
        if(bounds.extents.x == 0){
            bounds = new Bounds(go.transform.position,Vector3.zero);
            foreach (Transform child in go.transform) {
                childRender = child.GetComponent<Renderer>();
                if (childRender) {
                    bounds.Encapsulate(childRender.bounds);
                }else{
                    bounds.Encapsulate(getBounds(child.gameObject));
                }
            }
        }
        return bounds;
    }
}
