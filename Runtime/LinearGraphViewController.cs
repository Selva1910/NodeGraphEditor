using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NodeGraph
{
    public class LinearGraphViewController : MonoBehaviour
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
            StartProcess(graphInstance);
        }


        public void StartProcess(GraphAssetSO instance)
        {
            ExecuteAsset(instance);
        }

        private void ExecuteAsset(GraphAssetSO codeGraphAsset)
        {
            graphInstance.Init(gameObject);

            BaseGraphNode startNode = graphInstance.GetStartNode();

            if (startNode != null)
            {
                // Add safety checks to avoid endless loop
                HashSet<string> visitedNodes = new HashSet<string>();
                ProcessAndMoveNextNode(startNode, visitedNodes, 0);
            }
            else
            {
                Debug.LogError("Start node not found in the graph.");
            }
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
                        ProcessAndMoveNextNode(nextNode, visitedNodes, depth + 1);
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
