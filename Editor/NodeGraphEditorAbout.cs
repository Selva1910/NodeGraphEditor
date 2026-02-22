using UnityEngine;
using UnityEditor;

namespace NodeGraph.Editor
{

public class NodeGraphEditorAbout : EditorWindow {
   
    public static void ShowWindow() {
        var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(NodeGraphEditorAbout).Assembly);

        if (packageInfo != null) 
        {
            EditorUtility.DisplayDialog($"About {packageInfo.displayName}", 
                $"Developed by {packageInfo.author.name}\n" +
                $"Version: {packageInfo.version}\n\n" +
                "This software is licensed under AGPLv3.", "OK");
        } 
        else 
        {
            EditorUtility.DisplayDialog("About Node Graph Editor", "Metadata not found. Ensure the script is inside a UPM package folder with an .asmdef file.", "OK");
        }
    }
}
}
