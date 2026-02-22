using UnityEngine.EventSystems;
using UnityEngine;

namespace NodeGraph
{
    public class BaseEventNode : BaseGraphNode
    {
        [ExposedProperty]
        [DisplayName("Allow External Trigger")]
        public bool allowExternalTrigger = false;
    }
}