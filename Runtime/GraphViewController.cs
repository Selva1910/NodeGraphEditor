using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class GraphViewController : MonoBehaviour
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
            ExecuteAsset(graphInstance);
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

            if (visitedNodes.Contains(currentNode.guid))
            {
                Debug.LogError("Circular reference detected at node: " + currentNode.guid);
                return;
            }

            visitedNodes.Add(currentNode.guid);

            currentNode.Stage = LifeStage.Activating;
            currentNode.AwakeNode();

            currentNode.StartNode();

            StartCoroutine(UpdateCurrentNodeTillComplete(currentNode, () => {
                currentNode.ExitNode();
                string nextNodeId = currentNode.OnProcess(graphInstance);

                if (!string.IsNullOrEmpty(nextNodeId))
                {
                    BaseGraphNode nextNode = graphInstance.GetNode(nextNodeId);
                    ProcessAndMoveNextNode(nextNode, visitedNodes, depth + 1);
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
