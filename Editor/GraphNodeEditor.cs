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

                if (info.hasFlowInputs)
                {
                    DrawFlowInputs(node);
                }

                foreach (FieldInfo property in typeInfo.GetFields())
                {
                    if (property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedProperty)
                    {
                        string fieldName = string.Empty;
                        if (property.GetCustomAttribute<DisplayNameAttribute>() is DisplayNameAttribute displayName)
                        {
                            fieldName = displayName.Name;
                        }
                        else
                        {
                            fieldName = property.Name;
                        }
                        PropertyField field = DrawProperty(property.Name, fieldName);
                        // Register value change callbacks here if needed
                    }
                }
            }
            else
            {
                Debug.LogWarning($"NodeInfoAttribute not found on {typeInfo.Name}");
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        private PropertyField DrawProperty(string propertyName, string displayName)
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

            PropertyField field = new PropertyField(prop)
            {
                bindingPath = prop.propertyPath
            };
            field.label = string.IsNullOrWhiteSpace(displayName) ? propertyName : displayName;
            extensionContainer.Add(field);

            return field;
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
                    if (elementId != null && elementId.stringValue == m_graphNode.guid)
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
            m_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(PortTypes.FlowPort));
            m_outputPort.portName = "Out";
            m_outputPort.tooltip = "The Flow Output";
            m_Ports.Add(m_outputPort);
            outputContainer.Add(m_outputPort);
        }

        public void SavePosition()
        {
            m_graphNode.SetPosition(GetPosition());
        }
    }
}
