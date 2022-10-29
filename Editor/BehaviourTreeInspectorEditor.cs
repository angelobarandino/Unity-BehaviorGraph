using BehaviourGraph.Runtime;
using UnityEditor;
using UnityEngine;

namespace BehaviourGraph.Editor
{
    [CustomEditor(typeof(BehaviorOwner))]
    public class BehaviourTreeInspectorEditor : UnityEditor.Editor
    {
        private BehaviorOwner owner;
        private void OnEnable()
        {
            owner = target as BehaviorOwner;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            DrawBehaviorAssetField();
            DrawUpdateModeField();
        }

        private void DrawBehaviorAssetField()
        {
            GUILayout.BeginHorizontal();
            var behaviorAssetProperty = serializedObject.FindProperty("behaviorAsset");
            GUILayout.Label(new GUIContent("Behavior Asset"), GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(behaviorAssetProperty, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            if (behaviorAssetProperty.objectReferenceValue == null)
            {
                if (GUILayout.Button("CREATE", GUILayout.Width(80)))
                    owner.SetBehavior(BehaviorAssetUtility.Create<BehaviorAsset>(
                        title: "Create Behavior Asset",
                        filename: "BehaviorAsset"
                    ));
            }
            else
            {
                if (GUILayout.Button("OPEN", GUILayout.Width(80)))
                {
                    Selection.activeObject = target;
                    BehaviourGraphEditorWindow.OpenWindow();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawUpdateModeField()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateMode"));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
