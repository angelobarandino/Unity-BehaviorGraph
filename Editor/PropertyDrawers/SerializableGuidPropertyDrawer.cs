using BehaviorGraph.Runtime;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableGuid))]
public class SerializableGuidPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Start property draw
        EditorGUI.BeginProperty(position, label, property);

        // Get property
        SerializedProperty serializedGuid = property.FindPropertyRelative("serializedGuid");

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        Rect pos = new Rect(position.xMin, position.yMin, position.width, position.height);
        EditorGUI.PropertyField(pos, serializedGuid, GUIContent.none);

        // End property
        EditorGUI.EndProperty();
    }
}