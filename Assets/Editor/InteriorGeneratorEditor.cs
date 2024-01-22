using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Unity.VisualScripting;



#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(InteriorGenerator))]
public class InteriorGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InteriorGenerator generator = (InteriorGenerator)target;

        EditorGUILayout.Space(15f);
        EditorGUILayout.LabelField("Actions", GUILayout.Width(45f));
        EditorGUILayout.Space(15f);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate")) generator.Generate();
        if (GUILayout.Button("Destroy")) generator.DestoryHouse();
        EditorGUILayout.EndHorizontal();
    }
  
}

#endif
