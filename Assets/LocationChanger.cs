using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRoriginManager : MonoBehaviour
{
    public GameObject xrOrigin;
    public GameObject mainXRCamera;

    public RoomSizeManager roomSizeManager;
    private Vector3 origCameraPos = new Vector3();

    public GameObject root;
    private Vector3 normPrevPosition;
    private float playerColliderDiameter = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");
        origCameraPos = mainXRCamera.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (normPrevPosition != roomSizeManager.GetNormalisedPlayerPos())
        {
            Debug.Log (normPrevPosition);
            roomSizeManager.SetPlayerTransform();
        }
        normPrevPosition = roomSizeManager.GetNormalisedPlayerPos();
    }

    public void TeleportToLocation (Vector3 newLocation, Vector3 roomDimensions)
    {
        Vector3 test = new Vector3((newLocation.x - 0.5f) * (roomDimensions.x - playerColliderDiameter) + root.transform.position.x,
                                    newLocation.y * roomDimensions.y + root.transform.position.y,
                                   (newLocation.z - 0.5f) * (roomDimensions.z - playerColliderDiameter) + root.transform.position.z);


        var distanceDiff = mainXRCamera.transform.position - test;
        xrOrigin.transform.position -= distanceDiff;
    }

    public void SetPlayerX(float x, float roomWidth)
    {
        float cameraOffset = xrOrigin.transform.GetChild(0).transform.localPosition.y;

        Vector3 test = new Vector3(x * roomWidth + root.transform.position.x,
                                         transform.position.y,
                                         transform.position.z);
        var distanceDiff = mainXRCamera.transform.position - test;
        xrOrigin.transform.position = test;

    }

    public void SetPlayerY(float y, float roomHeight)
    {
        float cameraOffset = xrOrigin.transform.GetChild(0).transform.position.y;

        Vector3 test = new Vector3(transform.position.x,
                                         y * roomHeight + root.transform.position.y,
                                         transform.position.z);
        var distanceDiff = mainXRCamera.transform.position - test;
        xrOrigin.transform.position = test;

    }

    public void SetPlayerZ(float z, float roomDepth)
    {
        Vector3 test = new Vector3(transform.position.x,
                                         transform.position.y,
                                         z * roomDepth + root.transform.position.z);
        var distanceDiff = mainXRCamera.transform.position - test;
        xrOrigin.transform.position = test;
    }

}
