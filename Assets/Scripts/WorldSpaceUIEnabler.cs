using UnityEngine;

class WorldSpaceUIEnabler : MonoBehaviour
{
    private void Update() {
        Cursor.lockState = CursorLockMode.Confined;
    }
 
    private void LateUpdate() {
        Cursor.lockState = CursorLockMode.Locked;
    }
}