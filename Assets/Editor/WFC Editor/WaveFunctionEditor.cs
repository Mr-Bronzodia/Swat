using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WaveFunctionCollapse))]
public class WaveFunctionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WaveFunctionCollapse generator = (WaveFunctionCollapse)target;

        EditorGUILayout.Space(15f);
        EditorGUILayout.LabelField("Actions", GUILayout.Width(45f));
        EditorGUILayout.Space(15f);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate")) generator.GenerateTilemap();
        if (GUILayout.Button("Destroy")) generator.DestroyGrid();
        EditorGUILayout.EndHorizontal();

    }
}

#endif