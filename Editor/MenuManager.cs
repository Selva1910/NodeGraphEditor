using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class MenuManager
    {
        
        private const string menuName = "NodeGraph/New Sequence"; 
        
        [MenuItem(menuName)]
        public static void NewSequence()
        {
            EditorWindow.GetWindow<SceneCreation>().ShowModal();
        }
    }
}
