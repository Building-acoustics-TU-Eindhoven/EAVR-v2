using System.Collections;
using System.Collections.Generic;
using UnityEngine;


static public class GlobalFunctions
{
    static public GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }


    static public void GetChildrenWithComponent<T> (GameObject parent, ref List<GameObject> childrenWithComponent)
    {
        foreach (Transform child in parent.transform)
        {
            T test = child.gameObject.GetComponent<T>();
            if (child.gameObject.GetComponent<T>().ToString() != "null")
                childrenWithComponent.Add(child.gameObject);
            GetChildrenWithComponent<T> (child.gameObject, ref childrenWithComponent);
        }
    }

    static public bool shouldUseInWorldMenu = false;
}


