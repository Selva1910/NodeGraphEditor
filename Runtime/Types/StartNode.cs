using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Start", "Process/Start", false, true)]
    public class StartNode : BaseGraphNode
    {
        public override string OnProcess(GraphAssetSO currentGraph)
        {
            Debug.Log("Hello World, Start!");
            return base.OnProcess(currentGraph);
        }

    }
}
