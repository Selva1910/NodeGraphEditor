using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace NodeGraph.Editor
{
    [CustomEditor(typeof(GraphAssetSO))]
    public class GraphAssetEditor : UnityEditor.Editor
    {

        [OnOpenAsset]
        public static bool OnOpenAsset(int instancId, int index)
        {
            Object asset = EditorUtility.InstanceIDToObject(instancId);
            if (asset.GetType() == typeof(GraphAssetSO))
            {
                GraphEditorWindow.Open((GraphAssetSO)asset);
            }
            return false;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                GraphEditorWindow.Open((GraphAssetSO)target);
            }
        }
    }
}
