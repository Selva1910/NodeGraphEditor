using UnityEngine;

namespace NodeGraph
{
    public class TestEvent : MonoBehaviour, ITestEvent
    {
        [field:SerializeField] public bool success { get; set; }
    }
}