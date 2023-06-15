//MIT License
//Copyright (c) 2023 DA LAB (https://www.youtube.com/@DA-LAB)
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeHandle;

public class SelectTransformGizmo : MonoBehaviour
{
    public Material highlightMaterial;
    public Material selectionMaterial;

    private Material originalMaterialHighlight;
    private Material originalMaterialSelection;
    private Transform highlight;
    private List<Transform> selection = new List<Transform>();
    private RaycastHit raycastHit;
    private RaycastHit raycastHitHandle;
    private GameObject runtimeTransformGameObj;
    private RuntimeTransformHandle runtimeTransformHandle;
    private int runtimeTransformLayer = 6;
    private int runtimeTransformLayerMask;

    public GeometryManager geometryManager;

    bool leftShiftDown, rightShiftDown, shiftDown;

    private void Start()
    {
        runtimeTransformGameObj = new GameObject();
        runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
        runtimeTransformGameObj.layer = runtimeTransformLayer;
        runtimeTransformLayerMask = 1 << runtimeTransformLayer; //Layer number represented by a single bit in the 32-bit integer using bit shift
        runtimeTransformHandle.type = HandleType.POSITION;
        runtimeTransformHandle.autoScale = true;
        runtimeTransformHandle.autoScaleFactor = 1.0f;
        runtimeTransformGameObj.SetActive(false);
    }


    private void RemoveFromSelection (Transform selToRemove)
    {
        foreach (Transform sel in selection)
        {
            if (sel != selToRemove)
                continue;
            Transform activeChild = GetActiveChild(sel);
            Material[] materials = new Material[activeChild.GetComponent<MeshRenderer>().materials.Length];
            for (int i = 0; i < activeChild.GetComponent<MeshRenderer>().materials.Length; ++i)
                materials[i] = GetOriginalMaterial (activeChild.name);
            
            activeChild.GetComponent<MeshRenderer>().materials = materials;
            selection.Remove (selToRemove);
            return;
        }
    }
    private void ClearSelection()
    {
        foreach (Transform sel in selection)
        {
            Transform activeChild = GetActiveChild(sel);
            Material[] materials = new Material[activeChild.GetComponent<MeshRenderer>().materials.Length];
            for (int i = 0; i < activeChild.GetComponent<MeshRenderer>().materials.Length; ++i)
                materials[i] = GetOriginalMaterial (activeChild.name);
            
            activeChild.GetComponent<MeshRenderer>().materials = materials;
        }
        selection.Clear();
    }
    void Update()
    {
        if (Input.GetKeyDown("left shift"))
        {
            leftShiftDown = true;
        }
        if (Input.GetKeyUp("left shift"))
        {
            leftShiftDown = false;
        }
        if (Input.GetKeyDown("right shift"))
        {
            rightShiftDown = true;
        }
        if (Input.GetKeyUp("right shift"))
        {
            rightShiftDown = false;
        }

        shiftDown = (leftShiftDown || rightShiftDown);
                    

        // Highlight
        if (highlight != null)
        {
            highlight.GetComponent<MeshRenderer>().sharedMaterial = originalMaterialHighlight;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && selection.Contains(highlight.parent))
            {
                if (highlight.GetComponent<MeshRenderer>().material != highlightMaterial)
                {
                    originalMaterialHighlight = highlight.GetComponent<MeshRenderer>().material;
                    // highlight.GetComponent<MeshRenderer>().material.Lerp (highlightMaterial, originalMaterialHighlight, 0.9f);
                }
            }
            else
            {
                highlight = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            // ApplyLayerToChildren(runtimeTransformGameObj);
            if (Physics.Raycast(ray, out raycastHit))
            {
                // if (Physics.Raycast(ray, out raycastHitHandle, Mathf.Infinity, runtimeTransformLayerMask)) //Raycast towards runtime transform handle only
                // {
                // }
                // else if (highlight)
                // {
                    Transform parentWall = raycastHit.transform.parent.transform; 
                    // If wall is already selected remove it from the list
                    if (selection.Contains (parentWall))
                    {
                        if (shiftDown || selection.Count == 1)
                        {
                            RemoveFromSelection (parentWall);
                        }
                        else
                        {
                            ClearSelection();
                            selection.Add (parentWall);
                        }
                    }
                    else
                    {
                        // If shift is not down, clear wall list first
                        if (!shiftDown)
                            ClearSelection();

                        // Add the selected wall to the list
                        selection.Add (parentWall);
                    }
                    // if (selection.Contains(parentWall))
                    // {
                    //     highlight.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                    // }

                    foreach (Transform sel in selection)
                    {
                        Transform activeChild = GetActiveChild (sel);
                        if (activeChild.GetComponent<MeshRenderer>().material != selectionMaterial)
                        {
                            originalMaterialSelection = GetOriginalMaterial (activeChild.name);

                            selectionMaterial.mainTextureScale = originalMaterialSelection.mainTextureScale;
                            foreach (Material mat in activeChild.GetComponent<MeshRenderer>().materials)
                                mat.Lerp(selectionMaterial, originalMaterialSelection, 0.5f);


                            // runtimeTransformHandle.target = highlight;

                            // COULD BE INTERESTING TO FIGURE OUT IF INTERESTING: 
                            // runtimeTransformGameObj.SetActive(true);
                        }
                        }
                    highlight = null;
                // }
                // else
                // {
                //     if (selection)
                //     {
                //         selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                //         selection = null;

                //         runtimeTransformGameObj.SetActive(false);
                //     }
                // }
            }
            else
            {
                // if (selection)
                // {
                //     selection.GetComponent<MeshRenderer>().material = originalMaterialSelection;
                //     selection = null;

                //     runtimeTransformGameObj.SetActive(false);
                // }
            }
        }

        // //Hot Keys for move, rotate, scale, local and Global/World transform
        // if (runtimeTransformGameObj.activeSelf)
        // {
        //     if (Input.GetKeyDown(KeyCode.W))
        //     {
        //         runtimeTransformHandle.type = HandleType.POSITION;
        //     }
        //     if (Input.GetKeyDown(KeyCode.E))
        //     {
        //         runtimeTransformHandle.type = HandleType.ROTATION;
        //     }
        //     if (Input.GetKeyDown(KeyCode.R))
        //     {
        //         runtimeTransformHandle.type = HandleType.SCALE;
        //     }
        //     if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        //     {
        //         if (Input.GetKeyDown(KeyCode.G))
        //         {
        //             runtimeTransformHandle.space = HandleSpace.WORLD;
        //         }
        //         if (Input.GetKeyDown(KeyCode.L))
        //         {
        //             runtimeTransformHandle.space = HandleSpace.LOCAL;
        //         }
        //     }
        // }

    }

    private Transform GetActiveChild (Transform wall)
    {
            foreach (Transform child in wall)
                if (child.gameObject.activeSelf)
                    return child;

        Debug.LogError ("NO ACTIVE CHILD OBJECT");
        return wall.GetChild(0).transform;
    }

    private Material GetOriginalMaterial (string activeChildName)
    {
        foreach (UnityEngine.Material mat in geometryManager.visualMaterials)
        {
            if (mat.name == activeChildName.Split('_')[1])
            {
                return mat;
            }

        }

        Debug.LogError ("NO MATERIAL SELECTED!");
        return geometryManager.visualMaterials[0];
    }
    private void ApplyLayerToChildren(GameObject parentGameObj)
    {
        foreach (Transform transform1 in parentGameObj.transform)
        {
            int layer = parentGameObj.layer;
            transform1.gameObject.layer = layer;
            foreach (Transform transform2 in transform1)
            {
                transform2.gameObject.layer = layer;
                foreach (Transform transform3 in transform2)
                {
                    transform3.gameObject.layer = layer;
                    foreach (Transform transform4 in transform3)
                    {
                        transform4.gameObject.layer = layer;
                        foreach (Transform transform5 in transform4)
                        {
                            transform5.gameObject.layer = layer;
                        }
                    }
                }
            }
        }
    }

}