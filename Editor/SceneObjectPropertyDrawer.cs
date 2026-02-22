#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

namespace NodeGraph.Editor
{

    // [CustomPropertyDrawer(typeof(SceneObject))]
    // public class SceneObjectDrawer : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         var uniqueNameProp = property.FindPropertyRelative("uniqueName");
    //         var referenceProp = property.FindPropertyRelative("reference");
    //
    //         EditorGUI.BeginProperty(position, label, property);
    //         var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
    //         EditorGUI.LabelField(labelRect, label);
    //
    //         var refRect = new Rect(position.x, position.y + 20, position.width, EditorGUIUtility.singleLineHeight);
    //         EditorGUI.PropertyField(refRect, referenceProp);
    //
    //         var nameRect = new Rect(position.x, position.y + 40, position.width, EditorGUIUtility.singleLineHeight);
    //         EditorGUI.PropertyField(nameRect, uniqueNameProp);
    //
    //         EditorGUI.EndProperty();
    //     }
    //
    //     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //     {
    //         return EditorGUIUtility.singleLineHeight * 3;
    //     }
    // }
#endif
}
