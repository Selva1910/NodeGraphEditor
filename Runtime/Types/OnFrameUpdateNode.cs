using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("On Frame Update", "Events/Timing/On Frame Update", false, true)]
    public class OnFrameUpdateNode : BaseEventNode
    {
        [ExposedProperty]
        [DisplayName("Execute Once")]
        public bool executeOnce = true;

        private const string FRAME_UPDATE_EVENT = "OnFrameUpdate";
        private bool m_hasExecuted = false;

        public override void StartNode()
        {
            base.StartNode();
            m_hasExecuted = false;
            
            // Subscribe to frame update event
            EventManager.Instance.Subscribe(FRAME_UPDATE_EVENT, OnFrameUpdate);
        }

        public override void UpdateNode()
        {
            base.UpdateNode();
            
            // Trigger frame update event
            EventManager.Instance.Trigger(FRAME_UPDATE_EVENT);
        }

        public override void ExitNode()
        {
            base.ExitNode();
            
            // Unsubscribe from event
            EventManager.Instance.Unsubscribe(FRAME_UPDATE_EVENT, OnFrameUpdate);
        }

        private void OnFrameUpdate()
        {
            if (executeOnce && m_hasExecuted)
                return;

            IsCompleted = true;
            m_hasExecuted = true;
        }
    }
}
