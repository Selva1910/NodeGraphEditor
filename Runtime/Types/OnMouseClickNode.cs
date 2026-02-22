using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("On Mouse Click", "Events/Input/Mouse Click", false, true)]
    public class OnMouseClickNode : BaseEventNode
    {
        private int m_updateCounter = 0;

        public override void StartNode()
        {
            base.StartNode();
            m_updateCounter = 0;
            Debug.Log($"[OnMouseClickNode] Started - Waiting for mouse click");
        }

        public override void UpdateNode()
        {
            base.UpdateNode();

            m_updateCounter++;

            var mgr = InputEventManager.Instance;

            // Log every 60 frames (status) and echo manager state for mouse
            if (m_updateCounter % 60 == 0)
            {
                bool mouseDownNow = mgr.WasMouseDownThisFrame(0);
                Debug.Log($"[OnMouseClickNode] Still waiting... ({m_updateCounter} frames) | mgr.WasMouseDownThisFrame(0)={mouseDownNow} | frame={mgr.FrameCount} time={Time.time:F3}");
            }

            // If manager observed mouse down, echo and complete
            if (mgr.WasMouseDownThisFrame(0))
            {
                Debug.Log($"[OnMouseClickNode] mgr.MouseDown detected on frame {mgr.FrameCount} | time={Time.time:F3}");
                Debug.Log($"[OnMouseClickNode] *** MOUSE CLICKED! Completing node! *** (frame={mgr.FrameCount} time={Time.time:F3})");
                IsCompleted = true;
            }
        }

        public override void ExitNode()
        {
            base.ExitNode();
            Debug.Log($"[OnMouseClickNode] Exited after {m_updateCounter} frames - Node completed!");
        }
    }
}
