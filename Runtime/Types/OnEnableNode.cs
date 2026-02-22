using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("On Enable", "Events/Lifecycle/On Enable", false, true)]
    public class OnEnableNode : BaseEventNode
    {
        private const string ON_ENABLE_EVENT = "OnNodeEnable";

        public override void StartNode()
        {
            base.StartNode();
            
            // Subscribe to enable event and trigger immediately
            EventManager.Instance.Subscribe(ON_ENABLE_EVENT, OnEnableEvent);
            
            // Trigger immediately since the node is already enabled
            EventManager.Instance.Trigger(ON_ENABLE_EVENT);
        }

        public override void ExitNode()
        {
            base.ExitNode();
            
            // Unsubscribe from event
            EventManager.Instance.Unsubscribe(ON_ENABLE_EVENT, OnEnableEvent);
        }

        private void OnEnableEvent()
        {
            IsCompleted = true;
        }
    }
}
