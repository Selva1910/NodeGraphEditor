using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("On Trigger Enter", "Events/Physics/Trigger Enter", false, true)]
    public class OnTriggerEnterNode : BaseEventNode
    {
        [ExposedProperty]
        [DisplayName("Target Tag")]
        public string targetTag = "Player";

        public override void StartNode()
        {
            base.StartNode();
            
            // Subscribe to trigger enter event
            string eventName = $"OnTriggerEnter_{targetTag}";
            EventManager.Instance.Subscribe(eventName, OnTriggerEnterEvent);
        }

        public override void UpdateNode()
        {
            base.UpdateNode();
        }

        public override void ExitNode()
        {
            base.ExitNode();
            
            // Unsubscribe from event
            string eventName = $"OnTriggerEnter_{targetTag}";
            EventManager.Instance.Unsubscribe(eventName, OnTriggerEnterEvent);
        }

        private void OnTriggerEnterEvent()
        {
            IsCompleted = true;
        }
    }
}
