using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Log", "Debug/Log Message", true, true)]
    public class DebugLogNode : BaseGraphNode
    {
        [TextArea]
        [ExposedProperty]
        [DisplayName("Message")]
        public string logMessage;


        public override void StartNode()
        {
            base.StartNode();
            Debug.Log(logMessage);
            IsCompleted = true;
        }
    }
}
