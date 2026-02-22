using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeGraph
{
    public class EventGraphViewController : MonoBehaviour
    {
        [SerializeField] GraphAssetSO m_graphAsset;

        private GraphAssetSO graphInstance;

        private void OnEnable()
        {
            if (m_graphAsset != null)
            {
                graphInstance = Instantiate(m_graphAsset);
            }
        }

        private void Start()
        {
            StartEventProcess(graphInstance);
        }


        public void StartEventProcess(GraphAssetSO instance)
        {
            ExecuteAsset(instance);
        }

        private void ExecuteAsset(GraphAssetSO codeGraphAsset)
        {
            graphInstance.Init(gameObject);

            BaseEventNode[] eventNodes = graphInstance.GetAllGraphEventNodes();

            if (eventNodes != null && eventNodes.Length > 0)
            {
                // For event-driven graphs, start processing all event nodes directly
                // No need for an EntryNode - event nodes ARE the entry points
                HashSet<string> visitedNodes = new HashSet<string>();
                
                foreach (BaseEventNode eventNode in eventNodes)
                {
                    ProcessEventNodes(eventNode, visitedNodes, 0, codeGraphAsset);
                }
            }
            else
            {
                Debug.LogError("Event node not found in the graph. Please add at least one event node (On Enable, On Key Press, etc.)");
            }
        }

        private void ProcessEventNodes(BaseGraphNode currentNode, HashSet<string> visitedNodes, int depth, GraphAssetSO currentGraph)
        {
            if (currentNode == null || depth > 1000) // Safety limit to prevent infinite loop
            {
                Debug.LogError("Reached maximum recursion depth or current node is null. Stopping execution.");
                return;
            }

            if (visitedNodes.Contains(currentNode.Guid))
            {
                Debug.LogError("Circular reference detected at node: " + currentNode.Guid);
                return;
            }

            visitedNodes.Add(currentNode.Guid);
            currentNode.Stage = LifeStage.Activating;
            currentNode.AwakeNode();

            currentNode.StartNode();
            
            StartCoroutine(UpdateCurrentNodeTillComplete(currentNode, () =>
            {
                currentNode.ExitNode();
                
                // Get the next connected node (use output->input mapping)
                BaseGraphNode nextNodeConnected = currentGraph.GetNodeFromOutput(currentNode.Guid, 0);
                
                if (nextNodeConnected != null)
                {
                    if (nextNodeConnected is BaseEventNode)
                    {
                        ProcessEventNodes(nextNodeConnected, visitedNodes, depth + 1, currentGraph);
                    }
                    else
                    {
                        ProcessAndMoveNextNode(nextNodeConnected, visitedNodes, depth + 1);
                    }
                }
            }));
        }
        private void ProcessAndMoveNextNode(BaseGraphNode currentNode, HashSet<string> visitedNodes, int depth)
        {
            if (currentNode == null || depth > 1000) // Safety limit to prevent infinite loop
            {
                Debug.LogError("Reached maximum recursion depth or current node is null. Stopping execution.");
                return;
            }

            if (visitedNodes.Contains(currentNode.Guid))
            {
                Debug.LogError("Circular reference detected at node: " + currentNode.Guid);
                return;
            }

            visitedNodes.Add(currentNode.Guid);

            currentNode.Stage = LifeStage.Activating;
            currentNode.AwakeNode();

            currentNode.StartNode();

            StartCoroutine(UpdateCurrentNodeTillComplete(currentNode, () =>
            {
                currentNode.ExitNode();

                // Check if current node is a ParallelNode
                if (currentNode is ParallelNode parallelNode)
                {
                    // Get all output nodes from the parallel node
                    List<BaseGraphNode> outputNodes = parallelNode.GetAllOutputNodes(graphInstance);
                    
                    if (outputNodes.Count > 0)
                    {
                        // Start all output nodes in parallel
                        foreach (BaseGraphNode outputNode in outputNodes)
                        {
                            if (outputNode != null && !visitedNodes.Contains(outputNode.Guid))
                            {
                                ProcessAndMoveNextNode(outputNode, visitedNodes, depth + 1);
                            }
                        }
                    }
                }
                else
                {
                    // Standard single output execution
                    string nextNodeId = currentNode.OnProcess(graphInstance);

                    if (!string.IsNullOrEmpty(nextNodeId))
                    {
                        BaseGraphNode nextNode = graphInstance.GetNode(nextNodeId);
                        if (nextNode != null)
                        {
                            ProcessAndMoveNextNode(nextNode, visitedNodes, depth + 1);
                        }
                    }
                }
            }));
        }
        IEnumerator UpdateCurrentNodeTillComplete(BaseGraphNode node, Action ActionCallBack)
        {
            while (!node.IsCompleted)
            {
                node.UpdateNode();
                yield return new WaitForEndOfFrame();
            }

            ActionCallBack();
        }
        
    }
}