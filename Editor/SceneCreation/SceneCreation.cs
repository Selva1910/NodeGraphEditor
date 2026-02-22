using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class SceneCreation : EditorWindow
    {
        
        public static void ShowWindow()
        {
            SceneCreation wnd = GetWindow<SceneCreation>();
            wnd.titleContent = new GUIContent("Scene Creation");
        }

        public void OnEnable()
        {
            // Load and clone UXML layout
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.cjhawk.graphnodeeditor/Editor/SceneCreation/SceneCreation.uxml");
            VisualElement root = visualTree.CloneTree();
            rootVisualElement.Add(root);

            // Apply USS styling
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.cjhawk.graphnodeeditor/Editor/SceneCreation/SceneCreation.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            // Find and bind button event
            var button = root.Q<Button>("create-scene-button");
            if (button != null)
            {
                button.clicked += () =>
                {
                    this.Close();
                    EditorApplication.delayCall += () =>
                    {
                        Debug.Log("Scene creation button clicked!");
                    };
                };
            }
        }
    }
}