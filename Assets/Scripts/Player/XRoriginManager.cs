using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRoriginManager : MonoBehaviour
{
    // The XR Origin. Its position gets changed when teleporting
    public GameObject xrOrigin;

    // The main camera in the XR origin. Its position is relative to the XR origin
    public GameObject mainXRCamera;

    public RoomMenuManager roomMenuManager;

    public GameObject root;

    private float playerColliderRadius = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");
    }

    // Update is called once per frame
    void Update()
    {
        // Check whether the position in the RoomMenuManger is updated
        if (Mathf.Abs(transform.position.magnitude - roomMenuManager.GetComponent<iRoomMenuManager>().GetNonNormalisedPlayerPosition().magnitude) > 0.001f)
        {
            roomMenuManager.GetComponent<iRoomMenuManager>().RefreshInternalPlayerPos(true);
        }
    }

    public void TeleportToLocation (Vector3 newLocation, Vector3 roomDimensions)
    {
        Vector3 test = new Vector3((newLocation.x - 0.5f) * (roomDimensions.x - playerColliderRadius) + root.transform.position.x,
                                    newLocation.y * roomDimensions.y + root.transform.position.y + 1.5f, // Camera Y offset
                                   (newLocation.z - 0.5f) * (roomDimensions.z - playerColliderRadius) + root.transform.position.z);


        var distanceDiff = mainXRCamera.transform.position - test;
        transform.position -= distanceDiff;
    }


    public void SetPlayerX(float x, float roomWidth)
    {
        Vector3 test = new Vector3((x - 0.5f) * (roomWidth - playerColliderRadius) + root.transform.position.x,
                                   transform.position.y,
                                   transform.position.z);
        transform.position = test;

    }

    public void SetPlayerY(float y, float roomHeight)
    {
        Vector3 test = new Vector3(transform.position.x,
                                   y * roomHeight + root.transform.position.y,
                                   transform.position.z);
        transform.position = test;

    }

    public void SetPlayerZ(float z, float roomDepth)
    {
        Vector3 test = new Vector3(transform.position.x,
                                   transform.position.y,
                                   (z - 0.5f) * (roomDepth - playerColliderRadius) + root.transform.position.z);
        transform.position = test;
    }
}
