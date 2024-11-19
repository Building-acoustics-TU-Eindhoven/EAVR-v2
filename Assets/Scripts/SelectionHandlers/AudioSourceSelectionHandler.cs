using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class AudioSourceSelectionHandler : SelectionHandler
{
    [SerializeField]
    private GameObject playerGO;

    [SerializeField]
    private InWorldSelectionHandler inWorldSelectionHandler;
    
    [SerializeField]
    private GameObject audioSource;
    
    [SerializeField]
    private Material selectionMaterial;

    [SerializeField]
    private Material hoverMaterial;

    private List<GameObject> gameObjectsWithMeshRenderer = new List<GameObject>();
    private List<string[]> originalMaterialNames = new List<string[]>();

    public List<Material> originalMaterials;

    private bool isHighlighted = false;

    void Start()
    {
        GlobalFunctions.GetChildrenWithComponent<MeshRenderer> (audioSource, ref gameObjectsWithMeshRenderer);
        foreach (GameObject go in gameObjectsWithMeshRenderer)
        {
            string[] materials = new string[go.GetComponent<MeshRenderer>().materials.Length];
            for (int i = 0; i < materials.Length; ++i)
                materials[i] = go.GetComponent<MeshRenderer>().materials[i].name;

            originalMaterialNames.Add (materials);
        }

    }

    public override void SetPositionBasedOnHit (RaycastHit hit)
    {
        var test = Quaternion.LookRotation(hit.point - playerGO.transform.position);
        test.eulerAngles = new Vector3 (0.0f, test.eulerAngles.y + 180.0f, 0.0f);

        if (GlobalFunctions.shouldUseInWorldMenu)
        {

            inWorldSelectionHandler.transform.rotation = test;
            inWorldSelectionHandler.transform.position = new Vector3(
                audioSource.transform.position.x,
                hit.collider.bounds.max.y + 0.5f * inWorldSelectionHandler.GetComponent<RectTransform>().rect.height * inWorldSelectionHandler.GetComponent<RectTransform>().localScale.y,
                audioSource.transform.position.z);
            Highlight();
            inWorldSelectionHandler.gameObject.SetActive(isHighlighted);
        }

        // Somehow the SetActive function (perhaps because it is a UI element) makes the cursor visible. This makes sure that the cursor stays Locked and thereby invisible.
        Cursor.lockState = CursorLockMode.Locked;

        base.SetPositionBasedOnHit (hit);
    }

    public void Highlight()
    {
        isHighlighted = !isHighlighted;

        if (!isHighlighted)
        {
            for (int i = 0; i < gameObjectsWithMeshRenderer.Count; ++i)
            {
                gameObjectsWithMeshRenderer[i].GetComponent<MeshRenderer>().materials = GetMaterialsFromStringArray(originalMaterialNames[i]);
            } 
        } else {     
            for (int i = 0; i < gameObjectsWithMeshRenderer.Count; ++i)
            {
                for (int j = 0; j < originalMaterialNames[i].Length; ++j)
                {
                    Material originalMat = GetMaterialsFromStringArray(originalMaterialNames[i])[j];
                    gameObjectsWithMeshRenderer[i].GetComponent<MeshRenderer>().materials[j].Lerp(selectionMaterial, originalMat, 0.5f);
                }
            }
        }

    }

    public void Hover (bool shouldHover)
    {
        if (isHighlighted)
            return; 

        if (!shouldHover)
        {
            for (int i = 0; i < gameObjectsWithMeshRenderer.Count; ++i)
            {
                gameObjectsWithMeshRenderer[i].GetComponent<MeshRenderer>().materials = GetMaterialsFromStringArray(originalMaterialNames[i]);
            } 
        } else {     
            for (int i = 0; i < gameObjectsWithMeshRenderer.Count; ++i)
            {
                for (int j = 0; j < originalMaterialNames[i].Length; ++j)
                {
                    Material originalMat = GetMaterialsFromStringArray(originalMaterialNames[i])[j];
                    gameObjectsWithMeshRenderer[i].GetComponent<MeshRenderer>().materials[j].Lerp(hoverMaterial, originalMat, 0.5f);
                }
            }
        }


    }

    Material[] GetMaterialsFromStringArray (string[] materialsNames)
    {
        Material[] materialsToReturn = new Material[materialsNames.Length];
        int j = 0;
        foreach (string name in materialsNames)
        {
            foreach (Material mat in originalMaterials)
                if (name.Split(" (")[0] == mat.name)
                {
                    materialsToReturn[j] = mat;
                    ++j;
                }
        }

        return materialsToReturn;
    }

    public override void Deselect()
    {
        if (isHighlighted)
            Highlight();
    }


}
