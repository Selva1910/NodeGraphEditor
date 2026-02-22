using UnityEngine;

namespace NodeGraph
{
    [NodeInfo("Test Event", "Events/Test", true, true)]
    public class OnTestNode : BaseEventNode
    {
        [ExposedProperty]
        [DisplayName("Test Name")]
        public string testName;
        
        
        public override void StartNode()
        {
            base.StartNode();
        }
        
        public override void UpdateNode()
        {
            base.UpdateNode();
            Debug.Log("Eventing");
            var TargetObject = SceneObjectManager.Instance.GetObjectByName(testName);
            IsCompleted = TargetObject.GetComponent<ITestEvent>().success;
        }
    }
}