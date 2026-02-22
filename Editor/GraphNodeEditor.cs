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

        public BaseGraphNode Node => m_graphNode;
        public List<Port> Ports => m_Ports;

        private SerializedObject m_serializedObject;
        public SerializedObject SerializedObject => m_serializedObject;

        // FIXED: The only definition of SerializedProperty. Always fetches fresh by GUID.
        public SerializedProperty SerializedProperty
        {
            get
            {
                SerializedProperty nodes = m_serializedObject.FindProperty("m_nodes");
                if (nodes != null && nodes.isArray)
                {
                    for (int i = 0; i < nodes.arraySize; i++)
                    {
                        var element = nodes.GetArrayElementAtIndex(i);
                        var elementId = element.FindPropertyRelative("m_guid");
                        if (elementId != null && elementId.stringValue == m_graphNode.Guid)
                        {
                            return element;
                        }
                    }
                }
                return null;
            }
        }

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
                DrawNodeProperties(typeInfo);

                if (info.hasFlowOutputs) DrawFlowOutputs(node);

                bool drawInputs = info.hasFlowInputs;
                if (!drawInputs && node is BaseEventNode evtNode)
                {
                    drawInputs = evtNode.allowExternalTrigger;
                }

                if (drawInputs) DrawFlowInputs(node);
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        private void DrawNodeProperties(Type typeInfo)
        {
            foreach (FieldInfo property in typeInfo.GetFields())
            {
                if (property.GetCustomAttribute<ExposedPropertyAttribute>() != null)
                {
                    string fieldName = property.Name;
                    if (property.GetCustomAttribute<DisplayNameAttribute>() is DisplayNameAttribute displayName)
                    {
                        fieldName = displayName.Name;
                    }

                    // Handle Special Node Layouts
                    if (property.GetCustomAttribute<ExposedInNodeAttribute>() is ExposedInNodeAttribute inAttr)
                        extensionContainer.Add(DrawInNodes(property.Name, inAttr));
                    else if (property.GetCustomAttribute<ExposedOutNodeAttribute>() is ExposedOutNodeAttribute outAttr)
                        extensionContainer.Add(DrawOutNodes(property.Name, outAttr));
                    else
                        DrawProperty(property.Name, fieldName);
                }
            }
        }

        // FIXED: The only definition of DrawProperty. Uses the dynamic SerializedProperty.
        private VisualElement DrawProperty(string propertyName, string displayName)
        {
            // 1. Get the current property
            SerializedProperty nodeProp = SerializedProperty;
            if (nodeProp == null) return null;

            SerializedProperty prop = nodeProp.FindPropertyRelative(propertyName);
            if (prop == null) return null;

            VisualElement fieldElement;

            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                var objectField = new ObjectField(displayName)
                {
                    objectType = typeof(UnityEngine.Object),
                    allowSceneObjects = true
                };

                // Use a TrackPropertyValue instead of internal bindingPath to be safer
                objectField.BindProperty(prop);
                fieldElement = objectField;
            }
            else
            {
                // 2. Do NOT set bindingPath in the constructor. 
                // This is often what causes the internal UIElements crash.
                var field = new PropertyField(prop)
                {
                    label = string.IsNullOrWhiteSpace(displayName) ? propertyName : displayName
                };

                field.RegisterValueChangeCallback((evt) =>
                {
                    // Safety check before applying
                    if (m_serializedObject != null && m_serializedObject.targetObject != null)
                    {
                        m_serializedObject.ApplyModifiedProperties();
                        if (propertyName == "numberOfOutputs" || propertyName == "allowExternalTrigger")
                        {
                            RefreshFlowPorts();
                        }
                    }
                });
                fieldElement = field;
            }

            return fieldElement;
        }

        public override void OnSelected()
        {
            // Only show inspector if the property is actually valid
            if (SerializedProperty != null)
            {
                NodeInspector.ShowInspector(this);
            }
        }

        private void RefreshFlowPorts()
        {
            var portsToRemove = new List<Port>();
            foreach (var p in m_Ports)
            {
                // Identify flow ports to clear them out
                if (p.direction == Direction.Output || p.direction == Direction.Input)
                    portsToRemove.Add(p);
            }

            foreach (var p in portsToRemove)
            {
                // Correct way: Remove the Port from the Node's containers
                if (p.direction == Direction.Input)
                    inputContainer.Remove(p);
                else
                    outputContainer.Remove(p);

                m_Ports.Remove(p);
            }

            Type typeInfo = m_graphNode.GetType();
            var info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();

            if (info != null && info.hasFlowOutputs) DrawFlowOutputs(m_graphNode);

            bool drawInputs = info != null && info.hasFlowInputs;
            if (!drawInputs && m_graphNode is BaseEventNode evtNode) drawInputs = evtNode.allowExternalTrigger;

            if (drawInputs) DrawFlowInputs(m_graphNode);

            RefreshExpandedState();
            RefreshPorts();
        }

        private void DrawFlowInputs(BaseGraphNode courseNode)
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(float));
            inputPort.portName = "In";
            m_Ports.Add(inputPort);
            inputContainer.Add(inputPort);
        }

        private void DrawFlowOutputs(BaseGraphNode courseNode)
        {
            if (courseNode is ParallelNode parallelNode)
            {
                int outputs = GetParallelOutputCount(parallelNode);
                for (int i = 0; i < outputs; i++)
                {
                    Port p = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
                    p.portName = $"Out {i}";
                    m_Ports.Add(p);
                    outputContainer.Add(p);
                }
            }
            else
            {
                m_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
                m_outputPort.portName = "Out";
                m_Ports.Add(m_outputPort);
                outputContainer.Add(m_outputPort);
            }
        }

        private int GetParallelOutputCount(ParallelNode parallelNode)
        {
            SerializedProperty nodeProp = SerializedProperty;
            if (nodeProp != null)
            {
                var prop = nodeProp.FindPropertyRelative("numberOfOutputs");
                if (prop != null) return Mathf.Max(0, prop.intValue);
            }
            return Mathf.Max(0, parallelNode.numberOfOutputs);
        }

        private VisualElement DrawInNodes(string propertyName, ExposedInNodeAttribute attr)
        {
            Port p = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            p.portName = propertyName;
            p.portColor = Color.red;
            m_Ports.Add(p);
            return p;
        }

        private VisualElement DrawOutNodes(string propertyName, ExposedOutNodeAttribute attr)
        {
            Port p = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float));
            p.portName = propertyName;
            p.portColor = Color.red;
            m_Ports.Add(p);
            return p;
        }

        public void SavePosition() => m_graphNode.SetPosition(GetPosition());
    }
}