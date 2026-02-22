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
        [DisplayName("Target Position")]
        public Vector3 targetPosition;
        
        [ExposedProperty]
        [DisplayName("Target")]
        [SceneObjectPicker]
        public string moveObject;
        
        [ExposedProperty]
        [DisplayName("duration")]
        public float moveDuration;
        
        [ExposedProperty]
        [DisplayName("Is Local Position")]
        public bool isLocalPosition; 
        
        private GameObject TargetObject;
        private IMovableObject movableObject;
        public override void StartNode()
        {
            base.StartNode();
            TargetObject = SceneObjectManager.Instance.GetObjectByName(moveObject);
            if (TargetObject == null)
            {
                Debug.LogError($"[MoveNode] Target object '{moveObject}' not found. Aborting MoveNode.");
                IsCompleted = true;
                return;
            }

            var comp = TargetObject.GetComponent<IMovableObject>();
            if (comp == null)
            {
                Debug.LogError($"[MoveNode] Target object '{moveObject}' does not implement IMovableObject. Aborting MoveNode.");
                IsCompleted = true;
                return;
            }

            comp.TargetPosition = targetPosition;
            movableObject = comp;
            movableObject.Duration = moveDuration;
            movableObject.IsLocalSpace = isLocalPosition;
            movableObject.StartMove();
        } 
        public override void UpdateNode()
        {
            base.UpdateNode();
            if (TargetObject == null || movableObject == null)
            {
                IsCompleted = true;
                return;
            }

            IsCompleted = movableObject.IsMoved;
        }

    }
}
