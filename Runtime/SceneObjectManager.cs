using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [DefaultExecutionOrder(-100)]
    public class SceneObjectManager : MonoBehaviour
    {
        public static SceneObjectManager Instance { get; private set; }

        private Dictionary<string, GameObject> registeredObjects = new Dictionary<string, GameObject>();

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void RegisterObject(string name, GameObject obj)
        {
            if (!registeredObjects.ContainsKey(name))
            {
                registeredObjects.Add(name, obj);
            }
            else
            {
                Debug.LogError($"An object with the name {name} is already registered.");
            }
        }

        public void UnregisterObject(string name)
        {
            if (registeredObjects.ContainsKey(name))
            {
                registeredObjects.Remove(name);
            }
            else
            {
                Debug.LogError($"No object with the name {name} is registered.");
            }
        }

        public GameObject GetObjectByName(string name)
        {
            registeredObjects.TryGetValue(name, out GameObject obj);
            return obj;
        }
    }


}
