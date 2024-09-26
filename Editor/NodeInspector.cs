using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class NodeInspector : EditorWindow
    {

        static INode currentNode;
        public static void ShowInspector(INode node)
        {
            currentNode = node;
            GetWindow<NodeInspector>().Show();
        }

        private void OnGUI()
        {
            GUILayout.Label(currentNode.Guid);
        }
    }
}
