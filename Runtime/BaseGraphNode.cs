using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public abstract class BaseGraphNode : INodeLifecycle, INode
    {
        [SerializeField] private string m_guid;
        [SerializeField] private Rect m_position;

        private string typeName;

        public void SetTypeName(string name)
        {
            typeName = name;
        }

        public string Guid => m_guid;
        public Rect position => m_position;

        public bool IsCompleted { get; set; }
        public LifeStage Stage { get; set; }

        public BaseGraphNode()
        {
            NewGUID();

        }

        private void NewGUID()
        {
            m_guid = System.Guid.NewGuid().ToString();
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
                return nextNodeInFlow.Guid;
            }
            return string.Empty;
        }

        public virtual void AwakeNode()
        {
            //Debug.Log($"Awake on Node {this.GetType()}");
        }

        public virtual void StartNode()
        {
           // Debug.Log($"Start on Node {this.GetType()}");
        }

        public virtual void UpdateNode()
        {
            //Debug.Log($"Update on Node {this.GetType()}");

        }

        public virtual void ExitNode()
        {
            //Debug.Log($"Exit on Node {this.GetType()}");

        }
    }
}
public enum LifeStage
{
    Activating,
    Active,
    Paused,
    DeActivating,
    InActive
}