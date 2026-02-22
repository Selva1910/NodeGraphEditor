using UnityEngine;
namespace NodeGraph
{
    public interface IRotatableObject
    {
        public bool IsRotated { get; }
        Vector3 TargetRotation { get; set; }
        bool IsLocalSpace { get; set; }
        public float Duration { get; set; }
        void Perform();
    }
}