using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Move", "Process/Move Object", true, true)]
    public class MoveNode : BaseGraphNode
    {
        [ExposedProperty]
        public Vector3 direction;

        [ExposedProperty]
        public bool isAnimated;

        public override string OnProcess(GraphAssetSO currentGraph)
        {
            currentGraph.gameObject.transform.localPosition += direction;
            return base.OnProcess(currentGraph);
        }

    }
}
