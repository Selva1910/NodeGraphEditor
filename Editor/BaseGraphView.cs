using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class BaseGraphView : GraphView
    {

        private GraphAssetSO m_codeGraph;
        private SerializedObject m_serializedObject;
        private GraphEditorWindow m_window;

        public GraphEditorWindow window => m_window;

        public List<GraphNodeEditor> m_graphNodes;
        public Dictionary<string, GraphNodeEditor> m_nodeDitionary;
        public Dictionary<Edge, GraphConnection> m_connectionDictionary;

        private GraphWindowSearchProvider m_searchProvider;

        public BaseGraphView(SerializedObject serializedObject, GraphEditorWindow window)
        {
            m_serializedObject = serializedObject;
            m_codeGraph = (GraphAssetSO)serializedObject.targetObject;
            m_window = window;

            m_graphNodes = new List<GraphNodeEditor>();
            m_nodeDitionary = new Dictionary<string, GraphNodeEditor>();
            m_connectionDictionary = new Dictionary<Edge, GraphConnection>();

            m_searchProvider = ScriptableObject.CreateInstance<GraphWindowSearchProvider>();
            m_searchProvider.graph = this;

            this.nodeCreationRequest = ShowSearchWindow;

            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.cjhawk.graphnodeeditor/Editor/USS/GraphEditor.uss");
            styleSheets.Add(style);

            GridBackground background = new GridBackground();
            background.name = "Grid";
            Insert(0, background);


            // Setup zoom and pan
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Setup dragging and selection
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            DrawNodes();
            DrawConnections();

            graphViewChanged += OnGraphViewChangedEvent;
        }


        private void DrawConnections()
        {
            if (m_codeGraph.Connections == null)
                return;
            foreach (var connection in m_codeGraph.Connections)
            {
                DrawConnections(connection);
            }
        }

        private void DrawConnections(GraphConnection connection)
        {
            GraphNodeEditor inputNode = GetNode(connection.inputPort.nodeId);
            GraphNodeEditor outputNode = GetNode(connection.outputPort.nodeId);

            if (inputNode == null || outputNode == null)
            {
                Debug.LogWarning("Input or output node not found for connection.");
                return;
            }

            if (inputNode.Ports.Count <= connection.inputPort.portIndex || outputNode.Ports.Count <= connection.outputPort.portIndex)
            {
                Debug.LogWarning("Port index out of range.");
                return;
            }

            Port inputPort = inputNode.Ports[connection.inputPort.portIndex];
            Port outPort = outputNode.Ports[connection.outputPort.portIndex];

            Edge edge = inputPort.ConnectTo(outPort);
            AddElement(edge);

            m_connectionDictionary.Add(edge, connection);
        }


        private GraphNodeEditor GetNode(string nodeId)
        {
            GraphNodeEditor node = null;
            m_nodeDitionary.TryGetValue(nodeId, out node);
            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new List<Port>();
            List<Port> ports = new List<Port>();

            foreach (var node in m_graphNodes)
            {
                allPorts.AddRange(node.Ports);
            }

            foreach (Port port in allPorts)
            {
                if (port == startPort) { continue; }
                if (port.node == startPort.node) { continue; }
                if (port.direction == startPort.direction) { continue; }
                if (port.portType == startPort.portType)
                {
                    ports.Add(port);
                }
            }

            return ports;
        }
        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Moved Elements");
                foreach (GraphNodeEditor editorNode in graphViewChange.movedElements.OfType<GraphNodeEditor>())
                {
                    editorNode.SavePosition();
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Removed Item from Graph");
                List<GraphNodeEditor> nodes = graphViewChange.elementsToRemove.OfType<GraphNodeEditor>().ToList();
                if (nodes.Count > 0)
                {
                    foreach (var node in nodes)
                    {
                        RemoveNode(node);
                    }
                }

                foreach (Edge e in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    RemoveConnection(e);
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Added Connection");
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    CreateEdge(edge);
                }
            }

            return graphViewChange;
        }

        private void CreateEdge(Edge edge)
        {
            GraphNodeEditor inputNode = (GraphNodeEditor)edge.input.node;
            int inputIndex = inputNode.Ports.IndexOf(edge.input);

            GraphNodeEditor outputNode = (GraphNodeEditor)edge.output.node;
            int outputIndex = outputNode.Ports.IndexOf(edge.output);

            GraphConnection connection = new GraphConnection(
                new GraphConnectionPort(inputNode.Node.guid, inputIndex),
                new GraphConnectionPort(outputNode.Node.guid, outputIndex)
            );

            // Add the new connection to the scriptable object
            m_codeGraph.Connections.Add(connection);

            // Update the serialized object to ensure changes are saved
            Undo.RecordObject(m_serializedObject.targetObject, "Added Connection");
            m_serializedObject.ApplyModifiedProperties();

            // Add to the dictionary for tracking
            m_connectionDictionary[edge] = connection;
        }

        private void RemoveConnection(Edge e)
        {
            if (m_connectionDictionary.TryGetValue(e, out GraphConnection connection))
            {
                // Remove the connection from the scriptable object
                m_codeGraph.Connections.Remove(connection);

                // Remove from the dictionary
                m_connectionDictionary.Remove(e);

                // Update the serialized object to ensure changes are saved
                Undo.RecordObject(m_serializedObject.targetObject, "Removed Connection");
                m_serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning("Failed to remove connection: Edge not found in dictionary.");
            }

            // Remove the edge from the graph view
            RemoveElement(e);
        }


        private void RemoveNode(GraphNodeEditor editorNode)
        {
            m_codeGraph.Nodes.Remove(editorNode.Node);
            m_nodeDitionary.Remove(editorNode.Node.guid);
            m_graphNodes.Remove(editorNode);
            m_serializedObject.Update();

        }

        private void DrawNodes()
        {
            foreach (var node in m_codeGraph.Nodes)
            {
                AddNodeToGraph(node);
                Bind();
            }
        }

        private void ShowSearchWindow(NodeCreationContext context)
        {
            m_searchProvider.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_searchProvider);
        }

        public void Add(BaseGraphNode node)
        {
            Undo.RecordObject(m_serializedObject.targetObject, "Added Node");

            m_codeGraph.Nodes.Add(node);
            m_serializedObject.Update();

            AddNodeToGraph(node);
            Bind();
        }

        private void AddNodeToGraph(BaseGraphNode node)
        {
            node.typeName = node.GetType().AssemblyQualifiedName;

            GraphNodeEditor editorNode = new GraphNodeEditor(node, m_serializedObject);
            editorNode.SetPosition(node.position);
            m_graphNodes.Add(editorNode);
            m_nodeDitionary.Add(node.guid, editorNode);

            AddElement(editorNode);
        }


        public void Bind()
        {
            m_serializedObject.Update();
            this.Bind(m_serializedObject);
        }
    }
}
