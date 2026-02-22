using UnityEngine;
using UnityEngine.Serialization;

namespace NodeGraph
{
    
    [NodeInfo("Perform","Process/Perform", true, true)]
    public class RotateNode : BaseGraphNode
    {
        [ExposedProperty][DisplayName("Target")]
        public string target; 
        
        [ExposedProperty] [DisplayName("Angle")]
        public Vector3 rotation;

        [FormerlySerializedAs("Duration")] [ExposedProperty] [DisplayName("duration")]
        public float duration;

        [ExposedProperty] [DisplayName("Local Rotation")]
        public bool localRotation;

        private GameObject TargetObject;
        private IRotatableObject rotatableObject;
        public override void StartNode()
        {
            base.StartNode();
            TargetObject = SceneObjectManager.Instance.GetObjectByName(target);
            TargetObject.GetComponent<IRotatableObject>().TargetRotation = rotation;
            rotatableObject = TargetObject.GetComponent<IRotatableObject>();
            rotatableObject.Duration = duration;
           
            rotatableObject.IsLocalSpace = localRotation;
            rotatableObject.Perform();  
        }

        public override void UpdateNode()
        {
            base.UpdateNode();
            IsCompleted = rotatableObject.IsRotated;
        }
    }
}