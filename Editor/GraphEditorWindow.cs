using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        public static void Open(GraphAssetSO target)
        {
            GraphEditorWindow[] windows = Resources.FindObjectsOfTypeAll<GraphEditorWindow>();
            foreach (GraphEditorWindow win in windows)
            {
                if (win.currentGraph == target)
                {
                    win.Focus();
                    return;
                }
            }
            GraphEditorWindow window = CreateWindow<GraphEditorWindow>(typeof(GraphEditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent($"{target.name}");
            window.Load(target);
        }


        [SerializeField]
        private GraphAssetSO m_currentGraph;

        [SerializeField]
        private SerializedObject m_serializedObject;

        [SerializeField]
        private BaseGraphView m_currenView;

        public GraphAssetSO currentGraph => m_currentGraph;

        private void OnEnable()
        {
            if (m_currentGraph != null)
            {
                DrawGraph();
            }
        }
        private void OnGUI()
        {
            if (m_currentGraph != null)
            {
                if (EditorUtility.IsDirty(m_currentGraph))
                {
                    this.hasUnsavedChanges = true;
                }
                else
                {
                    this.hasUnsavedChanges = false;
                }
            }
        }
        private void Load(GraphAssetSO target)
        {
            m_currentGraph = target;
            DrawGraph();
        }

        private void DrawGraph()
        {
            m_serializedObject = new SerializedObject(m_currentGraph);
            m_currenView = new BaseGraphView(m_serializedObject, this);
            m_currenView.graphViewChanged += OnChange;
            rootVisualElement.Add(m_currenView);
        }

        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            EditorUtility.SetDirty(m_currentGraph);
            return graphViewChange;
        }
    }
}
