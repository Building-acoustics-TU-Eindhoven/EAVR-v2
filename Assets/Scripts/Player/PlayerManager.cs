// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;

// Interface to the RoomMenuManager
public interface iRoomMenuManager
{
    void RefreshInternalPlayerPos(bool isXRorigin);
    Vector3 GetNonNormalisedPlayerPosition();
}

/// First-person player controller for Resonance Audio demo scenes.
[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    /// Camera
    public Camera mainCamera;

    // Room.
    public GameObject root;

    // Room menu manager
    public GameObject roomMenuManager;

    // Character controller.
    private CharacterController characterController = null;

    // Player movement speed.
    private float movementSpeed = 5.0f;

    // Target camera rotation in degrees.
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    // Maximum allowed vertical rotation angle in degrees.
    private const float clampAngleDegrees = 80.0f;

    // Camera rotation sensitivity.
    private const float sensitivity = 5.0f;

    private bool canvasActive = true;

    public GameObject reticleCanvas;

    private float playerColliderRadius = 0.5f;

    bool prepared = false;
    private Vector3 normalisedPosToSet = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 roomDimensionsToSet = new Vector3(0.0f, 0.0f, 0.0f);
    private bool shouldUpdatePosition = false;

    void Start()
    {
        root = GameObject.Find("root");

        characterController = GetComponent<CharacterController>();
        Vector3 rotation = mainCamera.transform.localRotation.eulerAngles;
        rotationX = rotation.x;
        rotationY = rotation.y;
        reticleCanvas.SetActive (!canvasActive);

        playerColliderRadius = GetComponent<CapsuleCollider>().radius;

        roomMenuManager.GetComponent<iRoomMenuManager>().RefreshInternalPlayerPos(false);

        // Position and dimensions should have been given by the RoomMenuManager through the PreparePlayerPosAndRoomDimensions() function
        ApplyPlayerPosAndRoomDimensions();

        prepared = true;

    }

    public void SetRotationAndPosition()
    {
        Vector3 rotation = mainCamera.transform.localRotation.eulerAngles;

        if (rotation.x > 80.0f)
            rotationX = rotation.x - 360f;
        else
            rotationX = rotation.x;
        rotationY = rotation.y;

        // characterController.center = mainCamera.transform.position
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown("space"))
            characterController.Move(new Vector3 (0.0f, 10.0f, 0.0f));
        
        // Check whether the position in the RoomMenuManger is updated
        if (Mathf.Abs(transform.position.magnitude - roomMenuManager.GetComponent<iRoomMenuManager>().GetNonNormalisedPlayerPosition().magnitude) > 0.001f)
        {
            roomMenuManager.GetComponent<iRoomMenuManager>().RefreshInternalPlayerPos(false);
        }

    }

    void LateUpdate()
    {
#if UNITY_EDITOR

        // if (Input.GetMouseButtonDown(0)) {
        //   //SetCursorLock(true);
        //     Cursor.lockState = CursorLockMode.None;
        //     Cursor.visible = false;
        // } else if (Input.GetKeyDown(KeyCode.Escape)) {
        //   SetCursorLock(false);
        // }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLock(false);
        }
#endif  // UNITY_EDITOR
        // Update the rotation.
        if (canvasActive)
            return;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Note that multi-touch control is not supported on mobile devices.
            mouseX = 0.0f;
            mouseY = 0.0f;
        }
        rotationX += sensitivity * mouseY;
        rotationY += sensitivity * mouseX;
        rotationX = Mathf.Clamp(rotationX, -clampAngleDegrees, clampAngleDegrees);
        mainCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        // Update the position.
        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(movementX, 0.0f, movementY);

        movementDirection = mainCamera.transform.localRotation * movementDirection;

        characterController.SimpleMove(movementSpeed * movementDirection);

    }

    // Sets the cursor lock for first-person control.
    private void SetCursorLock(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetCanvasActive(bool c)
    {
        reticleCanvas.SetActive (!c);
        canvasActive = c;
    }

    // Called from room menu manager Awake()
    public void PreparePlayerPosAndRoomDimensions (Vector3 normalisedPos, Vector3 roomDimensions)
    {
        normalisedPosToSet = normalisedPos;
        roomDimensionsToSet = roomDimensions;
    }

    public void ApplyPlayerPosAndRoomDimensions()
    {
        SetPlayerX (normalisedPosToSet.x, roomDimensionsToSet.x);
        SetPlayerY (normalisedPosToSet.y, roomDimensionsToSet.y);
        SetPlayerZ (normalisedPosToSet.z, roomDimensionsToSet.z);
    }

    public void SetPlayerX(float normalisedX, float roomWidth)
    {
        transform.position = new Vector3((normalisedX - 0.5f) * (roomWidth - playerColliderRadius) + root.transform.position.x,
                                         transform.position.y,
                                         transform.position.z);
    }

    public void SetPlayerY(float normalisedY, float roomHeight)
    {
        transform.position = new Vector3(transform.position.x,
                                         normalisedY * roomHeight + root.transform.position.y + 1.0f,
                                         transform.position.z);
    }

    public void SetPlayerZ(float normalisedZ, float roomDepth)
    {
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         (normalisedZ - 0.5f)  * (roomDepth - playerColliderRadius) + root.transform.position.z);
    }
}
