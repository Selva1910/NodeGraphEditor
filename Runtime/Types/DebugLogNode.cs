using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Log", "Debug/Log Message", true, true)]
    public class DebugLogNode : BaseGraphNode
    {
        [ExposedProperty]
        public string logMessage;

        public override string OnProcess(GraphAssetSO currentGraph)
        {
            Debug.Log(logMessage);

            return base.OnProcess(currentGraph);
        }
    }
}
