using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    private InWorldSelectionHandler _inWorldSelectionHandler; 

    private GameObject _inWorldSubMenu;

    public void SetInWorldSelectionHandler (InWorldSelectionHandler iWSH)
    {
        _inWorldSelectionHandler = iWSH;
    }

    public void SetInWorldSubMenu (ref GameObject iWSM)
    {
        _inWorldSubMenu = iWSM;
    }

    public ref GameObject GetInWorldSubMenu()
    {
        return ref _inWorldSubMenu;
    }


    public virtual void SetPositionBasedOnHit (RaycastHit hit) 
    {
        _inWorldSelectionHandler.ActivateThisSelection (this);
    }

    public virtual void Deselect() {}
}
