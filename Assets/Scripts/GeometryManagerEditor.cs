using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;
using UnityEditor;
using SteamAudio;
using Siccity.GLTFUtility;
using Vector3 = UnityEngine.Vector3;
using UnityEditor.SceneManagement;
[CustomEditor(typeof(GeometryManager))]
public class GeometryManagerEditor : Editor
{
    private string filepath = "./Assets/Models/10mcube.gltf";
    public GameObject _loadedGameObject;

    List<UnityEngine.Material> visualMaterials = new List<UnityEngine.Material>();
    List<SteamAudioMaterial> steamAudioMaterials = new List<SteamAudioMaterial>();
    private void Awake() 
    {
        // Collect visual materials
        string[] visualMaterialFiles = System.IO.Directory.GetFiles("Assets/Materials", "*.mat", SearchOption.TopDirectoryOnly);

        foreach (var file in visualMaterialFiles)
        {
            visualMaterials.Add(AssetDatabase.LoadAssetAtPath(file, typeof(UnityEngine.Material)) as UnityEngine.Material);
        }

        // Collect Steam Audio materials
        string[] steamAudioMaterialFiles = System.IO.Directory.GetFiles("Assets/Plugins/SteamAudio/Resources/Materials", "*.asset", SearchOption.TopDirectoryOnly);

        foreach (var file in steamAudioMaterialFiles)
        {
            steamAudioMaterials.Add(AssetDatabase.LoadAssetAtPath(file, typeof(SteamAudioMaterial)) as SteamAudioMaterial);
        }

    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space (10);
        if (GUILayout.Button ("Load New Model"))
        {
            RegenerateDynamicObjects();
            Debug.Log("Load model here!");
        }
    }

    private Vector3 Average(MeshFilter [] _meshFilterArray){

        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;
        int l = _meshFilterArray.Length;
        foreach (MeshFilter m in _meshFilterArray){
           x += m.sharedMesh.bounds.center.x;
           y += m.sharedMesh.bounds.center.y;
           z += m.sharedMesh.bounds.center.z;
        }
        return new Vector3( x / l , y / l, z / l);
    }

    public void ImportProcedure()
    {
        
        if (_loadedGameObject != null)
        {
            // Regenerate temporary folder with Dynamic Object Assets
            UnityEngine.Windows.Directory.Delete("Assets/DynamicObjects/TempFolder");
            UnityEngine.Windows.Directory.CreateDirectory("Assets/DynamicObjects/TempFolder");

            // Get Meshfilters
            MeshFilter[] mff = _loadedGameObject.GetComponentsInChildren<MeshFilter>();

            Debug.Log("found this number of meshFilter groups : " + mff.Length);

            // Initialise every mesh filter with all possible materials
            int counter = 0;
            foreach (MeshFilter mf in mff)
            {
                // Add components to the imported meshfilter
                GameObject go = mf.gameObject;

                var name = go.name;
                if (name == "Room")
                    continue;

                // Add meshcollider
                go.AddComponent(typeof(MeshCollider));
                go.layer = 10;

                //// Add children to each mesh: one per material ////
                string activeMaterial = GetMaterialBasedOnMeshName (go.name);

                bool firstChild = true;
                foreach (SteamAudioMaterial steamAudioMaterial in steamAudioMaterials)
                {
                    // Add child to mesh
                    GameObject goChild = Instantiate<GameObject>(firstChild ? mf.gameObject : mf.transform.GetChild(0).gameObject, mf.gameObject.transform);
                    goChild.name = counter + "_" + steamAudioMaterial.name;

                    // Add Steam Audio Components

                    if (firstChild)
                    {
                        // Add SteamAudioGeometry component
                        goChild.AddComponent (typeof(SteamAudioGeometry));
                        
                        // Add dynamic object component
                        goChild.AddComponent (typeof (SteamAudioDynamicObject));
                        
                        firstChild = false;
                    }

                    // Set active so we can do things with it
                    goChild.SetActive (true);

                    //// EDIT VISUAL MATERIAL ////
                    
                    // Get the visual material name from the steam audio material
                    string visualMaterialName = GetMaterialFromName (steamAudioMaterial.name);
                    EditMeshRendererMaterial (goChild, visualMaterialName);

                    // Set the Steam Audio Material property of the SteamAudioGeometry
                    SetSteamAudioMaterial (goChild, steamAudioMaterial);

                    // Crate the SerializedData for the SteamAudioDynamicObject
                    CreateDynamicObjectSerializedData (goChild, name, counter);

                    // Export the dynamic object
                    SteamAudioManager.ExportDynamicObject (goChild.GetComponent<SteamAudioDynamicObject>(), false);

                    goChild.SetActive(steamAudioMaterial.name == activeMaterial);
                }
                mf.GetComponent<MeshRenderer>().enabled = false;
                ++counter;
            }

            Vector3 average = Average(mff);

            _loadedGameObject.transform.GetChild(0).position = new Vector3 (-average.x, 0, -average.z);
        }
        else
        {
            Debug.Log("loaded null");
        }
    }

    private void EditMeshRendererMaterial(GameObject goChild, string visualMaterialName)
    {
        // Serialize the material of the mesh renderer of the newly instantiated child
        UnityEditor.SerializedObject meshRenderer = new UnityEditor.SerializedObject (goChild.GetComponent<MeshRenderer>());
        var matsSO = meshRenderer.FindProperty("materials");

        // Set the visual material of the mesh renderer to the one 
        foreach (UnityEngine.Material mat in visualMaterials)
            if (mat.name == visualMaterialName)
                goChild.GetComponent<MeshRenderer>().sharedMaterial = mat;

        meshRenderer.ApplyModifiedProperties();

    }
    private void SetSteamAudioMaterial (GameObject goChild, SteamAudioMaterial newMaterialAsset)
    {
        // Make the component a SerializedObject
        UnityEditor.SerializedObject dynGeometry = new UnityEditor.SerializedObject (goChild.GetComponent<SteamAudioGeometry>());

        // Apply the material asset to the Material field 
        SerializedProperty matAsset = dynGeometry.FindProperty("material");        
        matAsset.objectReferenceValue = newMaterialAsset;
        
        // Apply the properties to the serializedobject
        dynGeometry.ApplyModifiedProperties();

    }

    private void CreateDynamicObjectSerializedData (GameObject goChild, string goName, int counter)
    {  
        // Create dynamic object asset ...
        string filename = "Assets/DynamicObjects/TempFolder/" + goChild.name + "_" + goName + ".asset";
        var dynObj = new UnityEditor.SerializedObject (goChild.GetComponent<SteamAudioDynamicObject>());
        SerializedProperty mAsset = dynObj.FindProperty("asset");

        // ... and add it to the dynamic object
        var dataAsset = ScriptableObject.CreateInstance<SerializedData>();
        AssetDatabase.CreateAsset(dataAsset, filename);
        mAsset.objectReferenceValue = dataAsset;
        dynObj.ApplyModifiedProperties();

    }
    public string GetMaterialBasedOnMeshName (string meshname)
    {
        switch (meshname)
        {
            case "Transparent":
                return "Transparent";
            case "AcousticCeilingTiles":
                return "Default";
            
            case "BrickBare":
            case "BrickPainted":
                return "Brick";
            
            case "ConcreteBlockCoarse":
            case "ConcreteBlockPainted":
            case "PolishedConcreteOrTile":
                return "Concrete";

            case "CurtainHeavy":
                return "Carpet";

            case "FiberglassInsulation":
                return "Default";

            case "GlassThin":
            case "GlassThick":
                return "Glass";

            case "Grass":
                return "Gravel"; //?;

            case "LinoleumOnConcrete":
                return "Default";
                
            case "Marble":
                return "Ceramic"; //?

            case "Metal":
                return "Metal";

            case "ParquetOnConcrete":
                return "Default";

            case "PlasterRough":
            case "PlasterSmooth":
                return "Plaster";

            case "PlywoodPanel":
                return "Default";

            case "Sheetrock":
                return "Rock";

            case "WaterOrIceSurface":
                return "Default";

            case "WoodCeiling":
            case "WoodPanel":
                return "Wood";

            case "Uniform":
                return "Default";

            case "CustomLeft":
            case "CustomRight":
            case "CustomUp":
            case "CustomDown":
            case "CustomFront":
            case "CustomBack":
                return "Default";

            default:
                Debug.LogError("Unknown Material: " + meshname);
                return "Default";
        }

    }

    public void RegenerateDynamicObjects()
    {
        GameObject root = GameObject.FindGameObjectWithTag("Model");
        if (root != null)
            DestroyImmediate(root);

        // TEMPORARY FOR WORKFLOW
        string filepath = EditorUtility.OpenFilePanel("Load GLTF", "", "gltf");
        // string filepath = "./Assets/Models/TrappenZaal.gltf";
        _loadedGameObject = Importer.LoadFromFile(filepath);
        _loadedGameObject.tag = "Model";
        ImportProcedure();
    }

    public string GetMaterialFromName(string name)
    {
        switch (name)
        {
            case "Brick":
            case "Carpet":
            case "Concrete":
            case "Glass":
            case "Gravel":
            case "Metal":
            case "Wood":
                return name;
            default:
                return "Default";
        }
    }

}
