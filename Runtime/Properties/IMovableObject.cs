using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public interface IMovableObject 
    {
        public bool IsMoved { get; }
        Vector3 TargetPosition { get; set; }
        bool IsLocalSpace { get; set; }
        public float Duration { get; set; }

        void StartMove();
    }
}
