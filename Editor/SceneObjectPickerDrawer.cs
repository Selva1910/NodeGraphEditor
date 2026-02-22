using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using NodeGraph;
namespace NodeGraph.Editor
{
    [CustomPropertyDrawer(typeof(SceneObjectPickerAttribute))]
    public class SceneObjectPickerDrawer : PropertyDrawer
    {
        private const int PICKER_ID = 99123;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use it on string fields only.");
                return;
            }

            Rect fieldRect = new Rect(position.x, position.y, position.width - 60, position.height);
            Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 55, position.height);

            // Draw the string field showing the object name
            EditorGUI.PropertyField(fieldRect, property, label);

            // Draw pick button
            if (GUI.Button(buttonRect, "Pick"))
            {
                // Open the object picker restricted to scene objects
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, true, "", PICKER_ID);
            }

            // Handle object picker result
            if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == PICKER_ID)
            {
                GameObject pickedObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                if (pickedObject != null)
                {
                    if(pickedObject.TryGetComponent<SceneObject>(out var sceneObj))
                    {
                        property.stringValue = sceneObj.uniqueName;
                    }
                    else
                    {
                        pickedObject.AddComponent<SceneObject>();
                        pickedObject.GetComponent<SceneObject>().uniqueName = pickedObject.name;
                        property.stringValue = pickedObject.GetComponent<SceneObject>().uniqueName;
                    }
                    
                    property.serializedObject.ApplyModifiedProperties();
                }
                Event.current.Use();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}
