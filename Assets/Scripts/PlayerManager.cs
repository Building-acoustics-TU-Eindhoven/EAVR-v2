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

/// First-person player controller for Resonance Audio demo scenes.
[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    /// Camera.
    public Camera mainCamera;

    // Room.
    private GameObject root;

    public RoomSizeManager roomSizeManager;

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

    private float playerColliderDiameter;
    void Start()
    {
        root = GameObject.Find("root");

        characterController = GetComponent<CharacterController>();
        Vector3 rotation = mainCamera.transform.localRotation.eulerAngles;
        rotationX = rotation.x;
        rotationY = rotation.y;
        reticleCanvas.SetActive (!canvasActive);

        playerColliderDiameter = GetComponent<CapsuleCollider>().radius * 2;

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
        if (Input.GetKeyDown("space"))
            characterController.Move(new Vector3 (0.0f, 10.0f, 0.0f));
        // Register player-move keypresses and forward them to the roomSizeManager so that the player's relative position to the roomsize doesn't change
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
            Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            roomSizeManager.SetPlayerTransform();
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

    public void SetPlayerPos (Vector3 pos, Vector3 roomDimensions)
    {
        Vector3 test = new Vector3((pos.x - 0.5f) * (roomDimensions.x - playerColliderDiameter) + root.transform.position.x,
                                         pos.y * roomDimensions.y + root.transform.position.y,
                                         (pos.z - 0.5f) * (roomDimensions.z - playerColliderDiameter) + root.transform.position.z);
        transform.position = test;
    }

    public void SetPlayerX(float x, float roomWidth)
    {
        transform.position = new Vector3(x * (roomWidth - playerColliderDiameter) + root.transform.position.x,
                                         transform.position.y,
                                         transform.position.z);
    }

    public void SetPlayerY(float y, float roomHeight)
    {
        transform.position = new Vector3(transform.position.x,
                                         y * roomHeight + root.transform.position.y,
                                         transform.position.z);
    }

    public void SetPlayerZ(float z, float roomDepth)
    {
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         z * (roomDepth - playerColliderDiameter) + root.transform.position.z);
    }
}
