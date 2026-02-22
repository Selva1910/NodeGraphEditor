using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("On Key Press", "Events/Input/Key Press", false, true)]
    public class OnKeyPressNode : BaseEventNode
    {
        [ExposedProperty]
        [DisplayName("Key Code")]
        public KeyCode targetKey = KeyCode.Space;

        private int m_updateCounter = 0;

        public override void StartNode()
        {
            base.StartNode();
            m_updateCounter = 0;
            Debug.Log($"[OnKeyPressNode] Started - Waiting for key press: {targetKey}");
        }

        public override void UpdateNode()
        {
            base.UpdateNode();

            m_updateCounter++;

            var mgr = InputEventManager.Instance;

            // Log every 60 frames (status) and echo input manager state
            if (m_updateCounter % 60 == 0)
            {
                bool any = mgr.AnyKeyDown;
                bool keyDownNow = mgr.WasKeyDownThisFrame(targetKey);
                //Debug.Log($"[OnKeyPressNode] Still waiting... ({m_updateCounter} frames). Target key: {targetKey} | mgr.AnyKeyDown={any} | mgr.WasKeyDownThisFrame={keyDownNow} | frame={mgr.FrameCount} time={Time.time:F3}");
            }

            // If any keydown occurred this frame (as seen by manager), echo it for comparison
            if (mgr.AnyKeyDown)
            {
                bool keyDownNow = mgr.WasKeyDownThisFrame(targetKey);
              //  Debug.Log($"[OnKeyPressNode] mgr.AnyKeyDown detected on frame {mgr.FrameCount}: mgr.WasKeyDownThisFrame({targetKey})={keyDownNow} | time={Time.time:F3}");
            }

            // Use manager query instead of calling Input directly
            if (mgr.WasKeyDownThisFrame(targetKey))
            {
            //    Debug.Log($"[OnKeyPressNode] *** KEY '{targetKey}' PRESSED! Completing node! *** (frame={mgr.FrameCount} time={Time.time:F3})");
                IsCompleted = true;
            }
        }

        public override void ExitNode()
        {
            base.ExitNode();
            Debug.Log($"[OnKeyPressNode] Exited after {m_updateCounter} frames - Node completed!");
        }
    }
}
