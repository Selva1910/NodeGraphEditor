using System.Reflection;
using UnityEditor;
using UnityEngine;
using NodeGraph;

namespace NodeGraph.Editor
{
    public class NodeInspector : EditorWindow
    {
        private GraphNodeEditor m_editorNode;
        private Object m_ownerObject; // The ScriptableObject holding the nodes
        private string m_targetNodeGuid;

        public static void ShowInspector(GraphNodeEditor editorNode)
        {
            var win = GetWindow<NodeInspector>("Node Inspector");
            win.m_editorNode = editorNode;
            win.m_ownerObject = editorNode.SerializedObject.targetObject;
            win.m_targetNodeGuid = editorNode.Node.Guid; // Assuming your node has a Guid property
            win.Show();
        }

        private void OnGUI()
        {
            if (m_ownerObject == null || string.IsNullOrEmpty(m_targetNodeGuid))
            {
                GUILayout.Label("No node selected", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            // Always create a fresh SerializedObject to stay synced with the asset
            SerializedObject so = new SerializedObject(m_ownerObject);
            so.Update();

            // Find the specific node property by GUID instead of index
            SerializedProperty nodeProp = FindNodeByGuid(so, m_targetNodeGuid);

            if (nodeProp == null)
            {
                GUILayout.Label("Selected node no longer exists.");
                // If it's gone, clear the reference so we stop trying to draw it
                m_targetNodeGuid = null;
                return;
            }

            if (m_editorNode == null)
            {
                GUILayout.Label("Editor node is null.");
                return;
            }

            // Draw Header
            var nodeType = m_editorNode.Node.GetType();
            var nodeInfo = nodeType.GetCustomAttribute<NodeInfoAttribute>();
            string nodeTitle = nodeInfo != null ? nodeInfo.title : nodeType.Name;
            EditorGUILayout.LabelField($"Node: {nodeTitle}", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Draw Properties Safely
            SerializedProperty iterator = nodeProp.Copy();
            SerializedProperty end = iterator.GetEndProperty();

            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (SerializedProperty.EqualContents(iterator, end)) break;
                enterChildren = false;

                // Skip internal framework fields
                if (iterator.name == "m_guid" || iterator.name == "m_position") continue;

                try
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
                catch (System.Exception)
                {
                    // If the property disappears while drawing (e.g. undo/redo/delete)
                    GUIUtility.ExitGUI();
                }
            }

            so.ApplyModifiedProperties();
        }

        private SerializedProperty FindNodeByGuid(SerializedObject so, string guid)
        {
            SerializedProperty nodes = so.FindProperty("m_nodes");
            if (nodes == null || !nodes.isArray) return null;

            for (int i = 0; i < nodes.arraySize; i++)
            {
                SerializedProperty element = nodes.GetArrayElementAtIndex(i);
                SerializedProperty guidProp = element.FindPropertyRelative("m_guid");
                if (guidProp != null && guidProp.stringValue == guid)
                {
                    return element;
                }
            }
            return null;
        }

        public static void ClearInspector()
        {
            if (HasOpenInstances<NodeInspector>())
            {
                var win = GetWindow<NodeInspector>();
                win.m_editorNode = null;
                win.m_ownerObject = null;
                win.m_targetNodeGuid = null;
                win.Repaint();
                // win.Close(); // Optional: Close it, or just let it show "No node selected"
            }
        }
    }
}