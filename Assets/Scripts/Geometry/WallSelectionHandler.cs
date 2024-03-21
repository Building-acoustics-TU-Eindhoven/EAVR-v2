using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class WallSelectionHandler : MonoBehaviour
{
    // UI elements in the Wall Menu
    public TMP_Text selectedWallText;
    public TMP_Text selectedMaterialText;
    public TMP_Dropdown dropdown;

    // The Unity material with which we interpolate the original material to show that it is selected (currently, makes the original material more red)
    [SerializeField]
    private Material selectionMaterial;

    // Main Menu (for updating Wall button based on if any walls are selected)
    public MenuManager menuManager;

    // Reference to the geometry manager for aApplying the selected material to the selected walls
    private GeometryManager geometryManager;

    // Reference to the geometry manager for aApplying the selected material to the selected walls
    public GameObject inWorldMenu;

    // List of walls that are currently selected
    private List<Transform> selectedWalls = new List<Transform>();

    // Keyboard modifiers 
    private bool shiftDown, leftShiftDown, rightShiftDown = false;

    void Start()
    {
        // Find the geometry manager
        geometryManager = GameObject.Find("GeometryManager").GetComponent<GeometryManager>();

        // Make sure that the geometry manager also has access to the selected walls list by passing it as a reference
        geometryManager.SetSelectedWallRef (ref selectedWalls);
    }

    // Update is called once per frame
    void Update()
    {

        // Handle mouse modifiers
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

        shiftDown = leftShiftDown || rightShiftDown;

        // If the menu manager is not active and the left mouse button is clicked, check whether the mouse position ray intersects with a wall
        if (!menuManager.IsMenuActive() && Input.GetMouseButtonDown(0))
        {
            bool hasAddedWallToSelection = false;

            // Convert mouse position to ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the ray hit..
            if (Physics.Raycast(ray, out hit, 100))
            {
                // .. a wall (walls start with a number)..
                if (int.TryParse(hit.transform.name.Substring(0, 1), out int value) )
                {
                    // inWorldMenu.transform.position = hit.point;

                    // .. set the material dropdown to the currently active material of this wall.
                    Debug.Log("Hit child " + hit.transform.name);
                    for (int o = 1; o < dropdown.options.Count; ++o)
                    {
                        if (dropdown.options[o].text == hit.transform.name.Substring (hit.transform.name.LastIndexOf('_') + 1))
                        {
                            Debug.Log(dropdown.options[o].text.Substring(dropdown.options[o].text.LastIndexOf('_') + 1));
                            dropdown.SetValueWithoutNotify (o);
                        }
                    }

                    // If wall is already selected remove it from the list
                    if (selectedWalls.Contains (hit.transform.parent))
                    {
                        if (shiftDown || selectedWalls.Count == 1)
                        {
                            DeselectWall (hit.transform.parent, false);
                        }
                        else
                        {
                            ClearSelection();
                            SelectWall (hit.transform.parent);
                            hasAddedWallToSelection = true;
                        }
                    }
                    else
                    {
                        // If shift is not down, clear wall list first
                        if (!shiftDown)
                            ClearSelection();

                        // Add the selected wall to the list
                        SelectWall (hit.transform.parent);
                        hasAddedWallToSelection = true;
                    }

                    // Update UI elements based on number of selected walls
                    switch (selectedWalls.Count)                       
                    {
                        case 0:
                        {
                            selectedWallText.text = "Selected wall: None";
                            selectedMaterialText.text = "Selected material: None";
                            break;
                        }

                        case 1: 
                        {
                            selectedWallText.text = "Selected wall: " + selectedWalls[0].name;
                            selectedMaterialText.text = "Selected material: " + geometryManager.GetActiveMaterialOf(0);
                            break;
                        }
                        default:
                        {
                            selectedWallText.text = "Selected wall: Multiple (" + selectedWalls.Count + ")";
                            selectedMaterialText.text = "Selected material: Multiple (" + selectedWalls.Count + ")";        
                            break;
                        }
                    }
                    if (hasAddedWallToSelection)
                    {
                        inWorldMenu.transform.rotation = Quaternion.LookRotation(hit.normal);
                        // inWorldMenu.transform.position = ray.GetPoint (Vector3.Distance (ray.origin, hit.point) - 1);
                        inWorldMenu.transform.position = hit.point + hit.normal * 0.01f;
                    }
                    inWorldMenu.SetActive (hasAddedWallToSelection);
                    menuManager.HasSelectedWalls (selectedWalls.Count != 0);

                    Cursor.lockState = CursorLockMode.Locked;

                }
            }
        }
    }

    private void SelectWall (Transform wall)
    {
        Transform activeChild = GetActiveChild (wall);

        Material originalMaterialSelection = GetOriginalMaterial (activeChild.name);

        selectionMaterial.mainTextureScale = originalMaterialSelection.mainTextureScale;

        // Interpolate all faces with the selection material (red)
        foreach (Material mat in activeChild.GetComponent<MeshRenderer>().materials)
            mat.Lerp(selectionMaterial, originalMaterialSelection, 0.5f);

        selectedWalls.Add (wall);
    }

    private void DeselectWall (Transform wall, bool clearingAll)
    {
        // .. get the active child
        Transform activeChild = GetActiveChild (wall);

        // .. one wall can have multiple faces and therefore multiple (identical) materials
        Material[] materials = new Material[activeChild.GetComponent<MeshRenderer>().materials.Length];
        for (int i = 0; i < activeChild.GetComponent<MeshRenderer>().materials.Length; ++i)
            materials[i] = GetOriginalMaterial (activeChild.name);
        
        // Set all the materials back to the orignal material
        activeChild.GetComponent<MeshRenderer>().materials = materials;

        if (!clearingAll)
            selectedWalls.Remove (wall);
    }

    private void ClearSelection()
    {
        // For each selected wall..
        foreach (Transform sel in selectedWalls)
            // .. deselect it, and set the original material
            DeselectWall (sel, true);

        selectedWalls.Clear();

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
        foreach (Material mat in geometryManager.visualMaterials)
        {
            if (mat.name == activeChildName.Split('_')[1])
            {
                return mat;
            }

        }

        Debug.LogError ("NO MATERIAL SELECTED!");
        return geometryManager.visualMaterials[0];
    }


}
