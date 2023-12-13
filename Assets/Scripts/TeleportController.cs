using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

// https://www.youtube.com/watch?v=wGvh7Suo1h4
//https://tuenl-my.sharepoint.com/:v:/r/personal/s_willemsen_tue_nl/Documents/ZendstationVR%20-%20SampleScene%20-%20Windows,%20Mac,%20Linux%20-%20Unity%202021.3.21f1%20Personal%20_DX11_%202023-08-16%2017-23-04.mp4?csf=1&web=1&e=Tur3Sy
public class TeleportController : MonoBehaviour
{
    public GameObject baseControllerGameObject;
    public GameObject teleportationGameObject;

    public InputActionReference teleportActivationReference;

    public GameObject teleportationArea;

    [Space]
    public UnityEvent onTeleportActivate;
    public UnityEvent onTeleportCancel;


    // Start is called before the first frame update
    void Start()
    {
        teleportActivationReference.action.performed += TeleportModeActivate;
        teleportActivationReference.action.canceled += TeleportModeCancel;
    }

    private void TeleportModeCancel(InputAction.CallbackContext obj) => Invoke("DeactivateTeleporter", .1f);

    void DeactivateTeleporter() => onTeleportCancel.Invoke();

    private void TeleportModeActivate(InputAction.CallbackContext obj) => onTeleportActivate.Invoke();

    public void ToggleTeleportationAreaMesh (bool shouldBeVisible)
    {
        foreach (Transform child in teleportationArea.transform)
        {
            if (child.GetComponent<MeshRenderer>())
                child.GetComponent<MeshRenderer>().enabled = shouldBeVisible;
            foreach (Transform childChild in child)
            {
                if (childChild.GetComponent<MeshRenderer>())
                    childChild.GetComponent<MeshRenderer>().enabled = shouldBeVisible;
            }
        }
    }

}
