using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class MenuManager
    {
        
        private const string mainMenu = "Node Graph"; 
        
        private const string newSequence = "New Sequence";
        private const string about = "About"; 
        
        //[MenuItem(mainMenu + "/" + newSequence)]
        public static void NewSequence()
        {
            EditorWindow.GetWindow<SceneCreation>().ShowModal();
        }

        [MenuItem(mainMenu + "/" + about)]
        public static void ShowAbout()
        {
            NodeGraphEditorAbout.ShowWindow();
        }

    }
}
