using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerRayCast : MonoBehaviour
{
    // Start is called before the first frame update

    public TMP_Text selectedWall;
    public TMP_Text selectedMaterial;
    public TMP_Dropdown dropdown;


    private GeometryManager geometryManager;

    private GameManagerSteamAudio gameManager;
    void Start()
    {
        geometryManager = GameObject.Find("GeometryManager").GetComponent<GeometryManager>();
        gameManager = GameObject.Find("MainCanvas").GetComponent<GameManagerSteamAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameManager.IsCanvasActive())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (int.TryParse(hit.transform.name.Substring(0, 1), out int value) )
                {
                    Debug.Log("Hit child " + hit.transform.name);
                    selectedWall.text = "Selected wall: " + hit.transform.parent.name;
                    selectedMaterial.text = "Selected material: " + hit.transform.name.Substring(hit.transform.name.LastIndexOf('_') + 1);
                    for (int o = 1; o < dropdown.options.Count; ++o)
                    {
                        if (dropdown.options[o].text == hit.transform.name.Substring (hit.transform.name.LastIndexOf('_') + 1))
                        {
                            Debug.Log(dropdown.options[o].text.Substring(dropdown.options[o].text.LastIndexOf('_') + 1));
                            dropdown.SetValueWithoutNotify (o);
                        }
                    }
                    geometryManager.SetSelectedWall (hit.transform.parent);
                    hit.transform.parent.GetChild(hit.transform.parent.childCount - 1).gameObject.SetActive(true);
                    foreach (Transform child in hit.transform.parent.parent)
                    {
                        if (child != hit.transform.parent)
                            child.GetChild(hit.transform.parent.childCount - 1).gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
