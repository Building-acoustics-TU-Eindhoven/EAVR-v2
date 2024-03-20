using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WallSelectionHandler : MonoBehaviour
{
    public TMP_Text selectedWallText;
    public TMP_Text selectedMaterialText;
    public TMP_Dropdown dropdown;

    // Main Menu (for updating Wall button based on if any walls are selected)
    public MenuManager menuManager;

    // Reference to the geometry manager for aApplying the selected material to the selected walls
    private GeometryManager geometryManager;

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
            // Convert mouse position to ray
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the ray hit..
            if (Physics.Raycast(ray, out hit, 100))
            {
                // .. a wall (walls start with a number)..
                if (int.TryParse(hit.transform.name.Substring(0, 1), out int value) )
                {
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
                            selectedWalls.Remove (hit.transform.parent);
                        else
                        {
                            selectedWalls.Clear();
                            selectedWalls.Add (hit.transform.parent);
                        }
                    }
                    else
                    {
                        // If shift is not down, clear wall list first
                        if (!shiftDown)
                            selectedWalls.Clear();

                        // Add the selected wall to the list
                        selectedWalls.Add (hit.transform.parent);
                    }

                    // Update UI elements based on number of selected walls
                    switch (selectedWalls.Count)                       
                    {
                        case 0:
                        {
                            selectedWallText.text = "Selected wall: None";
                            selectedMaterialText.text = "Selected material: None";
                            menuManager.HasSelectedWalls (false);
                            break;
                        }

                        case 1: 
                        {
                            selectedWallText.text = "Selected wall: " + selectedWalls[0].name;
                            selectedMaterialText.text = "Selected material: " + geometryManager.GetActiveMaterialOf(0);
                            menuManager.HasSelectedWalls (true);
                            break;
                        }
                        default:
                        {
                            selectedWallText.text = "Selected wall: Multiple (" + selectedWalls.Count + ")";
                            selectedMaterialText.text = "Selected material: Multiple (" + selectedWalls.Count + ")";        
                            menuManager.HasSelectedWalls (true);
                            break;
                        }
                    }
                }
            }
        }
    }
}
