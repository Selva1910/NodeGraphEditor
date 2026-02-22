using UnityEngine;
using UnityEditor;

namespace NodeGraph.Editor
{

public class NodeGraphEditorAbout : EditorWindow {
   
    public static void ShowWindow() {
        EditorUtility.DisplayDialog("About Node Graph Editor", 
            "Developed by Selvaraj Balakrishnan\n" +
            "Version: 1.0.5\n\n" +
            "This software is licensed under AGPLv3. " +
            "Modifications must remain open source and credit the original author.", "OK");
    }
}
}
