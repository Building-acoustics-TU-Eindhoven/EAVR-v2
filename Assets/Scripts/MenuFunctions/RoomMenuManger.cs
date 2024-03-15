using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherits from the interface in PlayerManager to allow for bidirectional communication between the classes
public class RoomMenuManger : SubMenu, iRoomMenuManager 
{
    // The GameObject holding the room geometry 
    public GameObject root;

    // Original size of the room geometry
    public Vector3 originalSize;

    // Current room dimensions
    public float curRoomWidth, curRoomHeight, curRoomDepth;

    // Player position normalised between -0.5 and 0.5 for X and Z and between 0 and 1 for Y.
    private Vector3 normalisedPlayerPos = new Vector3 (0.0f, 0.2f, 0.0f);

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

    public float playerColliderRadius = 0.75f;

    private bool prepared = false;

    // Acts like Start() but is called from the menu manager
    public override void PrepareSubMenu()
    {
        // Obtain room geometry 
        root = GameObject.Find("root");

        // Obtain the dimensions of the room geometry
        Bounds b = getBounds(root);

        originalSize = b.size;
        curRoomWidth = originalSize.x;
        curRoomHeight = originalSize.y;
        curRoomDepth = originalSize.z;

        audioSourceManager.SetOriginalRoomDimensions (originalSize, true);

        playerActive = playerGO.activeSelf;
        if (playerActive)
        {
            playerManager = playerGO.GetComponent<PlayerManager>();
            playerColliderRadius = playerManager.GetComponent<CapsuleCollider>().radius;
            RefreshInternalPlayerPos();
            playerManager.PreparePlayerPosAndRoomDimensions (normalisedPlayerPos, originalSize); // This also calls RefreshInternalPlayerPos
        }
        else {
            xrOriginManager = xrOrigin.GetComponent<XRoriginManager>();
            playerColliderRadius = 0.25f;

        }

        prepared = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Doesn't change the player position, simply retrieves its position as a ratio of the room width and height
    public void RefreshInternalPlayerPos()
    {
        if (!prepared)
            return;
        nonNormalisedPlayerPos = playerActive ? playerGO.transform.position : xrOrigin.transform.position;

        normalisedPlayerPos = new Vector3 ((nonNormalisedPlayerPos.x - root.transform.localPosition.x) / (curRoomWidth - playerColliderRadius), // Subtract radius so that the normalised player position all the way at the edge is 0.5
                                           ((nonNormalisedPlayerPos.y - 1.0f) - root.transform.localPosition.y) / curRoomHeight,                // Subtract 1.0f as this is half the height of the player collider
                                           (nonNormalisedPlayerPos.z - root.transform.localPosition.z) / (curRoomDepth - playerColliderRadius)); // Subtract radius so that the normalised player position all the way at the edge is 0.5

    }

    public void SetRoomXfromKnob (KnobButton knob)
    {
        // Obtain value from the knob
        float xScaling = knob.GetValue();

        // Set the width of the room geometry
        root.transform.localScale = new Vector3(xScaling, root.transform.localScale.y, root.transform.localScale.z);

        // Set the internal room width variable by scaling the original width 
        curRoomWidth = xScaling * originalSize.x;
        
        // Update the source location
        audioSourceManager.SetSourcesFromRoomSizeX (curRoomWidth);

        // Update the player X location
        if (playerActive)
            playerManager.SetPlayerX(normalisedPlayerPos.x, curRoomWidth);
        else
            xrOriginManager.SetPlayerX(normalisedPlayerPos.x, curRoomWidth);

    }

    public void SetRoomYfromKnob (KnobButton knob)
    {
        // Obtain value from the knob
        float yScaling = knob.GetValue();
                
        // Set the height of the room geometry
        root.transform.localScale = new Vector3(root.transform.localScale.x, yScaling, root.transform.localScale.z);

        // Set the internal room height variable by scaling the original height 
        curRoomHeight = yScaling * originalSize.y;

        // Update the source location
        audioSourceManager.SetSourcesFromRoomSizeY (curRoomHeight);

        // Update the player Y location
        if (playerActive)
            playerManager.SetPlayerY(normalisedPlayerPos.y, curRoomHeight);
        else
            xrOrigin.GetComponent<XRoriginManager>().SetPlayerY(normalisedPlayerPos.y, curRoomHeight);

    }

    public void SetRoomZfromKnob (KnobButton knob)
    {
        // Obtain value from the knob
        float zScaling = knob.GetValue();
        
        // Set the depth of the room geometry
        root.transform.localScale = new Vector3(root.transform.localScale.x, root.transform.localScale.y, zScaling);

        // Set the internal room height variable by scaling the original height 
        curRoomDepth = zScaling * originalSize.z;

        // Update the source location
        audioSourceManager.SetSourcesFromRoomSizeZ (curRoomDepth);
       
        // Update the player Z location
        if (playerActive)
            playerManager.SetPlayerZ(normalisedPlayerPos.z, curRoomDepth);
        else
            xrOriginManager.SetPlayerZ(normalisedPlayerPos.z, curRoomDepth);

    }

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
