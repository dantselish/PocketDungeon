using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloorGeneratorTool))]
public class FloorGeneratorToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FloorGeneratorTool tool = (FloorGeneratorTool) target;
        if (GUILayout.Button("Generate"))
        {
            tool.Generate();
        }
    }
}
