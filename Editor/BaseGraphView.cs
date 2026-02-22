using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        // Clipboard for copy/paste functionality
        private List<BaseGraphNode> m_clipboard = new List<BaseGraphNode>();
        private Vector2 m_lastMousePosition = Vector2.zero;

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

            // Register for keyboard events to frame nodes
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<ContextualMenuPopulateEvent>(BuildContextualMenu, TrickleDown.TrickleDown);
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            m_lastMousePosition = evt.mousePosition;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            // Press 'F' to frame all nodes
            if (evt.keyCode == KeyCode.F && !evt.ctrlKey && !evt.commandKey)
            {
                FrameAll();
                evt.StopPropagation();
            }

            // Ctrl+C / Cmd+C to copy selected nodes
            if ((evt.ctrlKey || evt.commandKey) && evt.keyCode == KeyCode.C)
            {
                CopySelectedNodes();
                evt.StopPropagation();
            }

            // Ctrl+V / Cmd+V to paste nodes
            if ((evt.ctrlKey || evt.commandKey) && evt.keyCode == KeyCode.V)
            {
                PasteNodes();
                evt.StopPropagation();
            }

            // Delete to remove selected nodes
            if (evt.keyCode == KeyCode.Delete)
            {
                DeleteSelectedNodes();
                evt.StopPropagation();
            }
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var selectedNodes = selection.OfType<GraphNodeEditor>().ToList();

            if (selectedNodes.Count > 0)
            {
                evt.menu.AppendAction("Copy", (a) => CopySelectedNodes(), DropdownMenuAction.AlwaysEnabled);
            }

            if (m_clipboard.Count > 0)
            {
                evt.menu.AppendAction("Paste", (a) => PasteNodes(), DropdownMenuAction.AlwaysEnabled);
            }

            if (selectedNodes.Count > 0)
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Delete", (a) => DeleteSelectedNodes(), DropdownMenuAction.AlwaysEnabled);
            }
        }

        private void CopySelectedNodes()
        {
            m_clipboard.Clear();
            var selectedNodes = selection.OfType<GraphNodeEditor>().ToList();

            if (selectedNodes.Count == 0)
                return;

            // Deep copy selected nodes
            foreach (var editorNode in selectedNodes)
            {
                BaseGraphNode copiedNode = DeepCopyNode(editorNode.Node);
                m_clipboard.Add(copiedNode);
            }

            Debug.Log($"Copied {m_clipboard.Count} node(s)");
        }

        private void PasteNodes()
        {
            if (m_clipboard.Count == 0)
                return;

            Undo.RecordObject(m_serializedObject.targetObject, "Pasted Nodes");

            ClearSelection();

            // Dictionary to map old GUIDs to new nodes for connection recreation
            Dictionary<string, BaseGraphNode> nodeMapping = new Dictionary<string, BaseGraphNode>();
            List<BaseGraphNode> newNodes = new List<BaseGraphNode>();

            // Create all new nodes first with offset
            Vector2 offset = new Vector2(50, 50);
            foreach (var clipboardNode in m_clipboard)
            {
                BaseGraphNode newNode = DeepCopyNode(clipboardNode);

                // Offset the position
                Rect newPosition = newNode.position;
                newPosition.position += offset;
                newNode.SetPosition(newPosition);

                newNodes.Add(newNode);
                nodeMapping[clipboardNode.Guid] = newNode;
            }

            // Add all nodes to the graph first
            foreach (var newNode in newNodes)
            {
                m_codeGraph.Nodes.Add(newNode);
            }

            // Update serialized object before creating editor nodes
            m_serializedObject.Update();
            m_serializedObject.ApplyModifiedProperties();

            // Now add nodes to the graph view
            foreach (var newNode in newNodes)
            {
                AddNodeToGraph(newNode);
                // Auto-select the pasted node
                AddToSelection(m_nodeDitionary[newNode.Guid]);
            }

            // Recreate connections between pasted nodes
            foreach (var connection in m_codeGraph.Connections.ToList())
            {
                bool inputInClipboard = nodeMapping.ContainsKey(connection.inputPort.nodeId);
                bool outputInClipboard = nodeMapping.ContainsKey(connection.outputPort.nodeId);

                // Only recreate connections where both nodes were copied
                if (inputInClipboard && outputInClipboard)
                {
                    var newInputNode = nodeMapping[connection.inputPort.nodeId];
                    var newOutputNode = nodeMapping[connection.outputPort.nodeId];

                    GraphConnection newConnection = new GraphConnection(
                        new GraphConnectionPort(newInputNode.Guid, connection.inputPort.portIndex),
                        new GraphConnectionPort(newOutputNode.Guid, connection.outputPort.portIndex)
                    );

                    m_codeGraph.Connections.Add(newConnection);
                }
            }

            m_serializedObject.ApplyModifiedProperties();
            Bind();

            Debug.Log($"Pasted {m_clipboard.Count} node(s)");
        }

        private void DeleteSelectedNodes()
        {
            var selectedNodes = selection.OfType<GraphNodeEditor>().ToList();
            if (selectedNodes.Count == 0) return;

            // Use Undo.RecordObject BEFORE making changes
            Undo.RecordObject(m_codeGraph, "Deleted Nodes");

            // Close inspector immediately to stop all property tracking
            NodeInspector.ClearInspector();

            foreach (var editorNode in selectedNodes)
            {
                // 1. Clean up connections first
                var connectionsToRemove = m_codeGraph.Connections
                    .Where(c => c.inputPort.nodeId == editorNode.Node.Guid || c.outputPort.nodeId == editorNode.Node.Guid)
                    .ToList();

                foreach (var connection in connectionsToRemove)
                {
                    // Find the edge associated with this connection and remove it from UI
                    var edge = m_connectionDictionary.FirstOrDefault(x => x.Value.Equals(connection)).Key;
                    if (edge != null) RemoveConnection(edge);
                    else m_codeGraph.Connections.Remove(connection);
                }

                // 2. Remove the node
                RemoveNode(editorNode);
            }

            // 3. Apply changes and clear selection
            m_serializedObject.ApplyModifiedProperties();
            ClearSelection();
        }

        /// <summary>
        /// Creates a deep copy of a node with all its properties
        /// </summary>
        private BaseGraphNode DeepCopyNode(BaseGraphNode original)
        {
            // Create a new instance of the same type using Activator
            BaseGraphNode copy = (BaseGraphNode)System.Activator.CreateInstance(original.GetType());

            // Copy all serializable fields from the original
            FieldInfo[] fields = original.GetType().GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                // Skip the GUID field - we'll generate a new one
                if (field.Name == "m_guid")
                    continue;

                try
                {
                    object value = field.GetValue(original);
                    field.SetValue(copy, value);
                }
                catch
                {
                    // Skip fields that can't be copied easily
                }
            }

            // Generate new GUID for the copy
            FieldInfo guidField = original.GetType().GetField("m_guid",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (guidField != null)
            {
                guidField.SetValue(copy, System.Guid.NewGuid().ToString());
            }

            return copy;
        }

        /// <summary>
        /// Frames all nodes on the screen using GraphView's built-in selection framing
        /// </summary>
        public void FrameAll()
        {
            if (m_graphNodes == null || m_graphNodes.Count == 0)
            {
                return;
            }

            // Select all nodes
            ClearSelection();
            foreach (var node in m_graphNodes)
            {
                AddToSelection(node);
            }

            // Frame the selection using built-in method
            FrameSelection();

            // Deselect all
            ClearSelection();
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
            // Handle Movement
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Moved Elements");
                foreach (GraphNodeEditor editorNode in graphViewChange.movedElements.OfType<GraphNodeEditor>())
                {
                    editorNode.SavePosition();
                }
            }

            // PLACE THE REMOVAL LOGIC HERE
            if (graphViewChange.elementsToRemove != null)
            {
                // Record the state of the ScriptableObject before we delete anything
                Undo.RecordObject(m_codeGraph, "Removed Item from Graph");

                // Use a list to avoid "collection modified" errors while iterating
                var elements = graphViewChange.elementsToRemove.ToList();

                foreach (var element in elements)
                {
                    if (element is GraphNodeEditor node)
                    {
                        RemoveNode(node);
                    }
                    else if (element is Edge edge)
                    {
                        RemoveConnection(edge);
                    }
                }

                // After cleaning up nodes and connections, apply changes
                m_serializedObject.ApplyModifiedProperties();
            }

            // Handle New Connections
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

            // Check if the output port already has a connection and disconnect it
            DisconnectExistingOutputConnection(outputNode.Node.Guid, outputIndex);

            GraphConnection connection = new GraphConnection(
                new GraphConnectionPort(inputNode.Node.Guid, inputIndex),
                new GraphConnectionPort(outputNode.Node.Guid, outputIndex)
            );

            // Add the new connection to the scriptable object
            m_codeGraph.Connections.Add(connection);

            // Update the serialized object to ensure changes are saved
            Undo.RecordObject(m_serializedObject.targetObject, "Added Connection");
            m_serializedObject.ApplyModifiedProperties();

            // Add to the dictionary for tracking
            m_connectionDictionary[edge] = connection;
        }

        /// <summary>
        /// Disconnects any existing connection on the output port
        /// </summary>
        private void DisconnectExistingOutputConnection(string outputNodeGuid, int outputPortIndex)
        {
            // Find all connections using this output port
            List<GraphConnection> connectionsToRemove = m_codeGraph.Connections
                .Where(c => c.outputPort.nodeId == outputNodeGuid && c.outputPort.portIndex == outputPortIndex)
                .ToList();

            foreach (var connection in connectionsToRemove)
            {
                m_codeGraph.Connections.Remove(connection);

                // Find and remove the corresponding edge from the view
                var edgesToRemove = m_connectionDictionary
                    .Where(kvp => kvp.Value.Equals(connection))
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var edge in edgesToRemove)
                {
                    RemoveElement(edge);
                    m_connectionDictionary.Remove(edge);
                }
            }

            m_serializedObject.ApplyModifiedProperties();
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
                //Debug.LogWarning("Failed to remove connection: Edge not found in dictionary.");
            }

            // Remove the edge from the graph view
            RemoveElement(e);
        }


        private void RemoveNode(GraphNodeEditor editorNode)
        {
            if (editorNode == null) return;

            // 1. Clear the Inspector if this node is the one being inspected
            // This prevents the 'ObjectDisposedException' from a deleted property
            NodeInspector.ClearInspector();

            // 2. Remove from data collections
            m_codeGraph.Nodes.Remove(editorNode.Node);
            m_nodeDitionary.Remove(editorNode.Node.Guid);
            m_graphNodes.Remove(editorNode);

            // 3. Physically remove the VisualElement from the GraphView
            RemoveElement(editorNode);

            // 4. Sync serialization
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
            node.SetTypeName(node.GetType().Name);

            GraphNodeEditor editorNode = new GraphNodeEditor(node, m_serializedObject);
            editorNode.SetPosition(node.position);
            m_graphNodes.Add(editorNode);
            m_nodeDitionary.Add(node.Guid, editorNode);

            AddElement(editorNode);
        }


        public void Bind()
        {
            m_serializedObject.Update();
            this.Bind(m_serializedObject);
        }
    }
}
