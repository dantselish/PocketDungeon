using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextureReplacerTool))]
public class TextureReplacerToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TextureReplacerTool tool = (TextureReplacerTool) target;
        if (GUILayout.Button("Replace"))
        {
            tool.ReplaceWithNewMaterial();
        }
    }
}
