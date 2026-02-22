using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    /// <summary>
    /// Central per-frame input capture for the node runtime.
    /// Polls Unity's Input once per frame in Update() and exposes fast query methods
    /// and simple events. Ensures consistent per-frame semantics for nodes.
    /// </summary>
    public class InputEventManager : MonoBehaviour
    {
        static InputEventManager s_instance;

        public static InputEventManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var existing = FindObjectOfType<InputEventManager>();
                    if (existing != null)
                    {
                        s_instance = existing;
                    }
                    else
                    {
                        var go = new GameObject("NodeGraph_InputEventManager");
                        s_instance = go.AddComponent<InputEventManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return s_instance;
            }
        }

        public int FrameCount { get; private set; }
        public bool AnyKeyDown { get; private set; }

        readonly HashSet<KeyCode> m_keysDown = new HashSet<KeyCode>();
        readonly HashSet<int> m_mouseButtonsDown = new HashSet<int>();

        public event Action<KeyCode> OnKeyDown;
        public event Action<int> OnMouseDown;
        public event Action OnAnyKeyDown;

        void Awake()
        {
            if (s_instance == null) s_instance = this;
            else if (s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            FrameCount = Time.frameCount;
            AnyKeyDown = Input.anyKeyDown;

            m_keysDown.Clear();
            m_mouseButtonsDown.Clear();

            if (AnyKeyDown)
            {
                // Only enumerate KeyCode values when any key down occurred this frame.
                foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(k))
                    {
                        m_keysDown.Add(k);
                        OnKeyDown?.Invoke(k);
                    }
                }

                OnAnyKeyDown?.Invoke();
            }

            // Mouse buttons (0..2) - extend if you need more
            for (int b = 0; b <= 2; b++)
            {
                if (Input.GetMouseButtonDown(b))
                {
                    m_mouseButtonsDown.Add(b);
                    OnMouseDown?.Invoke(b);
                }
            }
        }

        public bool WasKeyDownThisFrame(KeyCode key)
        {
            return m_keysDown.Contains(key);
        }

        public bool WasMouseDownThisFrame(int button)
        {
            return m_mouseButtonsDown.Contains(button);
        }
    }
}
