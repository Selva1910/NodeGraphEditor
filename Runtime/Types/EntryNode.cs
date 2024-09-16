using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Start", "Process/Start", false, true)]
    public class EntryNode : BaseGraphNode
    {

        public override string OnProcess(GraphAssetSO currentGraph)
        {
            return base.OnProcess(currentGraph);
        }

        public override void StartNode()
        {
            base.StartNode();
            Debug.Log("Hello World, Start!");
            IsCompleted = true;
        }

    }
}
