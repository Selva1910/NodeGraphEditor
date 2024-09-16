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
        public string moveObject;


        public override void UpdateNode()
        {
            base.UpdateNode();
            GameObject TargetObject = SceneObjectManager.Instance.GetObjectByName(moveObject);
            TargetObject.GetComponent<IMovableObject>().TargetPosition = targetPosition;
            IsCompleted = TargetObject.GetComponent<IMovableObject>().IsMoved;
        }

    }
}
