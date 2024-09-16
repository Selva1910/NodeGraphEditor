using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [DefaultExecutionOrder(-50)]
    public class SceneObject : MonoBehaviour
    {
        public string uniqueName;

        void Awake()
        {
            SceneObjectManager.Instance.RegisterObject(uniqueName, gameObject);
        }

        void OnDestroy()
        {
            SceneObjectManager.Instance.UnregisterObject(uniqueName);
        }
    }

}
