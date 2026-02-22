using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Test Event Manager", "Events/Debug/Test Event Manager", true, true)]
    public class TestEventManagerNode : BaseEventNode
    {
        private int m_updateCounter = 0;
        private const string TEST_EVENT = "TestEventManagerEvent";

        public override void StartNode()
        {
            base.StartNode();
            m_updateCounter = 0;

            // Subscribe to test event using inline lambda
            EventManager.Instance.Subscribe(TEST_EVENT, () =>
            {
                Debug.Log("[TestEventManagerNode] *** TEST EVENT CALLBACK TRIGGERED ***");
                IsCompleted = true;
            });

            Debug.Log("[TestEventManagerNode] Started - will trigger event after 5 updates");
        }

        public override void UpdateNode()
        {
            base.UpdateNode();
            m_updateCounter++;

            // Trigger the event after 5 frames to test the system
            if (m_updateCounter == 5)
            {
                Debug.Log("[TestEventManagerNode] Triggering test event...");
                EventManager.Instance.Trigger(TEST_EVENT);
                Debug.Log("[TestEventManagerNode] Test event triggered");
            }

            if (m_updateCounter <= 10)
            {
                Debug.Log($"[TestEventManagerNode] Update {m_updateCounter}, IsCompleted: {IsCompleted}");
            }
        }

        public override void ExitNode()
        {
            base.ExitNode();
            EventManager.Instance.Unsubscribe(TEST_EVENT, () => { });
            Debug.Log($"[TestEventManagerNode] Exited after {m_updateCounter} updates");
        }
    }
}
