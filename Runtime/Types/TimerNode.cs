using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace NodeGraph
{
    [NodeInfo("Timer","Process/Timer",true, true)]
    public class TimerNode : BaseGraphNode
    {
        
        [ExposedProperty]
        [DisplayName("Timer")]
        public float duration = 1.0f;


        public override void StartNode()
        {
            base.StartNode();
            SceneObjectManager.Instance.StartCoroutine(StartTimer(duration));
        }

        IEnumerator StartTimer(float timer)
        {
            yield return new WaitForSecondsRealtime(timer);
            Debug.Log("Timer Complete");
            IsCompleted = true;
        }
    }
}
