using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Wait for Seconds", "Events/Timing/Wait for Seconds", true, true)]
    public class WaitForSecondsNode : BaseEventNode
    {
        [ExposedProperty]
        [DisplayName("Duration (seconds)")]
        public float duration = 1f;

        private float m_elapsedTime = 0f;
        private string m_eventName;

        public override void StartNode()
        {
            base.StartNode();
            m_elapsedTime = 0f;
            
            // Create unique event name for this wait
            m_eventName = $"WaitForSeconds_{Guid}_{duration}";
            
            // Subscribe to the wait completion event
            EventManager.Instance.Subscribe(m_eventName, OnWaitComplete);
        }

        public override void UpdateNode()
        {
            base.UpdateNode();
            
            m_elapsedTime += Time.deltaTime;
            
            if (m_elapsedTime >= duration)
            {
                // Trigger the wait complete event
                EventManager.Instance.Trigger(m_eventName);
            }
        }

        public override void ExitNode()
        {
            base.ExitNode();
            
            // Unsubscribe from event
            if (!string.IsNullOrEmpty(m_eventName))
            {
                EventManager.Instance.Unsubscribe(m_eventName, OnWaitComplete);
            }
        }

        private void OnWaitComplete()
        {
            IsCompleted = true;
        }
    }
}
