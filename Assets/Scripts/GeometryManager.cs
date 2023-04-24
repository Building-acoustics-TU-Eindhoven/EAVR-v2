using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using SteamAudio;
using Vector3 = UnityEngine.Vector3;
public class GeometryManager : MonoBehaviour
{
    int curMaterial = 0;
    public TMP_Dropdown dropdown;
    public GameObject audioSourceLocation;
    public AudioSource audioSource;

    int totMaterials = -1;

    bool[] shouldChangeWallMaterial = new bool[6] { true, true, true, true, true, true };
    float scale = 1.0f;

    private List<Transform> selectedWalls = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        // SetActiveMaterial();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetActiveMaterial()
    {

        if (selectedWalls.Count == 0)
            return;
            
        if (totMaterials == -1)
            totMaterials = selectedWalls[0].childCount;

        foreach (Transform selectedWall in selectedWalls)
        {
            int i = 0;
            foreach (Transform child in selectedWall)
            {
                if (i == curMaterial)
                    child.gameObject.SetActive(true);
                else if (i != selectedWall.childCount-1)
                    child.gameObject.SetActive(false);
                ++i;
            }
        }
        /// Changing all materials ///
        // GameObject root = GameObject.Find("root");
        // foreach (Transform child in root.transform.GetChild(0).transform)
        // {
        //     if (totMaterials == -1)
        //         totMaterials = child.childCount;

        //     for (int i = 0; i < totMaterials; ++i)
        //     {
        //         if (i == curMaterial)
        //             child.GetChild(i).gameObject.SetActive(true);
        //         else
        //             child.GetChild(i).gameObject.SetActive(false);
        //     }
        // }
        // return;

        /// Walls in geometry manager (old) ///
        // int j = 0;
        // foreach (Transform child in transform)
        // {
        //     if (child.tag == "Wall" && shouldChangeWallMaterial[j])
        //     {
        //         if (totMaterials == -1)
        //             totMaterials = child.childCount;
        //         for (int i = 0; i < totMaterials; ++i)
        //         {
        //             if (i == curMaterial)
        //                 child.GetChild(i).gameObject.SetActive(true);
        //             else
        //                 child.GetChild(i).gameObject.SetActive(false);
        //         }
        //     }
        //     ++j;
        // }

    }

    public void SetShouldChangeWallMaterial(int wall, bool shouldChange)
    {
        shouldChangeWallMaterial[wall] = shouldChange;
    }

    public void ChangeMaterial()
    {
        curMaterial = (curMaterial + 1) % totMaterials;
        SetActiveMaterial();
    }

    public void ChangeMaterial(int mat)
    {
        Debug.Log("Changing Material");
        // first option is "Choose material..
        if (mat == 0)
            return;

        if (selectedWalls.Count == 0)
        {
            Debug.LogWarning ("Please select a wall first!");
            return;
        }

        curMaterial = mat - 1;
        SetActiveMaterial();
    }


    public void ChangeSize(float multiplier)
    {
        scale = multiplier;
        this.transform.localScale = new Vector3(scale, scale, scale);
        audioSource.transform.position = audioSourceLocation.transform.position;
    }

    public void ClearSelectedWalls()
    {
        selectedWalls.Clear();
    }

    public void AddSelectedWall (Transform s)
    {
        selectedWalls.Add(s);
    }

    public List<Transform> GetSelectedWalls()
    {
        return selectedWalls;
    }
    
    public string GetActiveMaterialOf (int idx)
    {
        foreach (Transform child in selectedWalls[0])
        {
            if (child.gameObject.activeSelf)
            {
                return child.name.Substring (child.name.LastIndexOf('_') + 1);
            }
        }
        return "";
    }
}
