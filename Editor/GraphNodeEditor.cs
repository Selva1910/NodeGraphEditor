using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class GraphNodeEditor : Node
    {
        private BaseGraphNode m_graphNode;
        private Port m_outputPort;
        private List<Port> m_Ports;
        private SerializedProperty m_serializedProperty;

        public BaseGraphNode Node => m_graphNode;
        public List<Port> Ports => m_Ports;

        private SerializedObject m_serializedObject;

        public GraphNodeEditor(BaseGraphNode node, SerializedObject codeGraphObject)
        {
            this.AddToClassList("node-graph-node");

            extensionContainer.style.backgroundColor = new Color(.2f, .2f, .2f, .8f);

            m_graphNode = node;
            
            m_Ports = new List<Port>();

            
            m_serializedObject = codeGraphObject;

            Type typeInfo = node.GetType();
            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();

            if (info != null)
            {
                title = info.title;

                string[] depths = info.menuItem.Split('/');
                foreach (string depth in depths)
                {
                    this.AddToClassList(depth.ToLower().Replace(' ', '-'));
                }

                this.name = typeInfo.Name;

                if (info.hasFlowOutputs)
                {
                    DrawFlowOutputs(node);
                }

                bool drawInputs = info.hasFlowInputs;
                // If this is an event node and it's configured to allow external triggers,
                // draw the input port even though NodeInfo hasFlowInputs may be false.
                if (!drawInputs && node is BaseEventNode evtNode)
                {
                    drawInputs = evtNode.allowExternalTrigger;
                }

                if (drawInputs)
                {
                    DrawFlowInputs(node);
                }
            }
            else
            {
                Debug.LogWarning($"NodeInfoAttribute not found on {typeInfo.Name}");
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        private void DrawNodeProperties(Type typeInfo)
        {
            foreach (FieldInfo property in typeInfo.GetFields())
            {
                if (property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedProperty)
                {
                    string fieldName = string.Empty;
                    if (property.GetCustomAttribute<DisplayNameAttribute>() is DisplayNameAttribute displayName)
                    {
                        fieldName = displayName.Name;
                    }
                    else if (property.GetCustomAttribute<ExposedInNodeAttribute>() is ExposedInNodeAttribute exposedInNodeAttribute)
                    {
                        extensionContainer.Add( DrawInNodes(property.Name, exposedInNodeAttribute));
                    }
                    else if (property.GetCustomAttribute<ExposedOutNodeAttribute>() is ExposedOutNodeAttribute exposedOutNodeAttribute)
                    {
                        extensionContainer.Add( DrawOutNodes(property.Name, exposedOutNodeAttribute));
                    }
                    else
                    {
                        fieldName = property.Name;
                    }
                    VisualElement field = DrawProperty(property.Name, fieldName);
                    // Register value change callbacks here if needed
                }
            }
        }

        private VisualElement DrawInNodes(string propertyName, ExposedInNodeAttribute exposedInNodeAttribute)
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = propertyName;
            inputPort.portColor = Color.red;
            return inputPort;
        }
        private VisualElement DrawOutNodes(string propertyName, ExposedOutNodeAttribute exposedOutNodeAttribute)
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            inputPort.portName = propertyName;
            inputPort.portColor = Color.red;
            return inputPort;
        }

        public override void OnSelected()
        {
            //Debug.Log(this.m_graphNode);
            NodeInspector.ShowInspector(m_graphNode);
        }
        private VisualElement DrawProperty(string propertyName, string displayName)
        {
            if (m_serializedProperty == null)
            {
                FetchBaseSerializeProperty();
            }

            if (m_serializedProperty == null)
            {
                Debug.LogError($"Serialized property could not be found or bound for {propertyName}");
                return null;
            }

            SerializedProperty prop = m_serializedProperty.FindPropertyRelative(propertyName);

            if (prop == null)
            {
                Debug.LogWarning($"Property {propertyName} not found in serialized object.");
                return null;
            }

            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                ObjectField objectField = new ObjectField(displayName)
                {
                    objectType = typeof(SceneObject),
                    allowSceneObjects = true,
                    bindingPath = prop.propertyPath
                };

                objectField.BindProperty(prop); // Properly bind it
                extensionContainer.Add(objectField);
                return objectField;
            }
            else
            {
                PropertyField field = new PropertyField(prop)
                {
                    bindingPath = prop.propertyPath
                };
                field.label = string.IsNullOrWhiteSpace(displayName) ? propertyName : displayName;
                extensionContainer.Add(field);

                // If this property controls number of outputs for ParallelNode, refresh ports on change
                if (propertyName == "numberOfOutputs")
                {
                    field.RegisterValueChangeCallback((evt) =>
                    {
                        // Apply changes so the underlying object has the new value
                        try
                        {
                            m_serializedObject.ApplyModifiedProperties();
                        }
                        catch { }

                        RefreshFlowPorts();
                    });
                }

                // If this property toggles external trigger for event nodes, refresh ports on change
                if (propertyName == "allowExternalTrigger")
                {
                    field.RegisterValueChangeCallback((evt) =>
                    {
                        try { m_serializedObject.ApplyModifiedProperties(); } catch { }
                        RefreshFlowPorts();
                    });
                }

                return field;
            }
        }

        private void RefreshFlowPorts()
        {
            // Remove existing input/output ports from containers and m_Ports
            var portsToRemove = new List<Port>();
            foreach (var p in m_Ports)
            {
                if (p.direction == Direction.Output || p.direction == Direction.Input)
                {
                    portsToRemove.Add(p);
                }
            }

            foreach (var p in portsToRemove)
            {
                // remove from input/output container if present
                try { inputContainer.Remove(p); } catch { }
                try { outputContainer.Remove(p); } catch { }
                m_Ports.Remove(p);
            }

            // Recreate flow ports based on current node metadata and serialized values
            Type typeInfo = m_graphNode.GetType();
            var info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();

            bool drawOutputs = info != null && info.hasFlowOutputs;
            bool drawInputs = info != null && info.hasFlowInputs;

            if (!drawInputs && m_graphNode is BaseEventNode evtNode)
            {
                drawInputs = evtNode.allowExternalTrigger;
            }

            // Preserve original ordering: draw outputs first, then inputs
            if (drawOutputs)
            {
                DrawFlowOutputs(m_graphNode);
            }

            if (drawInputs)
            {
                DrawFlowInputs(m_graphNode);
            }

            // Refresh visuals
            RefreshExpandedState();
            RefreshPorts();
            this.MarkDirtyRepaint();

            var gv = this.GetFirstAncestorOfType<UnityEditor.Experimental.GraphView.GraphView>();
            if (gv != null) gv.MarkDirtyRepaint();
        }

        private void FetchBaseSerializeProperty()
        {
            SerializedProperty nodes = m_serializedObject.FindProperty("m_nodes");

            if (nodes.isArray)
            {
                int size = nodes.arraySize;
                for (int i = 0; i < size; i++)
                {
                    var element = nodes.GetArrayElementAtIndex(i);
                    var elementId = element.FindPropertyRelative("m_guid");
                    if (elementId != null && elementId.stringValue == m_graphNode.Guid)
                    {
                        m_serializedProperty = element;
                        break; // Exit loop once the matching property is found
                    }
                }
            }
        }

        private void DrawFlowInputs(BaseGraphNode courseNode)
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            inputPort.portName = "In";
            inputPort.tooltip = "The Flow Input";
            m_Ports.Add(inputPort);
            inputContainer.Add(inputPort);
        }

        private void DrawFlowOutputs(BaseGraphNode courseNode)
        {
            // Special handling for ParallelNode - create multiple output ports
            if (courseNode is ParallelNode parallelNode)
            {
                int outputs = GetParallelOutputCount(parallelNode);
                for (int i = 0; i < outputs; i++)
                {
                    Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
                    outputPort.portName = $"Out {i}";
                    outputPort.tooltip = $"Output {i}";
                    m_Ports.Add(outputPort);
                    outputContainer.Add(outputPort);
                }
            }
            else
            {
                // Standard single output port for regular nodes
                m_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(PortTypes.FlowPort));
                m_outputPort.portName = "Out";
                m_outputPort.tooltip = "The Flow Output";
                m_Ports.Add(m_outputPort);
                outputContainer.Add(m_outputPort);
            }
        }

        public void AddOutputPort()
        {
            m_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(PortTypes.FlowPort));
            m_outputPort.portName = "Out";
            m_outputPort.tooltip = "The Flow Output";
            m_Ports.Add(m_outputPort);
            outputContainer.Add(m_outputPort);
        }
        
        private int GetParallelOutputCount(ParallelNode parallelNode)
        {
            if (m_serializedProperty != null)
            {
                try
                {
                    var prop = m_serializedProperty.FindPropertyRelative("numberOfOutputs");
                    if (prop != null && prop.propertyType == SerializedPropertyType.Integer)
                    {
                        return Mathf.Max(0, prop.intValue);
                    }
                }
                catch { }
            }

            return Mathf.Max(0, parallelNode.numberOfOutputs);
        }
        public void SavePosition()
        {
            m_graphNode.SetPosition(GetPosition());
        }
    }
}
