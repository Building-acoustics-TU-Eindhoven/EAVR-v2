using System.Collections;
using System.Collections.Generic;
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
            Directory.Delete("Assets/DynamicObjects/TempFolder");
            Directory.CreateDirectory("Assets/DynamicObjects/TempFolder");

            MeshFilter[] mff = _loadedGameObject.GetComponentsInChildren<MeshFilter>();

            Debug.Log("found this number of meshFilter groups : " + mff.Length);
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

                // Add Steam Audio Components
                AddGeometryAndSetMaterial (go);
                AddDynamicObjectAndCreateSerializedData (go, counter);
                
                // Export the dynamic object
                SteamAudioManager.ExportDynamicObject (go.GetComponent<SteamAudioDynamicObject>(), false);
                ++counter;
            }

            Vector3 average = Average(mff);

            _loadedGameObject.transform.GetChild(0).position = new Vector3 (-average.x, 0, -average.z);
            // get a bounding box 
            // Bounds b = _loadedGameObject.gameObject.GetComponentInChildren<MeshFilter>().mesh.bounds;

            // Vector3 offset = b.center;
            //myResonanceAudioRoom.transform.position = new Vector3 (-b.extents.x, b.extents.y, -b.extents.z);

        }
        else
        {
            Debug.Log("loaded null");
        }
    }

    public void RegenerateDynamicObjects()
    {
        GameObject root = GameObject.FindGameObjectWithTag("Model");
        if (root != null)
            DestroyImmediate(root);

        // TEMPORARY FOR WORKFLOW
        string filepath = EditorUtility.OpenFilePanel("Load GLTF", "", "gltf");
        // string filepath = "./Assets/Models/10mcube.gltf";
        _loadedGameObject = Importer.LoadFromFile(filepath);
        _loadedGameObject.tag = "Model";
        ImportProcedure();
    }

    private void AddGeometryAndSetMaterial (GameObject go)
    {
        // Name of the mesh
        string matName = GetMaterialBasedOnMeshName (go.name); 
        
        // Get material asset based on mesh name
        SteamAudioMaterial newMaterialAsset = AssetDatabase.LoadAssetAtPath<SteamAudioMaterial>
            ("Assets/Plugins/SteamAudio/Resources/Materials/" + matName + ".asset");

        // Add SteamAudioGeometry component
        go.AddComponent (typeof(SteamAudioGeometry));

        // Make the component a SerializedObject
        UnityEditor.SerializedObject dynGeometry = new UnityEditor.SerializedObject (go.GetComponent<SteamAudioGeometry>());

        // Apply the material asset to the Material field 
        SerializedProperty matAsset = dynGeometry.FindProperty("material");        
        matAsset.objectReferenceValue = newMaterialAsset;
        
        // Apply the properties to the serializedobject
        dynGeometry.ApplyModifiedProperties();

    }

    private void AddDynamicObjectAndCreateSerializedData(GameObject go, int counter)
    {
        // Add dynamic object component
        go.AddComponent (typeof(SteamAudioDynamicObject));
            // Create dynamic object asset ...
        string filename = "Assets/DynamicObjects/TempFolder/" + counter + "_" + name + ".asset";
        var dynObj = new UnityEditor.SerializedObject (go.GetComponent<SteamAudioDynamicObject>());

            // ... and add it to the dynamic object
        SerializedProperty mAsset = dynObj.FindProperty("asset");
        var dataAsset = ScriptableObject.CreateInstance<SerializedData>();
        AssetDatabase.CreateAsset(dataAsset, filename);
        if (mAsset.objectReferenceValue == null)
        {
            mAsset.objectReferenceValue = dataAsset;
            dynObj.ApplyModifiedProperties();
        }


    }
    public string GetMaterialBasedOnMeshName (string meshname)
    {
        switch (meshname)
        {
            case "Transparent":
                return "Default";
            case "AcousticCeilingTiles":
                return "Default";
            
            case "BrickBare":
            case "BrickPainted":
                return "Brick";
            
            case "ConcreteBlockCoarse":
            case "ConcreteBlockPainted":
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

            case "PolishedConcreteOrTile":
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

}
