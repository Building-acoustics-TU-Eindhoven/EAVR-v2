using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    private InWorldSelectionHandler _inWorldSelectionHandler; 

    public void SetInWorldSelectionHandler (InWorldSelectionHandler iWSH)
    {
        _inWorldSelectionHandler = iWSH;
    }

    public virtual void SetPositionBasedOnHit (RaycastHit hit) 
    {
        _inWorldSelectionHandler.ActivateThisSelection (this);
    }

    public virtual void Deselect() {}
}
