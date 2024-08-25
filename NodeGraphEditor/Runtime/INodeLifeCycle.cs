using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public interface INodeLifecycle
    {
        void StartNode();
        void UpdateNode();
        void ExitNode();
    }
}
