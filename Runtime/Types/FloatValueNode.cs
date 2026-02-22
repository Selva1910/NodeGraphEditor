using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Float","Values/Float")]
    public class FloatValueNode : BaseGraphNode
    {
        [ExposedProperty]
        [ExposedOutNode]
        public float value;
    }
}
