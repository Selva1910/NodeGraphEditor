using UnityEngine;

namespace NodeGraph
{
    // Attach this to any active GameObject in the scene while in Play mode
    // It will log input events to help diagnose Input.GetKeyDown / mouse issues
    public class TestInputDetector : MonoBehaviour
    {
        void Update()
        {
            if (Input.anyKeyDown)
            {
                Debug.Log("[TestInputDetector] anyKeyDown detected");
            }

            // Check a few common keys
            if (Input.GetKeyDown(KeyCode.Space)) Debug.Log("[TestInputDetector] Space pressed");
            if (Input.GetKeyDown(KeyCode.F)) Debug.Log("[TestInputDetector] F pressed");
            if (Input.GetKeyDown(KeyCode.Escape)) Debug.Log("[TestInputDetector] Escape pressed");

            // Mouse
            if (Input.GetMouseButtonDown(0)) Debug.Log("[TestInputDetector] Mouse left click");
            if (Input.GetMouseButtonDown(1)) Debug.Log("[TestInputDetector] Mouse right click");
        }
    }
}