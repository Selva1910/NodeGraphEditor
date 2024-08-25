using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public class BaseGraphNode
    {
        [SerializeField] private string m_guid;
        [SerializeField] private Rect m_position;

        public string typeName;

        public string guid => m_guid;
        public Rect position => m_position;

        public BaseGraphNode()
        {
            NewGUID();

        }

        private void NewGUID()
        {
            m_guid = Guid.NewGuid().ToString();
        }

        public void SetPosition(Rect position)
        {
            m_position = position;
        }

        public virtual string OnProcess(GraphAssetSO currentGraph)
        {
            if (currentGraph == null)
            {
                Debug.LogWarning("Current graph is null");
                return string.Empty;
            }

            BaseGraphNode nextNodeInFlow = currentGraph.GetNodeFromOutput(m_guid, 0);
            if (nextNodeInFlow != null)
            {
                return nextNodeInFlow.guid;
            }
            return string.Empty;
        }
    }
}
