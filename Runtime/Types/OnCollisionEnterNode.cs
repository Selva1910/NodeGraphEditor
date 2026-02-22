using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("On Collision Enter", "Events/Physics/Collision Enter", false, true)]
    public class OnCollisionEnterNode : BaseEventNode
    {
        [ExposedProperty]
        [DisplayName("Target Tag")]
        public string targetTag = "Default";

        public override void StartNode()
        {
            base.StartNode();
            
            // Subscribe to collision enter event
            string eventName = $"OnCollisionEnter_{targetTag}";
            EventManager.Instance.Subscribe(eventName, OnCollisionEnterEvent);
        }

        public override void UpdateNode()
        {
            base.UpdateNode();
        }

        public override void ExitNode()
        {
            base.ExitNode();
            
            // Unsubscribe from event
            string eventName = $"OnCollisionEnter_{targetTag}";
            EventManager.Instance.Unsubscribe(eventName, OnCollisionEnterEvent);
        }

        private void OnCollisionEnterEvent()
        {
            IsCompleted = true;
        }
    }
}
