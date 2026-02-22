using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    /// <summary>
    /// Centralized event manager for the node graph system
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        private static EventManager s_instance;
        
        public static EventManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    GameObject eventManagerGO = new GameObject("EventManager");
                    s_instance = eventManagerGO.AddComponent<EventManager>();
                    DontDestroyOnLoad(eventManagerGO);
                }
                return s_instance;
            }
        }

        // Dictionary to store event subscribers
        private Dictionary<string, List<Action>> m_eventSubscribers = new Dictionary<string, List<Action>>();
        private Dictionary<string, List<Action<object>>> m_eventSubscribersWithData = new Dictionary<string, List<Action<object>>>();

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Subscribe to an event
        /// </summary>
        public void Subscribe(string eventName, Action callback)
        {
            if (!m_eventSubscribers.ContainsKey(eventName))
            {
                m_eventSubscribers[eventName] = new List<Action>();
            }

            m_eventSubscribers[eventName].Add(callback);
        }

        /// <summary>
        /// Subscribe to an event with data
        /// </summary>
        public void Subscribe<T>(string eventName, Action<T> callback)
        {
            if (!m_eventSubscribersWithData.ContainsKey(eventName))
            {
                m_eventSubscribersWithData[eventName] = new List<Action<object>>();
            }

            m_eventSubscribersWithData[eventName].Add((data) => callback((T)data));
        }

        /// <summary>
        /// Unsubscribe from an event
        /// </summary>
        public void Unsubscribe(string eventName, Action callback)
        {
            if (m_eventSubscribers.ContainsKey(eventName))
            {
                m_eventSubscribers[eventName].Remove(callback);
            }
        }

        /// <summary>
        /// Unsubscribe from an event with data
        /// </summary>
        public void Unsubscribe<T>(string eventName, Action<T> callback)
        {
            if (m_eventSubscribersWithData.ContainsKey(eventName))
            {
                m_eventSubscribersWithData[eventName].RemoveAll(action => action.Target == callback.Target);
            }
        }

        /// <summary>
        /// Trigger an event (call all subscribers)
        /// </summary>
        public void Trigger(string eventName)
        {
            if (m_eventSubscribers.ContainsKey(eventName))
            {
                foreach (var callback in m_eventSubscribers[eventName])
                {
                    callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Trigger an event with data
        /// </summary>
        public void Trigger<T>(string eventName, T data)
        {
            if (m_eventSubscribersWithData.ContainsKey(eventName))
            {
                foreach (var callback in m_eventSubscribersWithData[eventName])
                {
                    callback?.Invoke(data);
                }
            }
        }

        /// <summary>
        /// Clear all subscribers for an event
        /// </summary>
        public void ClearEvent(string eventName)
        {
            if (m_eventSubscribers.ContainsKey(eventName))
            {
                m_eventSubscribers[eventName].Clear();
            }

            if (m_eventSubscribersWithData.ContainsKey(eventName))
            {
                m_eventSubscribersWithData[eventName].Clear();
            }
        }
    }
}
