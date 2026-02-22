using UnityEngine;

namespace NodeGraph
{
    /// <summary>
    /// Marks a string field as needing a scene object picker button in the inspector.
    /// The button allows selecting a GameObject from the scene and stores its name in the string.
    /// </summary>
    public class SceneObjectPickerAttribute : PropertyAttribute
    {
    }
}
