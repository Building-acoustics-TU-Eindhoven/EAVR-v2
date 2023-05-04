using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GeometryManager))]
public class GeometryManagerEditor : Editor
{
    private void Awake() 
    {
        (target as GeometryManager).CollectMaterials();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space (10);
        if (GUILayout.Button ("Load New Model"))
        {
            (target as GeometryManager).RegenerateDynamicObjects();
            Debug.Log("Load model here!");
        }
    }
}
