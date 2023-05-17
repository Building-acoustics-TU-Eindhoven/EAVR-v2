using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EvaluationManager : MonoBehaviour
{
    public List<string> items;
    [SerializeField] private List<float> scores;
    [SerializeField] private string note = "";

    private Transform viewportContent;

    void Start()
    {
        viewportContent = GlobalFunctions.GetChildWithName(this.gameObject, "Scroll View").transform.GetChild(0).GetChild(0);

        int i = 0;
        float itemHeight = viewportContent.GetChild(0).GetComponent<RectTransform>().rect.height;
        viewportContent.GetComponent<RectTransform>().sizeDelta = new Vector2(viewportContent.GetComponent<RectTransform>().rect.width, (items.Count) * itemHeight);
        // foreach (string item in items)
        // {
        //     Instantiate(viewportContent.GetChild(0), viewportContent.transform);
        //     GameObject newItem = viewportContent.GetChild(viewportContent.childCount - 1).gameObject;
        //     GlobalFunctions.GetChildWithName(newItem, "Slider").GetComponent<OnValueChanged>().SetItemIdx(i);
        //     GlobalFunctions.GetChildWithName(newItem, "Title").GetComponent<TextMeshProUGUI>().text = item;
        //     scores.Add (50.0f);

        //     ++i;
        // }

        viewportContent.GetChild(0).gameObject.SetActive(false);

        ScrollViewCallback();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScrollViewCallback()
    {
        float itemHeight = viewportContent.GetChild(0).GetComponent<RectTransform>().rect.height;
        for (int childIdx = 0; childIdx < viewportContent.childCount; ++childIdx)
        {
            Transform child = viewportContent.GetChild(childIdx);
            child.GetComponent<RectTransform>().localPosition = new Vector3(child.GetComponent<RectTransform>().localPosition.x,
                                                                             -(childIdx-1) * itemHeight,
                                                                             child.GetComponent<RectTransform>().localPosition.z);

        }

    }

    public void SetItemValue(float val, int itemIdx)
    {
        scores[itemIdx] = val;
    }

    public void SetNote(string noteIn)
    {
        note = noteIn;
    }
    
}
