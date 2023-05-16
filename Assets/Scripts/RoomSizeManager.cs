using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomSizeManager : MonoBehaviour
{
    public GameObject root;
    // public SourceManager sourceManager;
    public GameObject playerGO;
    public PlayerManager playerManager;
    public AudioSourceManager audioSourceManager;
    public GameObject roomsizeGO;
    // public SpeakerWallManager speakerWallManager;

    private TMP_InputField xText, yText, zText;
    private float prevXval, prevYval, prevZval;
    private Slider xSlider, ySlider, zSlider;

    private float sourceX, sourceY, sourceZ;
    private float playerX = 0.5f;
    private float playerZ = 0.5f;

    private List<Vector3> sourcePositions = new List<Vector3>();

    public float roomWidth, roomHeight, roomDepth;

    private Vector3 originalSize;

    Bounds getRenderBounds(GameObject objeto){
        Bounds bounds = new  Bounds(Vector3.zero,Vector3.zero);
        Renderer render = objeto.GetComponent<Renderer>();
        if(render!=null){
            return render.bounds;
        }
        return bounds;
    }

    public Bounds getBounds(GameObject objeto){
        Bounds bounds;
        Renderer childRender;
        bounds = getRenderBounds(objeto);
        if(bounds.extents.x == 0){
            bounds = new Bounds(objeto.transform.position,Vector3.zero);
            foreach (Transform child in objeto.transform) {
                childRender = child.GetComponent<Renderer>();
                if (childRender) {
                    bounds.Encapsulate(childRender.bounds);
                }else{
                    bounds.Encapsulate(getBounds(child.gameObject));
                }
            }
        }
        return bounds;
    }

    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("root");
        Bounds b = getBounds(root);
        // Bounds b = root.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds;

        // Vector3 offset = b.center;
        originalSize = b.size;
        roomWidth = originalSize.x;
        roomHeight = originalSize.y;
        roomDepth = originalSize.z;

        xText = GlobalFunctions.GetChildWithName(roomsizeGO, "xValue").GetComponent<TMP_InputField>();
        yText = GlobalFunctions.GetChildWithName(roomsizeGO, "yValue").GetComponent<TMP_InputField>();
        zText = GlobalFunctions.GetChildWithName(roomsizeGO, "zValue").GetComponent<TMP_InputField>();

        xSlider = GlobalFunctions.GetChildWithName(roomsizeGO, "xSlider").GetComponent<Slider>();
        ySlider = GlobalFunctions.GetChildWithName(roomsizeGO, "ySlider").GetComponent<Slider>();
        zSlider = GlobalFunctions.GetChildWithName(roomsizeGO, "zSlider").GetComponent<Slider>();

        SetPlayerTransform();
        audioSourceManager.SetOriginalRoomDimensions (roomWidth, roomHeight, roomDepth);
    }

    public void SetRoomSizeThroughSliderVals(float x, float y, float z)
    {
        Debug.Log(x + " " + y + " " + z);
        xSlider.SetValueWithoutNotify(x);
        SetRoomX(x);
        ySlider.SetValueWithoutNotify(y);
        SetRoomY(y);
        zSlider.SetValueWithoutNotify(z);
        SetRoomZ(z);
    }

    public void SetRoomSizeSliders (Vector3 sizeToSet)
    {
        xSlider.SetValueWithoutNotify(sizeToSet.x);
        xText.text = sizeToSet.x.ToString("0.0");
        roomWidth = sizeToSet.x;

        ySlider.SetValueWithoutNotify(sizeToSet.y);
        yText.text = sizeToSet.y.ToString("0.0");
        roomHeight = sizeToSet.y;

        zSlider.SetValueWithoutNotify(sizeToSet.z);
        zText.text = sizeToSet.z.ToString("0.0");
        roomDepth = sizeToSet.z;

    }

    public void SetRoomXFromString (string x)
    {
        float res;
        if (float.TryParse(x, out res))
        {
            res = Mathf.Max(1.0f, res);
            xSlider.SetValueWithoutNotify(res);
            SetRoomX(res);
        }
        else
        {
            xText.text = roomWidth.ToString("0.0");
        }

    }

    public void SetRoomYFromString(string y)
    {
        float res;
        if (float.TryParse(y, out res))
        {
            res = Mathf.Max(1.0f, res);
            ySlider.SetValueWithoutNotify(res);
            SetRoomY(res);
            prevYval = res;
        }
        else
        {
            yText.text = roomHeight.ToString("0.0");
        }
    }

    public void SetRoomZFromString(string z)
    {
        float res;
        if (float.TryParse(z, out res))
        {
            res = Mathf.Max(1.0f, res);
            zSlider.SetValueWithoutNotify(res);
            SetRoomZ(res);
        }
        else
        {
            zText.text = roomDepth.ToString("0.0");
        }
    }

    public void SetRoomX (float x)
    {
        root.transform.localScale = new Vector3(x, root.transform.localScale.y, root.transform.localScale.z);
        xText.text = x.ToString("0.0");

        audioSourceManager.SetSpeakersFromRoomSizeX (x * roomWidth);
        playerManager.SetPlayerX(playerX, x * roomWidth);
        // speakerWallManager.UpdateSpeakerGrid();
    }

    public void SetRoomY(float y)
    {
        root.transform.localScale = new Vector3(root.transform.localScale.x, y, root.transform.localScale.z);
        yText.text = y.ToString("0.0");

        audioSourceManager.SetSpeakersFromRoomSizeY (y * roomHeight);
        // speakerWallManager.UpdateSpeakerGrid();
    }

    public void SetRoomZ(float z)
    {
        root.transform.localScale = new Vector3(root.transform.localScale.x, root.transform.localScale.y, z);
        zText.text = z.ToString("0.0");

        audioSourceManager.SetSpeakersFromRoomSizeZ (z * roomDepth);
        playerManager.SetPlayerZ(playerZ, z * roomDepth);
        // speakerWallManager.UpdateSpeakerGrid();
    }

    public void SetSourceX(float x)
    {
        sourceX = x;
    }

    public void SetSourceY(float y)
    {
        sourceY = y;
    }

    public void SetSourceZ(float z)
    {
        sourceZ = z;
    }

    public void SetPlayerTransform()
    {
        playerX = (playerGO.transform.position.x - root.transform.localPosition.x) / roomWidth;
        playerZ = (playerGO.transform.position.z - root.transform.localPosition.z) / roomDepth;
    }

    // public void SetSourceTransform (Vector3 pos, int sourceIdx)
    // {
    //     sourcePositions[sourceIdx] = (new Vector3 (
    //         (pos.x - root.transform.localPosition.x) / roomWidth,
    //         (pos.y - root.transform.localPosition.y) / roomHeight,
    //         (pos.z - root.transform.localPosition.z) / roomDepth)
    //     );
    // }


    // public void SetSourceTransforms()
    // {
    //     for (int i = 0; i < sourcePositions.Count; ++i)
    //         SetSourceTransform(sourcePositions[i], i);
    // }

    //public void SetPlayerY(float y)
    //{
    //    playerY = y / roomHeight + 0.5f;
    //}

    //public void SetPlayerZ(float z)
    //{
    //    playerZ = z / roomDepth + 0.5f;
    //}
}
