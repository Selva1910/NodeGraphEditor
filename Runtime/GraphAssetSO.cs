using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeGraph
{
    [CreateAssetMenu(fileName = "NewCode", menuName = "CodeGraph/Asset")]
    public class GraphAssetSO : ScriptableObject
    {
        [SerializeReference]
        private List<BaseGraphNode> m_nodes;
        [SerializeField]
        private List<GraphConnection> m_connections;

        public List<BaseGraphNode> Nodes => m_nodes;

        public List<GraphConnection> Connections => m_connections;

        public Dictionary<string, BaseGraphNode> m_NodeDictionary;

        public GameObject gameObject;

        public GraphAssetSO()
        {
            m_nodes = new List<BaseGraphNode>();
            m_connections = new List<GraphConnection>();
        }

        public BaseGraphNode GetStartNode()
        {
            if (Nodes == null || Nodes.Count == 0)
            {
                Debug.LogError("No nodes available!");
                return null;
            }

            StartNode[] startNodes = Nodes.OfType<StartNode>().ToArray();
            if (startNodes.Length == 0)
            {
                Debug.LogError("No Start Node!");
                return null;
            }
            return startNodes[0];
        }


        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            m_NodeDictionary = new Dictionary<string, BaseGraphNode>();
            foreach (BaseGraphNode node in Nodes)
            {
                m_NodeDictionary.Add(node.guid, node);
            }
        }

        public BaseGraphNode GetNode(string nextNodeId)
        {
            if (m_NodeDictionary.TryGetValue(nextNodeId, out BaseGraphNode node))
            {
                return node;
            }
            return null;
        }

        public BaseGraphNode GetNodeFromOutput(string outputNodeId, int index)
        {
            if (Connections == null || Connections.Count == 0)
            {
                Debug.LogError("No connections available!");
                return null;
            }

            foreach (GraphConnection connection in Connections)
            {
                if (connection.outputPort.nodeId == outputNodeId && connection.outputPort.portIndex == index)
                {
                    if (m_NodeDictionary.TryGetValue(connection.inputPort.nodeId, out BaseGraphNode inputNode))
                    {
                        return inputNode;
                    }
                    else
                    {
                        Debug.LogError("Node ID not found in dictionary!");
                    }
                }
            }
            return null;
        }

    }
}
