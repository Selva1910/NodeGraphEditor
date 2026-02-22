using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Parallel", "Flow/Parallel", true, true)]
    public class ParallelNode : BaseGraphNode
    {
        [ExposedProperty]
        [DisplayName("Number of Outputs")]
        public int numberOfOutputs = 2;

        public override string OnProcess(GraphAssetSO currentGraph)
        {
            // Return the first output connection
            BaseGraphNode nextNodeInFlow = currentGraph.GetNodeFromOutput(Guid, 0);
            if (nextNodeInFlow != null)
            {
                return nextNodeInFlow.Guid;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets all the next nodes from all output ports
        /// </summary>
        public List<BaseGraphNode> GetAllOutputNodes(GraphAssetSO currentGraph)
        {
            List<BaseGraphNode> outputNodes = new List<BaseGraphNode>();

            for (int i = 0; i < numberOfOutputs; i++)
            {
                BaseGraphNode nextNode = currentGraph.GetNodeFromOutput(Guid, i);
                if (nextNode != null)
                {
                    outputNodes.Add(nextNode);
                }
            }

            return outputNodes;
        }

        public override void StartNode()
        {
            base.StartNode();
            // Parallel node completes immediately as it just splits execution
            IsCompleted = true;
        }
    }
}
