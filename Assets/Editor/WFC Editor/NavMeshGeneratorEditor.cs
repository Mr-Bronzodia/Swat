using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(NavMeshGenerator))]
public class NavMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NavMeshGenerator generator = (NavMeshGenerator)target;

        EditorGUILayout.Space(15f);
        EditorGUILayout.LabelField("Actions", GUILayout.Width(45f));
        EditorGUILayout.Space(15f);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate")) generator.GenerateEditorOnly();
        EditorGUILayout.EndHorizontal();
    }
}

#endif
